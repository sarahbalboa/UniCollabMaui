using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace UniCollabMaui.Models
{
    [Table("User")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool Active {  get; set; }
        public string ?Name { get; set; }
        public string ?Username { get; set; }
        public string ?Password { get; set; }
        public int RoleId { get; set; }    
    }
}
