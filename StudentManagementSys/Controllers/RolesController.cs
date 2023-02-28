using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagementSys.Controllers.Dto;
using StudentManagementSys.Data;
using StudentManagementSys.Services;
using StudentManagementSys.Views.Roles;

namespace StudentManagementSys.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RoleServices _roleServices;
        public RolesController(RoleManager<IdentityRole> roleManager, StudentManagementSysContext context, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _roleServices = new RoleServices(roleManager, context, userManager);
        }

        private MapperConfiguration config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<SimplifiedAccount, AssignRoleVM>()
        );
        private MapperConfiguration configReversed = new MapperConfiguration(cfg =>
                    cfg.CreateMap<AssignRoleVM, SimplifiedAccount>()
        );

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> AccountTable()
        {
            var accounts = await _roleServices.GetAllAccounts();
            return View(accounts);
        }

        [HttpGet]
        public IActionResult create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> create(IdentityRole model) {
            var rs = await _roleServices.Create(model);
            if( rs == false)
            {
                return Problem("cant create role");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var rs = await _roleServices.Get(id);
            return View(rs);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var rs = await _roleServices.Delete(id);
            if (rs == false)
            {
                return Problem("Cant delete role");
            }
            return RedirectToAction("Index");
        }

        [HttpGet, ActionName("SetRole")]
        public async Task<IActionResult> SetRole(string id)
        {
            var avaiableRoles = await _roleServices.GetAllRoles();
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (var i in avaiableRoles)
            {
                selectListItems.Add(
                    new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Name
                    }
                );
            }

            var ls = await _roleServices.GetAllAccounts();
            var account = ls.FirstOrDefault(i => i.accountId == id);

            var rs = new Mapper(config).Map<AssignRoleVM>(account);
            rs.RoleList = selectListItems;
            return View(rs);
        }
        [HttpPost]
        public async Task<IActionResult> SetRole([Bind("UID,accountId,Authority")] AssignRoleVM vm)
        {
            var sA = new Mapper(configReversed).Map<SimplifiedAccount>(vm);
            var rs = await _roleServices.AssignRole(sA);
            if (rs == false)
            {
                return Problem("Cant Assign role");
            }
            return RedirectToAction("AccountTable");
        }
    }
}
