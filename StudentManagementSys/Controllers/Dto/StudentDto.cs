namespace StudentManagementSys.Controllers.Dto
{
    public class StudentDto : UserDto
    {
        public String? SchoolSession { get; set; }
        public Double? CPA { get; set; }
        public int? TotalCredit { get; set; }
        public int? PassedCredit { get; set; }
        public String? ClassRoomID { get; set; }
        public String? Program { get; set; }

        // relations
        public List<String>? SubjectEnlisted { get; set; }
    }
}
