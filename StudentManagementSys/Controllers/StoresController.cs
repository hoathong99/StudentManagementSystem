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

namespace StudentManagementSys.Controllers
{
    [Authorize]
    public class StoresController : Controller
    {
        private readonly StudentManagementSysContext _context;
        private readonly StoreServices _storeServices;

        public StoresController(StudentManagementSysContext context)
        {
            _context = context;
            _storeServices = new StoreServices(context);
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
            //if(_context.Store != null)
            //{
            //    var mapper = new Mapper(config);
            //    List<StoreDto> rs = new List<StoreDto>();
            //    List<Store> lsStore = await _context.Store.ToListAsync();
            //    foreach(Store s in lsStore)
            //    {
            //        rs.Add(mapper.Map<StoreDto>(s));
            //    }
            //    return View(rs);
            //}
            //else
            //{
            //    return Problem("Entity set 'StuManSysContext.Store'  is null.");
            //}
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
            //if (id == null || _context.Store == null)
            //{
            //    return NotFound();
            //}

            //var store = await _context.Store
            //    .FirstOrDefaultAsync(m => m.SID == id);
            //if (store == null)
            //{
            //    return NotFound();
            //}

            //var mapper = new Mapper(config);
            //StoreDto rs = mapper.Map<StoreDto>(store);
            //rs.Items = mapStringToList(store.Items).Result != null ? mapStringToList(store.Items).Result.Value : new List<ItemDto>() ;

            var rs = await _storeServices.GetStore(id);
            if (rs == null)
            {
                return NotFound();
            }
            return View(rs);
        }

        // GET: Stores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SID,OwnerID,Status,description,Type")] StoreDto storeDto)
        {
            //var mapper = new Mapper(configReversed);

            //_context.Add(mapper.Map<Store>(storeDto));
            //    await _context.SaveChangesAsync();
            var rs = await _storeServices.RegisterStoreAsync(storeDto);
            if (!rs)
            {
                return Problem("cant create item.");
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            //if (id == null || _context.Store == null)
            //{
            //    return NotFound();
            //}
            //var mapper = new Mapper(config);
            //var store = mapper.Map<StoreDto>(await _context.Store.FindAsync(id));
            //if (store == null)
            //{
            //    return NotFound();
            //}
            var store = await _storeServices.GetStore(id);
            if( store == null)
            {
                return NotFound();
            }
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SID,OwnerID,Status,description,Type")] StoreDto storeDto)
        {
            //if (id != storeDto.SID)
            //{
            //    return NotFound();
            //}
            //var mapper = new Mapper(configReversed);
            ////if (ModelState.IsValid)
            ////{
            //var obj = mapper.Map<Store>(storeDto);
            //obj.Items = _context.Store.FindAsync(storeDto.SID).Result == null ? "" : _context.Store.FindAsync(storeDto.SID).Result.Items;
            //_context.ChangeTracker.Clear();

            ////var model = _context.Entry(obj);
            ////// state is now Modified. This supercedes the AsNoTracking()

            ////model.State = EntityState.Modified;

            //_context.Entry(obj).State = EntityState.Modified;
            //try
            //{
            //    //_context.Update(obj);
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!StoreExists(storeDto.SID))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}
            var rs = await _storeServices.UpdateStore(id, storeDto);
            if(rs == null)
            {
                return Problem("Cant edit store!");
            }
            return RedirectToAction(nameof(Index));
            //}
            //return View(store);
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            //if (id == null || _context.Store == null)
            //{
            //    return NotFound();
            //}

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
            //if (_context.Store == null)
            //{
            //    return Problem("Entity set 'StuManSysContext.Store'  is null.");
            //}
            //var store = await _context.Store.FindAsync(id);
            //if (store != null)
            //{
            //    _context.Store.Remove(store);
            //}

            //await _context.SaveChangesAsync();
            var rs = await _storeServices.Delete(id);
            if (!rs)
            {
                return Problem("Cant Delete store!");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(string id)
        {
            return (_context.Store?.Any(e => e.SID == id)).GetValueOrDefault();
        }
    }
}
