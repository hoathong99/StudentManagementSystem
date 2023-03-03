using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Model;
using StudentManagementSys.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace StudentManagementSys.Services
{
    public class ClassSubjectServices
    {
        private readonly StudentManagementSysContext _context;
        private readonly StudentServices _studentService;

        public ClassSubjectServices(StudentManagementSysContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _studentService = new StudentServices(context, userManager);
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
            if(stDto.subjectCode == null)
            {
                stDto.subjectCode = stDto.classSubjectCode;
            }
            if (stDto.classSubjectCode == null)
            {
                stDto.classSubjectCode = stDto.subjectCode;
            }
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

        public async Task<List<ClassSubjectDto>> GetClassSubjectsByStudent(string sId)
        {
            var mapper = new Mapper(config);

            if (_context.ClassSubject == null)
            {
                return null;
            }
            List<ClassSubject> lsCs = await _context.ClassSubject.Where(x => x.lstStudentID.Contains(sId)).ToListAsync();
            List<ClassSubjectDto> lsStuDto = new List<ClassSubjectDto>();

            foreach (ClassSubject s in lsCs)
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
            rs.lstStudentID = rs.lstStudentID == null ? new List<String>() : rs.lstStudentID;
            return rs;
        }

        public async Task<ClassSubjectDto> UpdateClassSubject(string id, ClassSubjectDto stuDto)
        {
            var oG = await GetClassSubject(id);
            stuDto.classSubjectCode = stuDto.subjectCode;
            stuDto.classSubjectId = oG.classSubjectId;
            //stuDto.lstStudentID = oG.lstStudentID;
        
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
            var Stu = await _studentService.GetStudent(sId);
            if (Cs == null)
            {
                return false;
            }
            if (Stu == null)
            {
                return false;
            }
            if (Cs.lstStudentID == null)
            {
                Cs.lstStudentID = new List<string>();
            }
            Cs.lstStudentID.Add(sId);

            if (Stu.SubjectEnlisted == null)
            {
                Stu.SubjectEnlisted = new List<string>();
            }

            if (!Stu.SubjectEnlisted.Contains(CsId))
            {
                Stu.SubjectEnlisted.Add(CsId);
            }
            
            var rs = await this.UpdateClassSubject(CsId, Cs);
            var rs2 = await _studentService.UpdateStudent(sId, Stu);
            bool flag = (rs == null || rs2 == null) ? false : true;

            return flag;
        }

        //public async Task<Boolean> AddStudent(string CsId, List<string> lst)
        //{
        //    var Cs = await this.GetClassSubject(CsId);
        //    if (Cs == null)
        //    {
        //        return false;
        //    }
        //    if (Cs.lstStudentID == null)
        //    {
        //        Cs.lstStudentID = new List<string>();
        //    }
        //    Cs.lstStudentID.AddRange(lst);
        //    var rs = await this.UpdateClassSubject(CsId, Cs);
        //    return this.UpdateClassSubject(CsId, Cs).IsCompletedSuccessfully;
        //}

        //public async Task<Boolean> RemoveStudent(string CsId, List<string> lst)
        //{
        //    var Cs = await this.GetClassSubject(CsId);
        //    if (Cs == null)
        //    {
        //        return false;
        //    }
        //    if (Cs.lstStudentID == null)
        //    {
        //        Cs.lstStudentID = new List<string>();
        //    }
        //    foreach (var item in lst)
        //    {
        //        Cs.lstStudentID.Remove(item);
        //    }
        //    var rs = await this.UpdateClassSubject(CsId, Cs);
        //    bool flag = rs == null ? false : true;

        //    return flag;
        //}

        public async Task<Boolean> RemoveStudent(string CsId, string sId)
        {
            var Cs = await this.GetClassSubject(CsId);
            var Stu = await _studentService.GetStudent(sId);
            if (Cs == null)
            {
                return false;
            }
            if (Stu == null)
            {
                return false;
            }
            if (Cs.lstStudentID == null)
            {
                Cs.lstStudentID = new List<string>();
            }
            Cs.lstStudentID.Remove(sId);
            if (Stu.SubjectEnlisted == null)
            {
                Stu.SubjectEnlisted = new List<string>();
            }
            Stu.SubjectEnlisted.Remove(CsId);

            var rs = await _studentService.UpdateStudent(sId, Stu);
            var rs2 = await this.UpdateClassSubject(CsId, Cs);

            bool flag = (rs == null || rs2 ==null) ? false : true;

            return flag;
        }

    }
}
