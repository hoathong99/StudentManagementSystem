using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagementSys.Controllers.Dto;

namespace StudentManagementSys.Views.ViewModels
{
    public class ClassroomVM
    {
        public string CRID { get; set; }
        public List<StudentVM>? StudentsID { get; set; }
        public string? HomeRoomTeacherID { get; set; }
        public string? HomeRoomTeacherName { get; set; }
        public string? MonitorID { get; set; }
        public IEnumerable<SelectListItem> StaffList { get; set; }
        public IEnumerable<SelectListItem> StudentList { get; set; }

    }

    public class AddStudentToClassVM
    {
        public List<StudentDto> students { get; set; }
        public string cid { get; set; }

    }
}
