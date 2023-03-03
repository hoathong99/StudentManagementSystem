using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> IndexForStudent()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentStudent = await _studentServices.GetStudentByAccount(userId);

            var rs = await _classSubjectServices.GetClassSubjectsByStudent(currentStudent.UID);
            if (rs == null)
            {
                return NotFound();
            }
            var vm = new Mapper(config).Map<List<ClassSubjectVM>>(rs);
            return View(vm);
        }

        // GET: ClassSubjects/Details/5
        [Authorize]
        public async Task<IActionResult> Details(string id)
        {
            var classSubject = await _classSubjectServices.GetClassSubject(id);
            if(classSubject == null)
            {
                return NotFound();
            }
            //List<StudentVM> stuVmList= new List<StudentVM>();

            
            var vm = new Mapper(config).Map<ClassSubjectVM>(classSubject);
            vm.lstStudent = new List<StudentVM>();
            var lsStu = await _studentServices.GetStudentsByClassSubject(id);
            if (lsStu != null)
            {
                foreach (var i in lsStu)
                {
                    vm.lstStudent.Add(new StudentVM
                    {
                        id = i.UID,
                        name = i.Name,
                        status = i.Status
                    });
                }
            }
            //vm.lstStudent = stuVmList;

            return View(vm);
        }

        [Authorize(Roles = "student")]
        public async Task<IActionResult> EnlistClass(string csId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentStudent = await _studentServices.GetStudentByAccount(userId);

            var rs = await _classSubjectServices.AddStudent(csId, currentStudent.UID);
            if (!rs)
            {
                return Problem("Cant add student to classSubject!");
            }
            return RedirectToAction(nameof(IndexForStudent));
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
            var teacher = await _staffServices.GetStaff(classSubject.TeacherID);
            if(teacher== null) {
                return NoContent(); 
            }
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
            var oG = await _classSubjectServices.GetClassSubject(id);                               // form does not provide lstStudentID
            classSubject.lstStudentID = oG.lstStudentID;                                            // lstStudentID will only updated by service: AddStudent

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

        [Authorize(Roles = "student")]
        public async Task<IActionResult> RemoveFromClassSubjectForStudent(string csId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentStudent = await _studentServices.GetStudentByAccount(userId);

            var rs = await _classSubjectServices.RemoveStudent(csId, currentStudent.UID);
            if (!rs)
            {
                return Problem("Cant Remove student From classSubject!");
            }
            return RedirectToAction(nameof(IndexForStudent));
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
