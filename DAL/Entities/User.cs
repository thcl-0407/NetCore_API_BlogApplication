using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        public Guid UserID { get; set; }

        [Required]
        [MaxLength]
        public string UserName { get; set; }
        
        [Required]
        [MaxLength]
        public string HashPassword { get; set; }

        [Required]
        [MaxLength]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
        public string Email { get; set; }

        public bool isAuthenticated { get; set; }

        public Token Token { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
