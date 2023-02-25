using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSys.Model
{
    public class Item
    {
        public String Name { get; set; }
        public String Desc { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public String ItemID { get; set; }
        public int price { get; set; }
    }
}
