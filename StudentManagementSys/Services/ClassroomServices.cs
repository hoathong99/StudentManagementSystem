﻿using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Model;
using StudentManagementSys.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace StudentManagementSys.Services
{
    public class ClassroomServices
    {
        private readonly StudentManagementSysContext _context;
        private readonly StudentServices _studentService;
        
        public ClassroomServices(StudentManagementSysContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _studentService = new StudentServices(context, userManager);
        }

        //AutoMapper Configuration
        private MapperConfiguration config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<Classroom, ClassroomDto>()
                    .ForMember(des => des.StudentsID, act => act.MapFrom(scr => mapStringToList(scr.StudentsID)))
        );

        private MapperConfiguration configReversed = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ClassroomDto, Classroom>()
                    .ForMember(des => des.StudentsID, act => act.MapFrom(scr => String.Join(',', scr.StudentsID.Where(s => !string.IsNullOrEmpty(s)))))
        );

        private static List<String> mapStringToList(String? a)
        {
            return String.IsNullOrEmpty(a) ? new List<String>() : a.Split(",").ToList();
        }

        //Methods
        public async Task<Boolean> RegisterClassroomAsync(ClassroomDto stDto) {

            _context.Add(new Mapper(configReversed).Map<Classroom>(stDto));
            await _context.SaveChangesAsync();
            return _context.SaveChangesAsync().IsCompletedSuccessfully;
        }

        public async Task<List<ClassroomDto>> GetAllClassroom()
        {
            var mapper = new Mapper(config);


            if (_context.Classroom == null)
            {
                return null;
            }
            List<Classroom> lsStu = await _context.Classroom.ToListAsync();
            List<ClassroomDto> lsStuDto = new List<ClassroomDto>();

            foreach (Classroom s in lsStu)
            {
                lsStuDto.Add(mapper.Map<ClassroomDto>(s));
            }

            return lsStuDto;
        }

        public async Task<ClassroomDto> GetClassroom(String id)
        {
            var mapper = new Mapper(config);

            if (id == null || _context.Classroom == null)
            {
                return null;
            }
            Classroom classroom = await _context.Classroom
                .FirstOrDefaultAsync(m => m.CRID == id);
            if (classroom == null)
            {
                return null;
            }
            var rs = mapper.Map<ClassroomDto>(classroom);
            return rs;
        }

        public async Task<bool> RemoveMinitor(String cId)
        {
            var classroom = await this.GetClassroom(cId);
            classroom.MonitorID = null;
            var rs = await this.UpdateClassroom(cId,classroom);
            if(rs == null)
            {
                return false;
            }
            return true;
        }

        public async Task<ClassroomDto> UpdateClassroom(string id, ClassroomDto stuDto)
        {
            if (id != stuDto.CRID)
            {
                return null;
            }
            var classroom = new Mapper(configReversed).Map<Classroom>(stuDto);
            try
            {
                _context.ChangeTracker.Clear();
                _context.Update(classroom);
                await _context.SaveChangesAsync();
                return stuDto;
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
                throw;
            }
        }

        //delete classroom, update classroom/student db
        public async Task<Boolean> Delete(string id)
        {
            if (_context.Classroom == null)
            {
                return false;
            }
            var classroomDto = await this.GetClassroom(id);
            var classroom = new Mapper(configReversed).Map<Classroom>(classroomDto);

            if(classroomDto.StudentsID != null)
            {
                foreach (var s in classroomDto.StudentsID)
                {
                    await RemoveStudent(s, id);
                }
            }
                
            if (classroom != null)
            {
                _context.Classroom.Remove(classroom);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        //add student to classroom and remove them from previous one
        public async Task<Boolean> AddStudent(String sId, String cId)
        {
            var classroom = await GetClassroom(cId);
            if (classroom == null) { return false; }

            var student = await _studentService.GetStudent(sId);
            if (student == null) { return false; }

            await RemoveStudent(sId, student.ClassRoomID);                                // remove student from it classroom

            var classroomDto = new Mapper(config).Map<ClassroomDto>(classroom);

            if(classroomDto.StudentsID== null)
            {
                classroomDto.StudentsID = new List<string>();
            }

            if (!classroomDto.StudentsID.Contains(sId))
            {
                classroomDto.StudentsID.Add(sId);
                student.ClassRoomID = cId;
            }
            
            var rs = await UpdateClassroom(classroom.CRID, classroomDto);
            var rsStu = await _studentService.UpdateStudent(sId, student);
            if (rs == null || rsStu == null)
            {
                return false;
            }
            System.Console.WriteLine("Student added to class /n");
            return true;
        }

        //Remove student from classroom and update both classroom/student db
        public async Task<Boolean> RemoveStudent(String sId, String cId)
        {
            var classroom = await GetClassroom(cId);
            var student = await _studentService.GetStudent(sId);
            if (student == null)
            {
                return false;
            }

            var classroomDto = new Mapper(config).Map<ClassroomDto>(classroom);
            if(classroomDto == null)
            {
                classroomDto = new ClassroomDto();
            }
            if (classroomDto.StudentsID == null)
            {
                classroomDto.StudentsID = new List<string>();
            }
            if (classroomDto.MonitorID == sId)                                      // reset monitor if student removed is that of the classroom
            {
                classroomDto.MonitorID = null;
            }
            classroomDto.StudentsID.Remove(sId);
            
            student.ClassRoomID = null;
            if( classroom != null)
            {
                var rs = await UpdateClassroom(classroom.CRID, classroomDto);       //update classroom db
                if(rs == null)
                {
                    return false;
                }
            }
            var rsStu = await _studentService.UpdateStudent(sId, student);          //update student db
            if (rsStu == null)
            {
                return false;
            }
            System.Console.WriteLine("Student removed from class /n");
            return true;
        }

    }
}
