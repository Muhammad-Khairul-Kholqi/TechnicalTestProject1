using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace technicalTestProject_1.Models
{
    public class UpdateProfileViewModel
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        // public string ProfileImageUrl { get; set; }
    }
}