using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCFaculty.ViewModels
{
    public class UnEnrollStudentsViewModel
    {
        [Display(Name = "Предмет")]
        public int CourseId { get; set; }
        [Required]
        [Display(Name = "Студенти")]
        public IList<int> EnrollmentIds { get; set; }
        [Display(Name = "Семестар")]
        public string Semester { get; set; }
        [Display(Name = "Година")]
        public int Year { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Датум завршување")]
        public DateTime FinishDate { get; set; }
        public SelectList Courses { get; set; }
        public SelectList Enrollments { get; set; }
        public SelectList Semesters { get; set; }
        public SelectList Years { get; set; }
    }
}
