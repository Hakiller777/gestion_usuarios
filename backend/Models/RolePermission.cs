namespace backend.Models
{
    // Tabla intermedia para relación many-to-many entre Role y Permission
    public class RolePermission
    {
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
 
        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }
}