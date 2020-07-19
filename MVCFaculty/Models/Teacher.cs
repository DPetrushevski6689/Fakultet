using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCFaculty.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Презиме")]
        public string LastName { get; set; }

        [StringLength(50)]
        [Display(Name = "Степен образование")]
        public string Degree { get; set; }

        [StringLength(25)]
        [Display(Name = "Звање")]
        public string AcademicRank { get; set; }

        [StringLength(10)]
        [Display(Name = "Канцеларија")]
        public string OfficeNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Датум вработување")]
        public DateTime? HireDate { get; set; }

        [Display(Name = "Име и презиме")]
        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        public ICollection<Course> CoursesFirst { get; set; }
        public ICollection<Course> CoursesSecond { get; set; }
    }
}
