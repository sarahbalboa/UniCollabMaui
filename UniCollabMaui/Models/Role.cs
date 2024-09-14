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
        public bool Active { get; set; }
        public bool IsSystemRole { get; set; } //default system roles (non-editable) 
        public bool IsRoleAdmin { get; set; } //access CRUD and assign roles and manage users
        public bool IsTaskAdmin { get; set; } //access to CRUD tasks/taskboard
        public bool IsTaskViewer { get; set; } //access to view tasks/taskboard
        public bool IsProgressViewer { get; set; } //access to view progress and user insights visualisation
    }
}
