using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Model;
using StudentManagementSys.Data;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;

namespace StudentManagementSys.Services
{
    public class StoreServices
    {
        private readonly StudentManagementSysContext _context;
        private readonly StudentServices _studentServices;
        private readonly StaffServices _staffServices;
        public StoreServices(StudentManagementSysContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _studentServices = new StudentServices(context, userManager);
            _staffServices = new StaffServices(context, userManager);
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


        //Methods                                                                                       // register store, update owner information of said store
        public async Task<Boolean> RegisterStoreAsync(StoreDto storeDto, String aId) {                  // return true if success / false if not
            var Stu = await _studentServices.GetStudentByAccount(aId);
            var Staf = await _staffServices.GetStaffByAccount(aId);
            var mapper = new Mapper(configReversed);
            
            if (Stu != null)
            {
                storeDto.OwnerID = Stu.UID;
                var savingStore = _context.Add(mapper.Map<Store>(storeDto));
                await _context.SaveChangesAsync();
                Stu.StoreID = savingStore.Entity.SID;
                await _studentServices.UpdateStudent(Stu.UID, Stu);
                return true;
            }
            if(Staf != null)
            {
                storeDto.OwnerID = Staf.UID;
                var savingStore = _context.Add(mapper.Map<Store>(storeDto));
                await _context.SaveChangesAsync();
                Staf.StoreID = savingStore.Entity.SID;
                await _staffServices.UpdateStaff(Staf.UID, Staf);
                return true;
            }
            return false;
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

        public async Task<StoreDto> GetStoreByAccount(String aid)
        {
            String id = "";
            var Stu = await _studentServices.GetStudentByAccount(aid);
            if (Stu == null)
            {
                var Staf = await _staffServices.GetStaffByAccount(aid);
                if(Staf == null)
                {
                    return null;
                }
                else
                {
                    id = Staf.StoreID;
                }
            }
            else
            {
                id = Stu.StoreID;
            }

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
