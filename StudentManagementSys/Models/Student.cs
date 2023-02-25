using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSys.Model
{
    public class Student : User
    {
        [Required]
        public String? SchoolSession { get; set; }
        public Double? CPA { get; set; }
        public int? TotalCredit { get; set; }
        public int? PassedCredit { get; set; }
        public String? ClassRoomID { get; set; }
        public String? Program { get; set; }
        
        // relations
        public String? SubjectEnlisted { get; set; }                     //Subject will be serialized to one string and populate in Dto formatted in ("id"+,"id2"+....)
    }
}
