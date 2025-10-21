namespace backend.Models
{
    // Tabla intermedia para relación many-to-many entre User y Role
    public class UserRole
    {
        public int UserId { get; set; } //fk apunta a la pk de la tabla User(Users con EFcore)
        public User User { get; set; } = null!; //Para acceder a los datos de usuario desde UserRole

        public int RoleId { get; set; } //fk apunta a la tabla role
        public Role Role { get; set; } = null!; //Para acceder a todos los datos del rol desde UserRole
    }
}