using Microsoft.AspNetCore.Mvc.Rendering;
using MVCFaculty.Models;
using System.Collections.Generic;

namespace MVCFaculty.ViewModels
{
    public class TeachersSearchViewModel
    {
        public IList<Teacher> Teachers { get; set; }
        public string SFullName { get; set; }
        public SelectList Degrees { get; set; }
        public string SDegree { get; set; }
        public SelectList AcademicRanks { get; set; }
        public int SRank { get; set; }
    }
}
