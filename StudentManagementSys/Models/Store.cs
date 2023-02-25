using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSys.Model
{
    public class Store
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public String SID { get; set; }
        [Required]
        public String OwnerID { get; set; }
        [Required]
        public Boolean Status { get; set; }
        public String description { get; set; }
        public String Type { get; set; }
        public String? Items { get; set; }
    }
}

