using System.ComponentModel.DataAnnotations;

namespace StudentManagementSys.Views.ViewModels
{
    public class ItemsVM
    {
        [Required]
        public String Name { get; set; }
        public String Desc { get; set; }
        public String ItemID { get; set; }
        public int price { get; set; }
        public String SID { get; set; }
    }
}
