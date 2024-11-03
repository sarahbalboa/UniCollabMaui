using Postgrest.Attributes;
using Postgrest.Models;



namespace UniCollabMaui.Models
{
    [Table("User")]
    public class User : BaseModel
    {
        [PrimaryKey("Id", false)]
        public int Id { get; set; }  // Use nullable long to avoid setting it explicitly

        public bool Active { get; set; }
        public string? Name { get; set; }
        public int Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
}
