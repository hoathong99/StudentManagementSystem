using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Model;
using StudentManagementSys.Data;
using System.Runtime.CompilerServices;

namespace StudentManagementSys.Services
{
    public class ClassroomServices
    {
        private readonly StudentManagementSysContext _context;

        public ClassroomServices(StudentManagementSysContext context)
        {
            _context = context;
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

        public async Task<Boolean> Delete(string id)
        {
            if (_context.Classroom == null)
            {
                return false;
            }
            var classroom = await _context.Classroom.FindAsync(id);
            if (classroom != null)
            {
                _context.Classroom.Remove(classroom);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Boolean> AddStudent(List<String> sIdList, String cId)
        {
            var classroom = await GetClassroom(cId);
            classroom.StudentsID.Union(sIdList);
            var classroomDto = new Mapper(config).Map<ClassroomDto>(classroom);
            var rs = await UpdateClassroom(classroom.CRID, classroomDto);
            if(rs == null)
            {
                return false;
            }
            return true;
        }

    }
}
