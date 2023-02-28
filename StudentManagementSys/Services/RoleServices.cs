using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Data;
using StudentManagementSys.Model;

namespace StudentManagementSys.Services
{
    public class RoleServices
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StudentManagementSysContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public RoleServices(RoleManager<IdentityRole> roleManager) { 
            _roleManager = roleManager;
        }

        public RoleServices(RoleManager<IdentityRole> roleManager, StudentManagementSysContext context, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }

        // AutoMapper configuration
        private MapperConfiguration configStaff = new MapperConfiguration(cfg =>
                    cfg.CreateMap<Staff, SimplifiedAccount>()
        );

        private MapperConfiguration configStudent = new MapperConfiguration(cfg =>
                    cfg.CreateMap<Student, SimplifiedAccount>()
        );

        //Methods
        public async Task<IdentityRole> Get(string id)
        {
            var rs = await _roleManager.FindByIdAsync(id);
            if(rs == null)
            {
                return null;
            }
            return rs;
        }

        public async Task<List<IdentityRole>> GetAllRoles()
        {
            var rs = _roleManager.Roles.ToList();
            if (rs == null)
            {
                return null;
            }
            return rs;
        }

        public async Task<List<SimplifiedAccount>> GetAllAccounts ()
        {
            var mapperStaff = new Mapper(configStaff);
            var mapperStudent = new Mapper(configStudent);
            List<Student> lsStu = _context.Student.ToList();
            List<Staff> lsSta = _context.Staff.ToList();
            List<SimplifiedAccount> ls = new List<SimplifiedAccount>();
            foreach (var s1 in lsStu)
            {
                ls.Add(mapperStudent.Map<SimplifiedAccount>(s1));
            }
            foreach (var s2 in lsSta)
            {
                ls.Add(mapperStaff.Map<SimplifiedAccount>(s2));
            }
            return ls;
        }

        public async Task<Boolean> Create(IdentityRole model)
        {
            if (_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                return false;
            }
            var rs = await _roleManager.CreateAsync(new IdentityRole(model.Name));
            return rs.Succeeded;
        }

        public async Task<Boolean> Delete(string id)
        {
            var role = await this.Get(id);
            if (role == null)
            {
                return false;
            }
            var rs = await _roleManager.DeleteAsync(role);
            return rs.Succeeded;
        }

        public async Task<Boolean> Update(IdentityRole model)
        {
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                return false;
            }
            var rs = await _roleManager.UpdateAsync(model);
            return rs.Succeeded;
        }

        public async Task<Boolean> AssignRole(SimplifiedAccount simplifiedAccount) 
        {
            var role = await _roleManager.FindByNameAsync(simplifiedAccount.Authority);
            Student student = await _context.Student
                .FirstOrDefaultAsync(m => m.AccountId == simplifiedAccount.accountId);

            if (student == null)
            {
                Staff staff = await _context.Staff
                .FirstOrDefaultAsync(m => m.AccountId == simplifiedAccount.accountId);
                if (staff == null)
                {
                    return false;
                }
                //Update Model DB
                staff.Authority = role.Name;
                _context.ChangeTracker.Clear();
                _context.Update(staff);
                await _context.SaveChangesAsync();
            }
            else
            {
                //Update Model DB
                student.Authority = role.Name;
                _context.ChangeTracker.Clear();
                _context.Update(student);
                var rs = await _context.SaveChangesAsync();
            }
            
            //Assign authority
            var user = _userManager.Users.FirstOrDefault(m => m.Id == simplifiedAccount.accountId);
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            var rsRole = await _userManager.AddToRoleAsync(user, simplifiedAccount.Authority);
            //var boolean = _context.SaveChangesAsync().IsCompletedSuccessfully && rsRole.Succeeded;
            return true;
        }

    }
    
}
