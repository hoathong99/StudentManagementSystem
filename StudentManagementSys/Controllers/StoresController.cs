﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Data;
using StudentManagementSys.Model;
using AutoMapper;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using StudentManagementSys.Views.ViewModels;

namespace StudentManagementSys.Controllers
{
    [Authorize]
    public class StoresController : Controller
    {
        private readonly StudentManagementSysContext _context;
        private readonly StoreServices _storeServices;

        public StoresController(StudentManagementSysContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _storeServices = new StoreServices(context, userManager);
        }

        //Automapper Configuration
        private MapperConfiguration config = new MapperConfiguration(cfg =>
             cfg.CreateMap<Store, StoreDto>()
        );

        private MapperConfiguration configReversed = new MapperConfiguration(cfg =>
                    cfg.CreateMap<StoreDto, Store>()
        );

        private MapperConfiguration itemMapConfig = new MapperConfiguration(cfg =>
                    cfg.CreateMap<Item, ItemDto>()
        );
        private MapperConfiguration ToStoreVMConfig = new MapperConfiguration(cfg =>
             cfg.CreateMap<StoreDto, StoresVM>()
        );
        private MapperConfiguration FromStoreVMConfig = new MapperConfiguration(cfg =>
             cfg.CreateMap<StoresVM, StoreDto>()
        );

        private async Task<ActionResult<List<ItemDto>>> mapStringToList(String? a)
        {
            var mapper = new Mapper(itemMapConfig);

            List<String> itemsId = String.IsNullOrEmpty(a) ? new List<String>() : a.Split(",").ToList();
            List<ItemDto> items = new List<ItemDto>();
            foreach (String s in itemsId)
            {
                var item = await _context.Item.FindAsync(s);
                if (item != null)
                {
                    items.Add(mapper.Map<ItemDto>(item));
                }
            }
            return items;
        }

        // GET: Stores
        public async Task<IActionResult> Index()
        {
            
            var rs = await _storeServices.GetAllStores();
            if (rs == null)
            {
                return NotFound();
            }
            return View(rs);
        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(string id)
        {
            
            var rs = await _storeServices.GetStore(id);
            var vm = new Mapper(ToStoreVMConfig).Map<StoresVM>(rs);

            if (rs == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        public async Task<IActionResult> CurrentStoreDetail()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // will give the user's Email
            var currentStore = await _storeServices.GetStoreByAccount(userId);
            if(currentStore == null)
            {
                return RedirectToAction(nameof(CreateStore));
            }
            else
            {
                var vm = new Mapper(ToStoreVMConfig).Map<StoresVM>(currentStore);
                return View(vm);
            }
        }

        // GET: Stores/Create
        public async Task<IActionResult> CreateItem(string classId)
        {

            var rs = await _storeServices.GetStore(classId);
            var vm = new Mapper(ToStoreVMConfig).Map<StoresVM>(rs);

            if (rs == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        // GET: Stores/Create
        public IActionResult CreateStore()
        {
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStore([Bind("SID,OwnerID,Status,description,Type")] StoreDto storeDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var rs = await _storeServices.RegisterStoreAsync(storeDto, userId);
            if (!rs)
            {
                return Problem("cant create item.");
            }
            if (userRole == "student")
            {
                return RedirectToAction(nameof(CurrentStoreDetail));
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
           
            var store = await _storeServices.GetStore(id);
            if( store == null)
            {
                return NotFound();
            }
            var vm = new Mapper(ToStoreVMConfig).Map<StoresVM>(store);
            vm.SelectStoreTypeLst = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Staff", Value = "staff" },
                    new SelectListItem { Text = "Student", Value = "student" },
                };
            return View(vm);
        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> EditWithMoreOption(string id)
        {

            var store = await _storeServices.GetStore(id);
            if (store == null)
            {
                return NotFound();
            }
            var vm = new Mapper(ToStoreVMConfig).Map<StoresVM>(store);
            vm.SelectStoreTypeLst = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Staff", Value = "staff" },
                    new SelectListItem { Text = "Student", Value = "student" },
                };
            vm.AllowSetStoreOwnerId = true;
            return View(vm);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SID,OwnerID,Status,description,Type")] StoreDto storeDto)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var rs = await _storeServices.UpdateStore(id, storeDto);
            if(rs == null)
            {
                return Problem("Cant edit store!");
            }
            if (userRole == "student")
            {
                return RedirectToAction(nameof(CurrentStoreDetail));
            }
            return RedirectToAction(nameof(Index));
           
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
       
            var store = await _storeServices.GetStore(id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            
            var rs = await _storeServices.Delete(id);
            if (!rs)
            {
                return Problem("Cant Delete store!");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteItemFromTable(string Iid, string SId)
        {

            var store = await _storeServices.GetStore(SId);
            if (store == null)
            {
                return NotFound();
            }
            //var vm = new Mapper(ToStoreVMConfig).Map<StoresVM>(store);
            await _storeServices.RemoveItemFromStore(Iid, SId);
            return RedirectToAction(nameof(Edit), new { id = SId });
        }

        private bool StoreExists(string id)
        {
            return (_context.Store?.Any(e => e.SID == id)).GetValueOrDefault();
        }
    }
}
