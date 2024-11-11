using Postgrest.Attributes;
using Postgrest.Models;


namespace UniCollabMaui.Models
{
    /// <summary>
    /// AppTask model that represents the AppTask table from the database
    /// </summary>
    [Table("AppTask")]
    public class AppTask : BaseModel
    {
        [PrimaryKey("Id", false)]
        public int Id { get; set; }
        public string ?Title { get; set; }
        public string ?Description { get; set; }
        public string ?Column { get; set; }
        public string ?Priority { get; set; } // Add this property
        public int? AssignedToUserId { get; set; }
    }
}
