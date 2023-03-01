using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagementSys.Controllers.Dto;

namespace StudentManagementSys.Views.Classrooms
{
    public class ClassroomVM
    {
        public String CRID { get; set; }
        public List<StudentVM>? StudentsID { get; set; }
        public String? HomeRoomTeacherID { get; set; }
        public String? HomeRoomTeacherName { get; set; }
        public String? MonitorID { get; set; }
        public IEnumerable<SelectListItem> StaffList { get; set; }
        public IEnumerable<SelectListItem> StudentList { get; set; }

    }
    public class StudentVM
    {
        public string id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
    }

    public class AddStudentToClassVM
    {
        public List<StudentDto> students { get; set; }
        public string cid { get; set; }

    }
}
