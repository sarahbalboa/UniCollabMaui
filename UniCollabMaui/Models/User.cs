using Postgrest.Models; // Make sure to include this namespace

namespace UniCollabMaui.Models
{
    public class User : BaseModel
    {
        public int Id { get; set; }
        public bool Active {  get; set; }
        public string ?Name { get; set; }
        public string ?Username { get; set; } //student no
        public string ?Password { get; set; }
        public int RoleId { get; set; }   
        public string Email { get; set; }
    }
}
