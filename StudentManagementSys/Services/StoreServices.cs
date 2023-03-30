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
        private readonly ItemServices _itemServices;
        public StoreServices(StudentManagementSysContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _studentServices = new StudentServices(context, userManager);
            _staffServices = new StaffServices(context, userManager);
            _itemServices = new ItemServices(context);
        }

        //Automapper Configuration
        private MapperConfiguration storeToDtoConfig = new MapperConfiguration(cfg =>
             cfg.CreateMap<Store, StoreDto>()
            .ForMember(x => x.Items, opt => opt.Ignore())
        );

        private MapperConfiguration dtoToStoreConfig = new MapperConfiguration(cfg =>
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
                var item = await _itemServices.GetItem(s);
                if (item != null)
                {
                    items.Add(mapper.Map<ItemDto>(item));
                }
                else
                {
                    var temp = new ItemDto { 
                        ItemID = s,
                        Desc = "not avaiable", 
                        Name = "not avaible",
                        price = 0 }
                    ;
                    items.Add(temp);
                }
            }
            return items;
        }

        private string mapListToString(List<ItemDto> items)
        {
            var rs = "";
            List<String> listString = new List<string>();
            foreach (var i in items)
            {
                listString.Add(i.ItemID);
            }
            rs = String.Join(",", listString);
            return rs;
        }


        //Methods                                                                                       // register store, update owner information of said store
        public async Task<Boolean> RegisterStoreAsync(StoreDto storeDto, String aId)
        {                  // return true if success / false if not
            var Stu = await _studentServices.GetStudentByAccount(aId);
            var Staf = await _staffServices.GetStaffByAccount(aId);
            var mapper = new Mapper(dtoToStoreConfig);

            if (Stu != null)
            {
                storeDto.OwnerID = Stu.UID;
                var savingStore = _context.Add(mapper.Map<Store>(storeDto));
                await _context.SaveChangesAsync();
                Stu.StoreID = savingStore.Entity.SID;
                await _studentServices.UpdateStudent(Stu.UID, Stu);
                return true;
            }
            if (Staf != null)
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
                List<StoreDto> rs = new List<StoreDto>();
                List<Store> lsStore = await _context.Store.ToListAsync();
                //foreach (Store s in lsStore)
                //{
                //    var dto = mapper.Map<StoreDto>(s);
                //    rs.Add(dto);
                //}
                //return rs;
                rs = new Mapper(storeToDtoConfig).Map<List<StoreDto>>(lsStore);
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
            var mapper = new Mapper(storeToDtoConfig);
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
                if (Staf == null)
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
            var mapper = new Mapper(storeToDtoConfig);
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
            var mapper = new Mapper(dtoToStoreConfig);
            var obj = mapper.Map<Store>(storeDto);
            obj.Items = _context.Store.FindAsync(storeDto.SID).Result == null ? "" : mapListToString(storeDto.Items);
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

        public async Task<Boolean> addItemToStore(string iId, string sId)
        {
            var store = await GetStore(sId);
            var item = await _itemServices.GetItem(iId);
            if (item == null || !StoreExists(sId))
            {
                return false;
            }
            if (store.Items == null)
            {
                store.Items = new List<ItemDto>();
            }
            store.Items.Add(item);
            var rs = await UpdateStore(sId, store);
            if (rs == null)
            {
                return false;
            }
            return true;
        }

        // only remove item from store
        public async Task<Boolean> RemoveItemFromStore(string iId, string sId)
        {
            var store = await GetStore(sId);
            var item = await _itemServices.GetItem(iId);
            //if (item == null || !StoreExists(sId))
            //{
            //    return false;
            //}
            //if (store.Items == null)
            //{
            //    return true;

            //}

            //if (store.Items.Contains(item))
            //{
            //    store.Items.Remove(item);
            //    var rs = await UpdateStore(sId, store);
            //    if (rs == null)
            //    {
            //        return false;
            //    }
            //    return true;
            //}
            for(int i = 0; i< store.Items.Count; i++)
            {
                if(store.Items.ElementAt(i).ItemID == iId)
                {
                    store.Items.RemoveAt(i);
                }
            }
            var rs = await UpdateStore(sId, store);
            await _itemServices.Delete(iId);
            return true;
        }

        private bool StoreExists(string id)
        {
            return (_context.Store?.Any(e => e.SID == id)).GetValueOrDefault();
        }

    }
}
