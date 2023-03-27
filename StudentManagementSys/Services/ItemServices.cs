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
    public class ItemServices
    {
        private readonly StudentManagementSysContext _context;

        public ItemServices(StudentManagementSysContext context)
        {
            _context = context;
        }

        // AutoMapper configuration
        private MapperConfiguration config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<Item, ItemDto>()
        );

        private MapperConfiguration configReversed = new MapperConfiguration(cfg =>
                    cfg.CreateMap<ItemDto, Item>()
        );


        //Methods
        public async Task<Boolean> RegisterItemAsync(ItemDto itemDto) {

            _context.Add(new Mapper(configReversed).Map<Item>(itemDto));
            await _context.SaveChangesAsync();
            return _context.SaveChangesAsync().IsCompletedSuccessfully;
        }

        public async Task<List<ItemDto>> GetAllItems()
        {
            var mapper = new Mapper(config);


            if (_context.Item == null)
            {
                return null;
            }
            List<Item> lsItem = await _context.Item.ToListAsync();
            List<ItemDto> lsItemDto = new List<ItemDto>();

            foreach (Item i in lsItem)
            {
                lsItemDto.Add(mapper.Map<ItemDto>(i));
            }
            return lsItemDto;
        }

        public async Task<ItemDto> GetItem(String id)
        {
            var mapper = new Mapper(config);

            if (id == null || _context.Item == null)
            {
                return null;
            }
            Item item = await _context.Item
                .FirstOrDefaultAsync(m => m.ItemID == id);
            if (item == null)
            {
                return null;
            }
            var rs = mapper.Map<ItemDto>(item);
            return rs;
        }

        public async Task<List<ItemDto>> GetItemByList(List<String> listId)
        {
            var mapper = new Mapper(config);
            
            if (listId == null || _context.Item == null)
            {
                return null;
            }
           
            var rs = await _context.Item.Where(c => listId.Contains(c.ItemID)).ToListAsync();
            var lstDto = mapper.Map<List<ItemDto>>(rs);
            return lstDto;
        }

        public async Task<ItemDto> UpdateItem(string id, ItemDto iDto)
        {
            if (id != iDto.ItemID)
            {
                return null;
            }
            var item = new Mapper(configReversed).Map<Item>(iDto);
            try
            {
                _context.ChangeTracker.Clear();
                _context.Update(item);
                await _context.SaveChangesAsync();
                return iDto;
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
                throw;
            }
        }
        public async Task<Boolean> Delete(string id)
        {
            if (_context.Item == null)
            {
                return false;
            }
            var item = await _context.Item.FindAsync(id);
            if (item != null)
            {
                _context.Item.Remove(item);
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
