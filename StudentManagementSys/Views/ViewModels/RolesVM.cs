using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using StudentManagementSys.Controllers.Dto;

namespace StudentManagementSys.Views.ViewModels
{
    public class RolesVM
    {
        public IEnumerable<SelectListItem> RoleList { get; set; }
        public string UID { get; set; }
        public string accountId { get; set; }
        public string Name { get; set; }
        [Required]
        public string Authority { get; set; }
        public string? Status { get; set; }
        public string Type { get; set; }
    }

}
