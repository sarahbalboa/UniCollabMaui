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
        public string ?RoleName { get; set; }
        public int Active { get; set; }
        public bool IsSystemRole { get; set; } //default system roles (non-editable) 
        public bool IsRoleAdmin { get; set; } //access CRUD and assign roles
        public bool IsTaskEditor { get; set; } //access to CRUD tasks/taskboard
        public bool IsTaskViewer { get; set; } //access to view tasks/taskboard
        public bool IsProgressEditor { get; set; } //access to CRUD progress visualisation
        public bool IsProgressViewer { get; set; } //access to view progress visualisation
    }
}
