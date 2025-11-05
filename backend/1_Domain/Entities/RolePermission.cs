namespace backend.Domain.Entities
{
    // Tabla intermedia para relación many-to-many entre Role y Permission
    public class RolePermission
    {
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public int PermissionId { get; set; }
        public Permission? Permission { get; set; }
    }
}
