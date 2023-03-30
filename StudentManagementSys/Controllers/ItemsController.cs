using System;
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
using Microsoft.AspNetCore.Identity;
using StudentManagementSys.Views.ViewModels;

namespace StudentManagementSys.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly StudentManagementSysContext _context;
        private readonly ItemServices _IteamService;
        private readonly StoreServices _StoreServices;
        public ItemsController(StudentManagementSysContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _IteamService = new ItemServices(context);
            _StoreServices = new StoreServices(context, userManager);
        }

        // AutoMapper configuration
        private MapperConfiguration config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<Item, ItemDto>()
        );

        private MapperConfiguration configReversed = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ItemDto, Item>()
        );

        private MapperConfiguration DtoToVmConfig = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ItemDto, ItemsVM>()
        );

        private MapperConfiguration VmToDtoConfig = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ItemsVM, ItemDto>()
        );

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var rs = await _IteamService.GetAllItems();
            if (rs == null)
            {
                return Problem("Entity set 'StuManSysContext.Item'  is null.");
            }
            return View(rs);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var item = await _IteamService.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        public IActionResult CreateItem( string? storeId)
        {
            var vm = new ItemsVM { SID = storeId };
            return View(vm);
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem([Bind("Name,Desc,ItemID,price,SID")] ItemsVM vm)
        {
            var dto = new Mapper(VmToDtoConfig).Map<ItemDto>(vm);
            var rs = await _IteamService.RegisterItemAsync(dto);
            var rs1 = await _StoreServices.addItemToStore(rs.ItemID, vm.SID);
            if (rs == null)
            {
                return Problem("Cant create item.");
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var item = await _IteamService.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Desc,ItemID,price")] ItemDto itemDto)
        {
            if (id != itemDto.ItemID)
            {
                return NotFound();
            }
            var rs = await _IteamService.UpdateItem(id, itemDto);
            if (rs == null)
            {
                return Problem("Cant edit item.");
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> DeleteItem(string id)
        {
            var item = await _IteamService.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string ItemID)
        {
            var rs = await _IteamService.Delete(ItemID);
            if (!rs)
            {
                return Problem("Cant delete item.");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(string id)
        {
            return (_context.Item?.Any(e => e.ItemID == id)).GetValueOrDefault();
        }
    }
}
