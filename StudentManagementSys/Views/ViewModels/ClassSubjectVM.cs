namespace StudentManagementSys.Views.ViewModels
{
    public class ClassSubjectVM
    {
        public String classSubjectId { get; set; }
        public String classSubjectCode { get; set; }
        public String subjectCode { get; set; }
        public String? SubjectName { get; set; }
        public String? typeClassSubject { get; set; }
        public List<StudentVM>? lstStudent { get; set; }
        public String? TeacherID { get; set; }
        public String? room { get; set; }
        public String? schedule { get; set; }
        public String? Semester { get; set; }
        public List<String>? lstStudentID { get; set; }
        public String? StudentIdsSerialized { get; set; }
    }
}
