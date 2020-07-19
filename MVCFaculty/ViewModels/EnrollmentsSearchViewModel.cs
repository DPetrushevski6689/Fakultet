using Microsoft.AspNetCore.Mvc.Rendering;
using MVCFaculty.Models;
using System.Collections.Generic;

namespace MVCFaculty.ViewModels
{
    public class EnrollmentsSearchViewModel
    {
        public IList<Enrollment> Enrollments { get; set; }
        public string STitle { get; set; }
        public string SInd { get; set; }
        public SelectList Semesters { get; set; }
        public string SSem { get; set; }
        public SelectList Years { get; set; }
        public int SYear { get; set; }
    }
}
