using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSys.Controllers.Dto
{
    public class StoreDto
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public String SID { get; set; }
        [Required]
        public String OwnerID { get; set; }
        [Required]
        public Boolean Status { get; set; }
        public String description { get; set; }
        public String Type { get; set; }
        public List<ItemDto> Items { get; set; }
    }
}

