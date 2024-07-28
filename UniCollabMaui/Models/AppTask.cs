using SQLite;

namespace UniCollabMaui.Models
{
    public class AppTask
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Column { get; set; }
        public string Priority { get; set; } // Add this property
        public int AssignedToUserId { get; set; }
    }
}
