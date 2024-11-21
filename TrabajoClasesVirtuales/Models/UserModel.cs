using System.ComponentModel.DataAnnotations;

namespace TrabajoClasesVirtuales.Models
{
    using System.ComponentModel.DataAnnotations;

    namespace Proyecto.Models
    {
        public class UserModel
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [MaxLength(50)]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string PasswordHash { get; set; }
        }
    }

}
