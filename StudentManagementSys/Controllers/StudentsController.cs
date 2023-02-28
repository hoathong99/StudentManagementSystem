using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Data;
using StudentManagementSys.Model;
using StudentManagementSys.Services;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using AutoMapper;
using StudentManagementSys.Controllers.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;
using StudentManagementSys.Views.Classrooms;

namespace StudentManagementSys.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly StudentManagementSysContext _context;
        private readonly StudentServices _StuService;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentsController(StudentManagementSysContext context , UserManager<IdentityUser> userManager)
        {
            _context = context;
            _StuService = new StudentServices(context,userManager);
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

        // GET: Students
        [Authorize]
        public async Task<IActionResult> Index()
        {
            List<StudentDto> lsStuDto = await _StuService.GetAllStudents();
            if (lsStuDto == null)
            {
                return Problem("Entity set 'StuManSysContext.Student'  is null.");
            }
            else
            {
                return View(lsStuDto);
            }
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var student = await _StuService.GetStudent(id);
            if(student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        public async Task<IActionResult> CurrentStudentDetail()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // will give the user's Email
            var currentStudent = await _StuService.GetStudentByAccount(userId);
            return View(currentStudent);
        }

        // GET: Students/Create
        [Authorize(Roles = "staff")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Create([Bind("SchoolSession,AccountId,CPA,TotalCredit,PassedCredit,ClassRoomID,Program,SubjectEnlisted,UID,Name,Status,BirtDate,Type,PhoneNumber,Email,Sex,Address,Relative,YearofStart,Religion,Authority,BCKey,StoreID")] StudentDto studentDto)
        {
            var rs = await _StuService.RegisterStudentAsync(studentDto);
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var student = await _StuService.GetStudent(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SchoolSession,AccountId,CPA,TotalCredit,PassedCredit,ClassRoomID,Program,SubjectEnlisted,UID,Name,Status,BirtDate,Type,PhoneNumber,Email,Sex,Address,Relative,YearofStart,Religion,Authority,BCKey,StoreID")] StudentDto student)
        {
            if (id != student.UID)
            {
                return NotFound();
            }
            var rs = await _StuService.UpdateStudent(id, student);
            if (rs == null)
            {
                return Problem("saving context problem!");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Delete(string id)
        {
            var student = await _StuService.GetStudent(id);
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var student = await _StuService.GetStudent(id);
            if (student == null)
            {
                return Problem("cant find item!");
            }
            var rs = await _StuService.Delete(id);
            if (!rs)
            {
                return Problem("cant delete item!");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(string id)
        {
            return (_context.Student?.Any(e => e.UID == id)).GetValueOrDefault();
        }
    }
}
