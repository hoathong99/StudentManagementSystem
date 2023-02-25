using AutoMapper;
using StudentManagementSys.Model;

namespace StudentManagementSys.Controllers.Dto
{
    public class ClassroomDto
    {
        public String CRID { get; set; }
        public List<String>? StudentsID { get; set; }               
        public String? HomeRoomTeacherID { get; set; }
        public String? MonitorID { get; set; }
    }
}
