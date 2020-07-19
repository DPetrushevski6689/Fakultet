using Microsoft.AspNetCore.Http;
using MVCFaculty.Models;
using System.ComponentModel.DataAnnotations;

namespace MVCFaculty.ViewModels
{
    public class EnrollmentUploadViewModel
    {
        public Enrollment Enrollment { get; set; }
        [Display(Name = "Прикачи семинарска")]
        public IFormFile SeminalFile { get; set; }
    }
}
