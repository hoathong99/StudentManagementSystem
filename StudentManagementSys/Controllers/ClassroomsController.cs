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
using StudentManagementSys.Views.Classrooms;

namespace StudentManagementSys.Controllers
{
    [Authorize]
    public class ClassroomsController : Controller
    {
        private readonly StudentManagementSysContext _context;
        private readonly ClassroomServices _classroomServices;
        private readonly StudentServices _studentServices;
        private readonly StaffServices _staffServices;

        public ClassroomsController(StudentManagementSysContext context, UserManager<IdentityUser> _userManager)
        {
            _context = context;
            _classroomServices = new ClassroomServices(context);
            _staffServices = new StaffServices(context, _userManager);
            _studentServices = new StudentServices(context, _userManager);
        }

        //Automapper configuration
        private MapperConfiguration config = new MapperConfiguration(cfg =>
            cfg.CreateMap<ClassroomDto, ClassroomVM>()
        );

        private MapperConfiguration configReversed = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ClassroomVM, ClassroomDto>()
        );

        // GET: Classrooms
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Index()
        {
            var rs = await _classroomServices.GetAllClassroom();
            if (rs == null)
            {
                return Problem("Entity set 'StudentManagementSysContext.Classroom'  is null.");
            }
            return View(rs);
        }

        // GET: Classrooms/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var classroom = await _classroomServices.GetClassroom(id);
            var vm = new Mapper(config).Map<ClassroomVM>(classroom);
            var staffs = _staffServices.GetAllStaffs().Result;
            List<SelectListItem> avaiableStudent = new List<SelectListItem>();
            List<SelectListItem> avaiableStaff = new List<SelectListItem>();

            if(classroom.StudentsID != null)
            {
                foreach (var i in classroom.StudentsID)
                {
                    var stu = _studentServices.GetStudent(i).Result;
                    avaiableStudent.Add(
                        new SelectListItem
                        {
                            Text = stu.Name,
                            Value = stu.UID
                        }
                    );
                }
            }

            foreach (var i in staffs)
            {
                avaiableStaff.Add(
                    new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.UID
                    }
                );
            }
            vm.StaffList = avaiableStaff;

            if (classroom == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [Authorize(Roles = "staff")]
        public async Task<IActionResult> StudentList(string id)
        {
            List<StudentDto> lsStuDto = await _studentServices.GetAllStudents();
            AddStudentToClassVM vm = new AddStudentToClassVM();
            vm.students = lsStuDto;
            vm.cid = id;

            if (lsStuDto == null)
            {
                return Problem("Entity set 'StuManSysContext.Student'  is null.");
            }
            else
            {
                return View(vm);
            }
        }

        // GET: Classrooms/Create
        [Authorize(Roles = "staff")]
        public IActionResult AddToClass(string sid, string cid)
        {
            System.Console.WriteLine("sid: "+sid +" cid: "+cid+"/n");
            return NoContent();
        }

        // GET: Classrooms/Create
        [Authorize(Roles = "staff")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Classrooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Create([Bind("CRID,StudentsID,HomeRoomTeacherID,MonitorID")] ClassroomDto classroom)
        {
            var rs = await _classroomServices.RegisterClassroomAsync(classroom);
            if (!rs)
            {
                return Problem("Cant create classroom!");
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Classrooms/Edit/5
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Edit(string id)
        {
            var classroom = await _classroomServices.GetClassroom(id);
            if (classroom == null)
            {
                return NotFound();
            }
            return View(classroom);
        }

        // POST: Classrooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Edit(string id, [Bind("CRID,StudentsID,HomeRoomTeacherID,MonitorID")] ClassroomDto classroom)
        {
            var rs = await _classroomServices.UpdateClassroom(id, classroom);
            if (rs == null)
            {
                return Problem("Cant edit classroom!");
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: Classrooms/Delete/5
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> Delete(string id)
        {
            var classroom = await _classroomServices.GetClassroom(id);
            if (classroom == null)
            {
                return NotFound();
            }

            return View(classroom);
        }

        // POST: Classrooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var rs = await _classroomServices.Delete(id);
            if (!rs)
            {
                return Problem("Cant delete classroom");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ClassroomExists(string id)
        {
            return (_context.Classroom?.Any(e => e.CRID == id)).GetValueOrDefault();
        }
    }
}
