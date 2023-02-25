using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSys.Controllers.Dto
{
    public class UserDto
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public String UID { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String? Status { get; set; }
        public DateTime? BirtDate { get; set; }
        public String Type { get; set; }
        public String? PhoneNumber { get; set; }
        public String? Email { get; set; }
        public String? Sex { get; set; }
        public String? Address { get; set; }
        public String? Relative { get; set; }
        public DateTime? YearofStart { get; set; }
        public String? Religion { get; set; }
        [Required]
        public String Authority { get; set; }
        public String? BCKey { get; set; }
        public String? StoreID { get; set; }
    }
}


