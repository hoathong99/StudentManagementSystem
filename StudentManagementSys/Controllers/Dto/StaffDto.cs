namespace StudentManagementSys.Controllers.Dto
{
    public class StaffDto : UserDto
    {
        public String? School { get; set; }
        public List<String>? LstClassSubject { get; set; }            // serialized to one string for easy storage formatted ("id"+,"id2"+....)
        public List<String>? LstClassRoom { get; set; }               // serialized to one string for easy storage ("id"+,"id2"+....)
    }
}
