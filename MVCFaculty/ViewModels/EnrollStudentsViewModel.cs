using Microsoft.AspNetCore.Mvc.Rendering;
using MVCFaculty.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCFaculty.ViewModels
{
    public class EnrollStudentsViewModel
    {
        [Required]
        [Display(Name = "Предмет")]
        public int CourseId { get; set; }
        [Required]
        [Display(Name = "Студенти")]
        public IList<int> StudentIds { get; set; }
        [Required]
        [Display(Name = "Година")]
        public int Year { get; set; }
        [Required]
        [Display(Name = "Семестар")]
        public string Semester { get; set; }
        public SelectList Courses { get; set; }
        public SelectList Students { get; set; }
        public SelectList Semesters { get; set; }
        public SelectList Years { get; set; }
    }
}
