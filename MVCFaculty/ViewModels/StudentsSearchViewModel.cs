using MVCFaculty.Models;
using System.Collections.Generic;

namespace MVCFaculty.ViewModels
{
    public class StudentsSearchViewModel
    {
        public IList<Student> Students { get; set; }
        public string SFullName { get; set; }
        public string SStudentId { get; set; }
    }
}
