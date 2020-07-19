using Microsoft.AspNetCore.Mvc.Rendering;
using MVCFaculty.Models;
using System.Collections.Generic;

namespace MVCFaculty.ViewModels
{
    public class CoursesSearchViewModel
    {
        public IList<Course> Courses { get; set; }
        public string STitle { get; set; }
        public SelectList Programs { get; set; }
        public string SProgram { get; set; }
        public SelectList Semesters { get; set; }
        public int SSemester { get; set; }
    }
}
