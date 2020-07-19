using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCFaculty.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Индекс")]
        public string StudentId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Презиме")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Датум упис")]
        public DateTime? EnrollmentDate { get; set; }

        [Range(0, 400)]
        [Display(Name = "Осв. кредити")]
        public int? AcquiredCredits { get; set; }

        [Range(0, 16)]
        [Display(Name = "Семестар")]
        public int? CurrentSemester { get; set; }
        
        [StringLength(25)]
        [Display(Name = "Степен")]
        public string EducationLevel { get; set; }

        [Display(Name = "Име и презиме")]
        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        [Display(Name = "Студент")]
        public string FullNameId
        {
            get
            {
                return String.Format("{0} {1}, {2}", FirstName, LastName, StudentId);
            }
        }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
