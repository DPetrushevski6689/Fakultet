using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCFaculty.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        [Display(Name = "Предмет")]
        public int CourseId { get; set; }
        [Display(Name = "Предмет")]
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        [Display(Name = "Студент")]
        public int StudentId { get; set; }
        [Display(Name = "Студент")]
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        [StringLength(10)]
        [Display(Name = "Семестар")]
        public string Semester { get; set; }

        [Range(2000, 2050)]
        [Display(Name = "Година")]
        public int? Year { get; set; }

        [Range(5, 10)]
        [Display(Name = "Оцена")]
        public int? Grade { get; set; }

        [StringLength(255)]
        [Display(Name = "Семинарска")]
        public string SeminalUrl { get; set; }

        [StringLength(255)]
        [Display(Name = "Проект")]
        public string ProjectUrl { get; set; }

        [Range(0, 200)]
        [Display(Name = "Поени испит")]
        public int? ExamPoints { get; set; }

        [Range(0, 200)]
        [Display(Name = "Поени сем.")]
        public int? SeminalPoints { get; set; }

        [Range(0, 200)]
        [Display(Name = "Поени проект")]
        public int? ProjectPoints { get; set; }

        [Range(0, 200)]
        [Display(Name = "Доп. поени")]
        public int? AdditionalPoints { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Датум завршување")]
        public DateTime? FinishDate { get; set; }
    }
}
