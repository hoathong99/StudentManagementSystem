namespace StudentManagementSys.Model
{
    public class Staff : User
    {
        public String? School { get; set; }
        public String? LstClassSubject { get; set; }            // serialized to one string for easy storage formatted ("id"+,"id2"+....)
        public String? LstClassRoom { get; set; }               // serialized to one string for easy storage ("id"+,"id2"+....)
    }
}
