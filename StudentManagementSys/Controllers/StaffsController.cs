using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

namespace StudentManagementSys.Controllers
{
    [Authorize(Roles = "staff")]
    public class StaffsController : Controller
    {
        private readonly StudentManagementSysContext _context;
        private readonly StaffServices _StaService;

        public StaffsController(StudentManagementSysContext context, UserManager<IdentityUser> _userManager)
        {
            _context = context;
            _StaService = new StaffServices(context, _userManager);
        }

        //AutoMapper Configuration
        private MapperConfiguration config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<Staff, StaffDto>()
                    .ForMember(des => des.LstClassRoom, act => act.MapFrom(scr => mapStringToList(scr.LstClassRoom)))
                    .ForMember(des => des.LstClassSubject, act => act.MapFrom(scr => mapStringToList(scr.LstClassSubject)))
        );

        private MapperConfiguration configReversed = new MapperConfiguration(cfg =>
                    cfg.CreateMap<StaffDto, Staff>()
                    .ForMember(des => des.LstClassRoom, act => act.MapFrom(scr => String.Join(',', scr.LstClassRoom.Where(s => !string.IsNullOrEmpty(s)))))
                    .ForMember(des => des.LstClassSubject, act => act.MapFrom(scr => String.Join(',', scr.LstClassSubject.Where(s => !string.IsNullOrEmpty(s)))))
        );

        private static List<String> mapStringToList(String? a)
        {
            return String.IsNullOrEmpty(a) ? new List<String>() : a.Split(",").ToList();
        }


        // GET: Staffs
        public async Task<IActionResult> Index()
        {
            List<StaffDto> lsStaDto = await _StaService.GetAllStaffs();
            if (lsStaDto == null)
            {
                return Problem("Entity set 'StuManSysContext.Staff'  is null.");
            }
            else
            {
                return View(lsStaDto);
            }
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var staff = await _StaService.GetStaff(id);
            if(staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }

        public async Task<IActionResult> CurrentStaffDetail()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // will give the user's Email
            var currentStaff = await _StaService.GetStaffByAccount(userId);
            return View(currentStaff);
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("School,LstClassSubject,LstClassRoom,UID,Name,Status,BirtDate,Type,PhoneNumber,Email,Sex,Address,Relative,YearofStart,Religion,Authority,BCKey,StoreID")] StaffDto staff)
        {
                var rs = await _StaService.RegisterStaffAsync(staff);
                if(rs == false)
                {
                    return Problem("Cant create new staff!");
                }
                return RedirectToAction(nameof(Index));
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            //if (id == null || _context.Staff == null)
            //{
            //    return NotFound();
            //}

            //var staff = await _context.Staff.FindAsync(id);
            //if (staff == null)
            //{
            //    return NotFound();
            //}
            var staff = await _StaService.GetStaff(id);
            if(staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("School,LstClassSubject,LstClassRoom,UID,Name,Status,BirtDate,Type,PhoneNumber,Email,Sex,Address,Relative,YearofStart,Religion,Authority,BCKey,StoreID")] StaffDto staff)
        {
            if (id != staff.UID)
            {
                return NotFound();
            }

            var rs = await _StaService.UpdateStaff(id, staff);
            if (rs == null)
            {
                return Problem("saving context problem!");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Staffs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var staff = await _StaService.GetStaff(id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var rs = await _StaService.Delete(id);
            if(rs == false)
            {
                return Problem("cant delete item!");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(string id)
        {
            return (_context.Staff?.Any(e => e.UID == id)).GetValueOrDefault();
        }
    }
}
