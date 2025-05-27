using System.ComponentModel.DataAnnotations;

namespace technicalTestProject_1.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = string.Empty;
    }

    public class UpdateCategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = string.Empty;
    }
}