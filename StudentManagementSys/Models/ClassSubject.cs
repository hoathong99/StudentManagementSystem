using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSys.Model
{
    public class ClassSubject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public String classSubjectId { get; set; }
        [Required]
        public String classSubjectCode { get; set; }
        [Required]
        public String subjectCode { get; set; }
        public String? SubjectName { get; set; }
        public String? typeClassSubject { get; set; }
        public String? lstStudentID { get; set; }           // serialized to one string for easy storage formatted ("id"+,"id2"+....)
        public String? TeacherID { get; set; }
        public String? room { get; set; }
        public String? schedule { get; set; }
        public String? Semester { get; set; }
    }
}
