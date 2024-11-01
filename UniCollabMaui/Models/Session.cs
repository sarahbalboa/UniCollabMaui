using Postgrest.Models; // Make sure to include this namespace

namespace UniCollabMaui.Models
{

    public class Session : BaseModel
    {

        public string SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
