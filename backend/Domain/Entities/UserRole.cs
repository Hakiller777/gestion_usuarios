using System.Text.Json.Serialization;
namespace backend.Models
{
    // Tabla intermedia para relación many-to-many entre User y Role
    public class UserRole
    {
        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }

        public int RoleId { get; set; }
        [JsonIgnore]
        public Role? Role { get; set; }
    }
}
