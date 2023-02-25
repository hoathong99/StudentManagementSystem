using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Data;
using StudentManagementSys.Model;
using StudentManagementSys.Services;

namespace StudentManagementSys.Controllers
{
    [Authorize]
    public class ClassSubjectsController : Controller
    {
        private readonly StudentManagementSysContext _context;
        private readonly ClassSubjectServices _classSubjectServices;
        public ClassSubjectsController(StudentManagementSysContext context)
        {
            _context = context;
            _classSubjectServices = new ClassSubjectServices(context);
        }

        // GET: ClassSubjects
        public async Task<IActionResult> Index()
        {
            var rs = await _classSubjectServices.GetAllClassSubject();
            if(rs == null)
            {
                return NotFound();
            }
            return View(rs);
        }

        // GET: ClassSubjects/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var classSubject = await _classSubjectServices.GetClassSubject(id);
            if(classSubject == null)
            {
                return NotFound();
            }
            return View(classSubject);
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
            var classSubject = await _classSubjectServices.GetClassSubject(id);
            if (classSubject == null)
            {
                return NotFound();
            }
            return View(classSubject);
        }

        // POST: ClassSubjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Edit(string id, [Bind("classSubjectId,classSubjectCode,subjectCode,SubjectName,typeClassSubject,lstStudentID,TeacherID,room,schedule,Semester")] ClassSubjectDto classSubject)
        {
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

        private bool ClassSubjectExists(string id)
        {
          return (_context.ClassSubject?.Any(e => e.classSubjectId == id)).GetValueOrDefault();
        }
    }
}
