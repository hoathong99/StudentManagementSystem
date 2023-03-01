using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Model;
using StudentManagementSys.Data;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;

namespace StudentManagementSys.Services
{
    public class StaffServices
    {
        private readonly StudentManagementSysContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public StaffServices(StudentManagementSysContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        //Methods
        public async Task<Boolean> RegisterStaffAsync(StaffDto stDto) {

            _context.Add(new Mapper(configReversed).Map<Staff>(stDto));
            await _context.SaveChangesAsync();
            return _context.SaveChangesAsync().IsCompletedSuccessfully;
        }

        public async Task<List<StaffDto>> GetAllStaffs()
        {
            var mapper = new Mapper(config);


            if (_context.Staff == null)
            {
                return null;
            }
            List<Staff> lsStu = await _context.Staff.ToListAsync();
            List<StaffDto> lsStuDto = new List<StaffDto>();

            foreach (Staff s in lsStu)
            {
                lsStuDto.Add(mapper.Map<StaffDto>(s));
            }

            return lsStuDto;
        }

        public async Task<StaffDto> GetStaff(String id)
        {
            var mapper = new Mapper(config);

            if (id == null || _context.Staff == null)
            {
                return null;
            }
            Staff staff = await _context.Staff
                .FirstOrDefaultAsync(m => m.UID == id);
            if (staff == null)
            {
                return null;
            }
            var rs = mapper.Map<StaffDto>(staff);
            return rs;
        }
        public async Task<StaffDto> GetStaffByAccount(String id)
        {
            var mapper = new Mapper(config);

            if (id == null || _context.Staff == null)
            {
                return null;
            }
            Staff staff = await _context.Staff
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (staff == null)
            {
                return null;
            }
            var rs = mapper.Map<StaffDto>(staff);
            return rs;
        }

        public async Task<StaffDto> UpdateStaff(string id, StaffDto stDto)
        {
            if (id != stDto.UID)
            {
                return null;
            }
            var oG = await GetStaff(id);
            stDto.LstClassRoom = oG.LstClassRoom;
            stDto.LstClassSubject = oG.LstClassSubject;
            stDto.AccountId = oG.AccountId;
            stDto.UID = oG.UID;
            stDto.Authority = oG.Authority;

            var staff = new Mapper(configReversed).Map<Staff>(stDto);
            
            try
            {
                _context.ChangeTracker.Clear();
                _context.Update(staff);
                await _context.SaveChangesAsync();
                return stDto;
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
                throw;
            }
        }
        public async Task<Boolean> Delete(string id)
        {
            if (_context.Staff == null)
            {
                return false;
            }
            var staff = await _context.Staff.FindAsync(id);
            if (staff != null)
            {
                _context.Staff.Remove(staff);
            }
            var user = _userManager.Users.FirstOrDefault(x => x.Id == staff.AccountId);
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public Boolean IsExist(string id)
        {
            return (_context.Staff?.Any(e => e.UID == id)).GetValueOrDefault();
        }
    }
}
