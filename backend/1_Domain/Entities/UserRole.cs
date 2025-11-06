using System.Text.Json.Serialization;
namespace backend.Domain.Entities
{
    // Tabla intermedia para relación many-to-many entre User y Role
    public class UserRole //Define la entidad intermedia UserRole que representa la relación many-to-many entre User y Role.
    {
        public int UserId { get; set; } //Propiedad que representa la clave foránea hacia User.
        [JsonIgnore] //Evita la serialización de la propiedad User para prevenir referencias circulares.
        public User? User { get; set; } //Propiedad de navegación hacia la entidad User. Es nullable reference type para reflejar que puede no estar cargada.

        public int RoleId { get; set; } //Propiedad que representa la clave foránea hacia Role.
        [JsonIgnore] //Evita la serialización de la propiedad Role para prevenir referencias circulares.
        public Role? Role { get; set; } //Propiedad de navegación hacia la entidad Role. Es nullable reference type para reflejar que puede no estar cargada.
    }
}
