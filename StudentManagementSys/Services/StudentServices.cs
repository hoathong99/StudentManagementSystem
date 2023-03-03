using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Model;
using StudentManagementSys.Data;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;

namespace StudentManagementSys.Services
{
    public class StudentServices
    {
        private readonly StudentManagementSysContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public StudentServices(StudentManagementSysContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //AutoMapper Configuration
        private MapperConfiguration config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<Student, StudentDto>()
                    .ForMember(des => des.SubjectEnlisted, act => act.MapFrom(scr => mapStringToList(scr.SubjectEnlisted)))
        );

        private MapperConfiguration configReversed = new MapperConfiguration(cfg =>
                    cfg.CreateMap<StudentDto, Student>()
                    .ForMember(des => des.SubjectEnlisted, act => act.MapFrom(scr => String.Join(',', scr.SubjectEnlisted.Where(s => !string.IsNullOrEmpty(s)))))
        );

        private static List<String> mapStringToList(String? a)
        {
            return String.IsNullOrEmpty(a) ? new List<String>() : a.Split(",").ToList();
        }

        //Methods
        public async Task<Boolean> RegisterStudentAsync(StudentDto stDto) {

            _context.Add(new Mapper(configReversed).Map<Student>(stDto));
            await _context.SaveChangesAsync();
            return _context.SaveChangesAsync().IsCompletedSuccessfully;
        }

        public async Task<List<StudentDto>> GetAllStudents()
        {
            var mapper = new Mapper(config);


            if (_context.Student == null)
            {
                return null;
            }
            List<Student> lsStu = await _context.Student.ToListAsync();
            List<StudentDto> lsStuDto = new List<StudentDto>();

            foreach (Student s in lsStu)
            {
                lsStuDto.Add(mapper.Map<StudentDto>(s));
            }

            return lsStuDto;
        }

        public async Task<List<StudentDto>> GetStudentsByClass(string cid)
        {
            var mapper = new Mapper(config);


            if (_context.Student == null)
            {
                return null;
            }
            List<Student> lsStu = _context.Student.Where(x => x.ClassRoomID == cid).ToList();
            List<StudentDto> lsStuDto = new List<StudentDto>();

            //foreach (Student s in lsStu)
            //{
            //    lsStuDto.Add(mapper.Map<StudentDto>(s));
            //}
            lsStuDto = mapper.Map<List<StudentDto>>(lsStu);

            return lsStuDto;
        }

        public async Task<StudentDto> GetStudent(String id)
        {
            var mapper = new Mapper(config);

            if (id == null || _context.Student == null)
            {
                return null;
            }
            Student student = await _context.Student
                .FirstOrDefaultAsync(m => m.UID == id);
            if (student == null)
            {
                return null;
            }
            var rs = mapper.Map<StudentDto>(student);
            return rs;
        }

        public async Task<StudentDto> GetStudentByAccount(String id)
        {
            var mapper = new Mapper(config);

            if (id == null || _context.Student == null)
            {
                return null;
            }
            Student student = await _context.Student
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (student == null)
            {
                return null;
            }
            var rs = mapper.Map<StudentDto>(student);
            return rs;
        }

        public async Task<List<StudentDto>> GetStudentsByClassSubject(string cid)
        {
            var mapper = new Mapper(config);


            if (_context.Student == null)
            {
                return null;
            }
            List<Student> lsStu = _context.Student.Where(x => x.SubjectEnlisted.Contains(cid)).ToList();
            List<StudentDto> lsStuDto = new List<StudentDto>();

            lsStuDto = mapper.Map<List<StudentDto>>(lsStu);

            return lsStuDto;
        }

        public async Task<StudentDto> UpdateStudent(string id, StudentDto stuDto)
        {
            if (id != stuDto.UID)
            {
                return null;
            }
            var Student = new Mapper(configReversed).Map<Student>(stuDto);
            try
            {
                _context.ChangeTracker.Clear();
                _context.Update(Student);
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
            if (_context.Student == null)
            {
                return false;
            }
            var student = await _context.Student.FindAsync(id);
            if (student != null)
            {
                _context.Student.Remove(student);
            }

            var user = _userManager.Users.FirstOrDefault(x => x.Id == student.AccountId);
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
