using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StudentManagementSys.Model
{
    public class Classroom
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public String CRID { get; set; }
        public String? StudentsID { get; set; }               // serialized to one string for easy storage formatted ("id"+,"id2"+....)
        public String? HomeRoomTeacherID { get; set; }
        public String? MonitorID { get; set; }
    }
}


