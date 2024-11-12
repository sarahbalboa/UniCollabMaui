using Postgrest.Models; // Make sure to include this namespace
using Postgrest.Attributes;
namespace UniCollabMaui.Models
{
    public class Session : BaseModel
    {
        /// <summary>
        /// Session model that represents the Session table from the database
        /// </summary>
        [PrimaryKey("SessionId", true)]
        public string SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
