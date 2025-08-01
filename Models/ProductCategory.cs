using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementApp.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
