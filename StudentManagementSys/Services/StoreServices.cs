using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Model;
using StudentManagementSys.Data;
using System.Runtime.CompilerServices;

namespace StudentManagementSys.Services
{
    public class StoreServices
    {
        private readonly StudentManagementSysContext _context;

        public StoreServices(StudentManagementSysContext context)
        {
            _context = context;
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


        //Methods
        public async Task<Boolean> RegisterStoreAsync(StoreDto storeDto) {
            var mapper = new Mapper(configReversed);
            _context.Add(mapper.Map<Store>(storeDto));
            await _context.SaveChangesAsync();
            return _context.SaveChangesAsync().IsCompletedSuccessfully;
        }

        public async Task<List<StoreDto>> GetAllStores()
        {
            if (_context.Store != null)
            {
                var mapper = new Mapper(config);
                List<StoreDto> rs = new List<StoreDto>();
                List<Store> lsStore = await _context.Store.ToListAsync();
                foreach (Store s in lsStore)
                {
                    rs.Add(mapper.Map<StoreDto>(s));
                }
                return rs;
            }
            else
            {
                return null;
            }
        }

        public async Task<StoreDto> GetStore(String id)
        {
            if (id == null || _context.Store == null)
            {
                return null;
            }
            var store = await _context.Store
                .FirstOrDefaultAsync(m => m.SID == id);
            if (store == null)
            {
                return null;
            }
            var mapper = new Mapper(config);
            StoreDto rs = mapper.Map<StoreDto>(store);
            rs.Items = mapStringToList(store.Items).Result != null ? mapStringToList(store.Items).Result.Value : new List<ItemDto>();
            return rs;
        }

        public async Task<StoreDto> UpdateStore(string id, StoreDto storeDto)
        {
            if (id != storeDto.SID)
            {
                return null;
            }
            var mapper = new Mapper(configReversed);
            var obj = mapper.Map<Store>(storeDto);
            obj.Items = _context.Store.FindAsync(storeDto.SID).Result == null ? "" : _context.Store.FindAsync(storeDto.SID).Result.Items;
            _context.ChangeTracker.Clear();
            _context.Entry(obj).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(storeDto.SID))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
            return storeDto;
        }
        public async Task<Boolean> Delete(string id)
        {
            if (_context.Store == null)
            {
                return false;
            }
            var store = await _context.Store.FindAsync(id);
            if (store != null)
            {
                _context.Store.Remove(store);
            }

            await _context.SaveChangesAsync();
            return _context.SaveChangesAsync().IsCompletedSuccessfully;
        }

        private bool StoreExists(string id)
        {
            return (_context.Store?.Any(e => e.SID == id)).GetValueOrDefault();
        }

    }
}
