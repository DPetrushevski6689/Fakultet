using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCFaculty.Models
{
    public class AppUser : IdentityUser
    {
        [Display(Name = "Улога")]
        public string Role { get; set; }
        public int? TeacherId { get; set; }
        [Display(Name = "Наставник")]
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }
        public int? StudentId { get; set; }
        [Display(Name = "Студент")]
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }
}

