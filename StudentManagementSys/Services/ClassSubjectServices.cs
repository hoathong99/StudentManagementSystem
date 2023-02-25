using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Model;
using StudentManagementSys.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace StudentManagementSys.Services
{
    public class ClassSubjectServices
    {
        private readonly StudentManagementSysContext _context;

        public ClassSubjectServices(StudentManagementSysContext context)
        {
            _context = context;
        }

        //AutoMapper Configuration
        private MapperConfiguration config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ClassSubject, ClassSubjectDto>()
                    .ForMember(des => des.lstStudentID, act => act.MapFrom(scr => mapStringToList(scr.lstStudentID)))
        );

        private MapperConfiguration configReversed = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ClassSubjectDto, ClassSubject>()
                    .ForMember(des => des.lstStudentID, act => act.MapFrom(scr => String.Join(',', scr.lstStudentID.Where(s => !string.IsNullOrEmpty(s)))))
        );

        private static List<String> mapStringToList(String? a)
        {
            return String.IsNullOrEmpty(a) ? new List<String>() : a.Split(",").ToList();
        }

        //Methods
        public async Task<Boolean> RegisterClassSubjectAsync(ClassSubjectDto stDto) {

            _context.Add(new Mapper(configReversed).Map<ClassSubject>(stDto));
            await _context.SaveChangesAsync();
            return _context.SaveChangesAsync().IsCompletedSuccessfully;
        }

        public async Task<List<ClassSubjectDto>> GetAllClassSubject()
        {
            var mapper = new Mapper(config);


            if (_context.ClassSubject == null)
            {
                return null;
            }
            List<ClassSubject> lsStu = await _context.ClassSubject.ToListAsync();
            List<ClassSubjectDto> lsStuDto = new List<ClassSubjectDto>();

            foreach (ClassSubject s in lsStu)
            {
                lsStuDto.Add(mapper.Map<ClassSubjectDto>(s));
            }

            return lsStuDto;
        }

        public async Task<ClassSubjectDto> GetClassSubject(String id)
        {
            var mapper = new Mapper(config);

            if (id == null || _context.ClassSubject == null)
            {
                return null;
            }
            ClassSubject classSubject = await _context.ClassSubject
                .FirstOrDefaultAsync(m => m.classSubjectId == id);
            if (classSubject == null)
            {
                return null;
            }
            var rs = mapper.Map<ClassSubjectDto>(classSubject);
            return rs;
        }

        public async Task<ClassSubjectDto> UpdateClassSubject(string id, ClassSubjectDto stuDto)
        {
            if (id != stuDto.classSubjectId)
            {
                return null;
            }
            var classSubject = new Mapper(configReversed).Map<ClassSubject>(stuDto);
            try
            {
                _context.ChangeTracker.Clear();
                _context.Update(classSubject);
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
            if (_context.ClassSubject == null)
            {
                return false;
            }
            var classSubject = await _context.ClassSubject.FindAsync(id);
            if (classSubject != null)
            {
                _context.ClassSubject.Remove(classSubject);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Boolean> AddStudent(string CsId, string sId)
        {
            var Cs = await this.GetClassSubject(CsId);
            if (Cs == null)
            {
                return false;
            }
            if(Cs.lstStudentID == null)
            {
                Cs.lstStudentID = new List<string>();
            }
            Cs.lstStudentID.Add(sId);
            var rs = await this.UpdateClassSubject(CsId, Cs);
            return this.UpdateClassSubject(CsId, Cs).IsCompletedSuccessfully;
        }

        public async Task<Boolean> AddStudent(string CsId, List<string> lst)
        {
            var Cs = await this.GetClassSubject(CsId);
            if (Cs == null)
            {
                return false;
            }
            if (Cs.lstStudentID == null)
            {
                Cs.lstStudentID = new List<string>();
            }
            Cs.lstStudentID.AddRange(lst);
            var rs = await this.UpdateClassSubject(CsId, Cs);
            return this.UpdateClassSubject(CsId, Cs).IsCompletedSuccessfully;
        }

        public async Task<Boolean> RemoveStudent(string CsId, List<string> lst)
        {
            var Cs = await this.GetClassSubject(CsId);
            if (Cs == null)
            {
                return false;
            }
            if (Cs.lstStudentID == null)
            {
                Cs.lstStudentID = new List<string>();
            }
            foreach (var item in lst)
            {
                Cs.lstStudentID.Remove(item);
            }
            var rs = await this.UpdateClassSubject(CsId, Cs);
            return this.UpdateClassSubject(CsId, Cs).IsCompletedSuccessfully;
        }

        public async Task<Boolean> RemoveStudent(string CsId, string sId)
        {
            var Cs = await this.GetClassSubject(CsId);
            if (Cs == null)
            {
                return false;
            }
            if (Cs.lstStudentID == null)
            {
                Cs.lstStudentID = new List<string>();
            }
            Cs.lstStudentID.Remove(sId);
            var rs = await this.UpdateClassSubject(CsId, Cs);
            return this.UpdateClassSubject(CsId, Cs).IsCompletedSuccessfully;
        }

    }
}
