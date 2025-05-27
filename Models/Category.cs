using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace technicalTestProject_1.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public string CreatorId { get; set; } = string.Empty;

        [ForeignKey("CreatorId")]
        public virtual ApplicationUser Creator { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}