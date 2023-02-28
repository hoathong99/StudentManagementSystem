using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using StudentManagementSys.Controllers.Dto;

namespace StudentManagementSys.Views.Roles
{
    public class AssignRoleVM
    {
        public IEnumerable<SelectListItem> RoleList { get; set; }
        public String UID { get; set; }
        public String accountId { get; set; }
        public String Name { get; set; }
        [Required]
        public String Authority { get; set; }
        public String? Status { get; set; }
        public String Type { get; set; }
    }

}
