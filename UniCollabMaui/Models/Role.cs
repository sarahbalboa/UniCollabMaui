using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCollabMaui.Models
{
    [Table("Role")]
    public class Role
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } 
        public string RoleName { get; set; }
        public int Active { get; set; }
        public int SystemRole { get; set; }
    }
}
