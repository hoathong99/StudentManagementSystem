using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Data;
using StudentManagementSys.Model;
using StudentManagementSys.Services;
using StudentManagementSys.Views.ViewModels;

namespace StudentManagementSys.Controllers
{
    [Authorize]
    public class ClassSubjectsController : Controller
    {
        private readonly StudentManagementSysContext _context;
        private readonly ClassSubjectServices _classSubjectServices;
        private readonly StudentServices _studentServices;
        private readonly StaffServices _staffServices;
        public ClassSubjectsController(StudentManagementSysContext context, UserManager<IdentityUser> _userManager)
        {
            _context = context;
            _classSubjectServices = new ClassSubjectServices(context, _userManager);
            _studentServices = new StudentServices(context, _userManager);
            _staffServices = new StaffServices(context, _userManager);
        }

        // AutoMapper configuration
        private MapperConfiguration config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ClassSubjectDto, ClassSubjectVM>()
        );

        private MapperConfiguration configReversed = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ClassSubjectVM, ClassSubjectDto>()
        );

        // GET: ClassSubjects
        public async Task<IActionResult> Index()
        {
            var rs = await _classSubjectServices.GetAllClassSubject();
            if(rs == null)
            {
                return NotFound();
            }
            var vm = new Mapper(config).Map<List<ClassSubjectVM>>(rs);
            return View(vm);
        }

        // GET: ClassSubjects/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var classSubject = await _classSubjectServices.GetClassSubject(id);
            if(classSubject == null)
            {
                return NotFound();
            }
            List<StudentVM> stuVmList= new List<StudentVM>();

            foreach(var i in classSubject.lstStudentID)
            {
                var stu = await _studentServices.GetStudent(i);
                StudentVM stuVm = new StudentVM { 
                    id = stu.UID, 
                    name = stu.Name, 
                    status = stu.Status
                };
                stuVmList.Add(stuVm);
            }
            var vm = new Mapper(config).Map<ClassSubjectVM>(classSubject);
            vm.lstStudent = stuVmList;

            return View(vm);
        }

        // GET: ClassSubjects/Create
        [Authorize(Roles = "staff")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClassSubjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Create([Bind("classSubjectId,classSubjectCode,subjectCode,SubjectName,typeClassSubject,lstStudentID,TeacherID,room,schedule,Semester")] ClassSubjectDto classSubject)
        {
            var rs = await _classSubjectServices.RegisterClassSubjectAsync(classSubject);
            if (!rs)
            {
                return Problem("Cant create classSubject!");
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ClassSubjects/Edit/5
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Edit(string id)
        {
            var cS = await _classSubjectServices.GetClassSubject(id);
            if (cS == null)
            {
                return NotFound();
            }

            var mapper = new Mapper(config);
            //ClassSubjectVM classSubjectVM = mapper.Map<ClassSubjectVM>(cS); // WTF why doesnt work?

            ClassSubjectVM classSubjectVM = new ClassSubjectVM
            {
                classSubjectId = cS.classSubjectId,
                classSubjectCode = cS.classSubjectCode,
                subjectCode = cS.subjectCode,
                SubjectName = cS.SubjectName,
                typeClassSubject = cS.typeClassSubject,
                lstStudent = new List<StudentVM>(),
                TeacherID = cS.TeacherID,
                room = cS.room,
                schedule = cS.schedule,
                Semester = cS.Semester,
                lstStudentID = cS.lstStudentID,
                StudentIdsSerialized = ""
            };

            var lsStu = await _studentServices.GetStudentsByClassSubject(id);

            if (lsStu != null)
            {
                foreach (var i in lsStu)
                {
                    classSubjectVM.lstStudent.Add(new StudentVM
                    {
                        id = i.UID,
                        name = i.Name,
                        status = i.Status
                    });
                }
            }
            return View(classSubjectVM);
        }

        // POST: ClassSubjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Edit(string id, [Bind("classSubjectId,classSubjectCode,subjectCode,SubjectName,typeClassSubject,lstStudentID,TeacherID,room,schedule,Semester")] ClassSubjectDto classSubject)
        {
            var StaffCheck = await _staffServices.GetStaff(classSubject.TeacherID);
            if (StaffCheck == null) {
                return NoContent();
            }
            var rs = await _classSubjectServices.UpdateClassSubject(id, classSubject);
            if (rs == null)
            {
                return Problem("Cant edit classSubject!");
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ClassSubjects/Delete/5
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Delete(string id)
        {
            //if (id == null || _context.ClassSubject == null)
            //{
            //    return NotFound();
            //}

            var classSubject = await _classSubjectServices.GetClassSubject(id);
            if (classSubject == null)
            {
                return NotFound();
            }

            return View(classSubject);
        }

        // POST: ClassSubjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var rs = await _classSubjectServices.Delete(id);
            if (!rs)
            {
                return Problem("Cant delete classSubject!");
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "staff")]
        public async Task<IActionResult> RemoveFromClassSubject(string sid, string cid)
        {
            var rs = await _classSubjectServices.RemoveStudent(cid, sid);
            if (!rs)
            {
                return Problem("Cant Remove student From classSubject!");
            }
            return RedirectToAction(nameof(Edit), new {id = cid});
        }

        
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> AddToClassSubjectTable(string CsId)
        {

            AddStudentToClassVM vm = new AddStudentToClassVM();
            vm.cid = CsId;
            vm.students = await _studentServices.GetAllStudents();
            
            return View(vm);
        }

        [Authorize(Roles = "staff")]
        public async Task<IActionResult> AddStudentToClassSubject(string sid, string csid)
        {
            var rs = await _classSubjectServices.AddStudent(csid, sid);
            if (!rs)
            {
                return Problem("Cant add student to classSubject!");
            }
            return RedirectToAction(nameof(Edit), new { id = csid });
        }
        private bool ClassSubjectExists(string id)
        {
          return (_context.ClassSubject?.Any(e => e.classSubjectId == id)).GetValueOrDefault();
        }
    }
}
