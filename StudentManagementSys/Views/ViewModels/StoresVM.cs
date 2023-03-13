using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagementSys.Controllers.Dto;


namespace StudentManagementSys.Views.ViewModels
{
    public class StoresVM
    {
        public String SID { get; set; }
        public String OwnerID { get; set; }
        public Boolean Status { get; set; }
        public String description { get; set; }
        public String Type { get; set; }
        public List<ItemDto> Items { get; set; }
        public IEnumerable<SelectListItem> SelectStoreTypeLst { get; set; }
        public Boolean AllowSetStoreOwnerId { get; set; }
        public StoresVM() {
            AllowSetStoreOwnerId = false;
        }
    }
}
