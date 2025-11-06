using System.ComponentModel.DataAnnotations;  //Importa los atributos de validación ([Key], [Required], [StringLength]) usados para validación de modelos y para que EF Core aplique convenciones/constraints en la BD.

namespace backend.Domain.Entities //•	Agrupa la entidad dentro de la capa de dominio. Facilita organización, descubrimiento por convención y evita colisiones de nombres.
{
    public class Role  //Define la entidad "Role" que representa un rol de usuario en el sistema.
    {
        [Key] //Atributo de data annotations que marca la propiedad siguiente como clave primaria explícita. EF Core detecta Id por convención, pero Key la hace explícita.
        public int Id { get; set; } //	Propiedad que representa la clave primaria. Tipo int — común para identities autoincrementales en SQL. EF la mapeará a la columna PK.

        [Required(ErrorMessage = "El nombre del rol es obligatorio")] //Indica que la propiedad Name no puede ser nula. Se usa tanto para validación de modelos en la capa de presentación como para influir en el esquema/constraints en EF Core.
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")] //Define restricciones de longitud: máximo 50, mínimo 3. También influye en validación de entrada y en el esquema (p. ej. crea nvarchar(50)).
        public string Name { get; set; } = string.Empty; //Propiedad no nullable (proyecto tiene Nullable activado). Se inicializa a string.Empty para garantizar no-null y evitar advertencias de compilador. Representa el nombre legible del rol.

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>(); //Navegación hacia la tabla intermedia UserRole (relación many-to-many entre User y Role).

        // Nombre plural para consistencia con Permission.RolePermissions
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>(); //Navegación hacia la tabla intermedia RolePermission (relación many-to-many entre Role y Permission). Igual razón de inicialización y tipo.
    }
}



//Notas y conceptos transversales relevantes:
//Nullable reference types está activado en el proyecto; por eso las propiedades string y ICollection<> se inicializan para cumplir la anotación no-nullable y evitar advertencias del compilador.
//Los atributos de DataAnnotations ([Required], [StringLength]) cumplen doble rol: validación en la capa de presentación (model binding + validation) y guía para EF Core al generar el esquema.
//Las relaciones many-to-many están modeladas con entidades intermedias (UserRole, RolePermission) en vez de la relación "skip navigation". Esto es útil cuando la tabla intermedia necesita campos adicionales (por ejemplo, fecha de asignación) o cuando quieres control explícito.
//Inicializar colecciones en la entidad es buena práctica: evita NRE y facilita trabajar con la entidad sin comprobar null.
//virtual no se usa: si necesitaras lazy-loading con proxies de EF Core tendrías que marcar las navegaciones como virtual y habilitar proxies; hoy esa opción no está activa por defecto y lazy-loading no es recomendable en APIs por sorpresa de consultas.
//Sugerencias prácticas (opcional):
//Si quieres API más inmutable/segura expón IReadOnlyCollection<RolePermission> y maneja mutaciones desde métodos en la entidad o servicios del dominio.
//Considera validar invariantes (p. ej. nombre no vacío) en un constructor o método de fábrica si el dominio exige reglas más estrictas que las anotaciones.

