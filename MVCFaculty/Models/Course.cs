using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCFaculty.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Предмет")]
        public string Title { get; set; }

        [Required]
        [Range(0, 60)]
        [Display(Name = "Кредити")]
        public int Credits { get; set; }

        [Required]
        [Range(0, 16)]
        [Display(Name = "Семестар")]
        public int Semester { get; set; }

        [StringLength(100)]
        [Display(Name = "Програма")]
        public string Programme { get; set; }

        [StringLength(25)]
        [Display(Name = "Степен")]
        public string EducationLevel { get; set; }

        [Display(Name = "Наставник 1")]
        public int? FirstTeacherId { get; set; }
        [Display(Name = "Наставник 1")]
        [ForeignKey("FirstTeacherId")]
        public Teacher FirstTeacher { get; set; }
        [Display(Name = "Наставник 2")]
        public int? SecondTeacherId { get; set; }
        [Display(Name = "Наставник 2")]
        [ForeignKey("SecondTeacherId")]
        public Teacher SecondTeacher { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
