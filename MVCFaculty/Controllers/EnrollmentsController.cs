using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCFaculty.Models;
using MVCFaculty.ViewModels;

namespace MVCFaculty.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EnrollmentsController : Controller
    {
        private readonly MVCFacultyContext _context;

        public EnrollmentsController(MVCFacultyContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index(string sTitle, string sInd, string sSem, int? sYear)
        {
            IQueryable<Enrollment> enrollments = _context.Enrollments;
            if (!string.IsNullOrEmpty(sTitle))
            {
                enrollments = enrollments.Where(s => s.Course.Title.ToLower().Contains(sTitle.ToLower()));
            }
            if (!string.IsNullOrEmpty(sInd))
            {
                enrollments = enrollments.Where(s => s.Student.StudentId.Contains(sInd));
            }
            IQueryable<string> semesters = enrollments.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();
            IQueryable<int?> years = enrollments.OrderBy(m => m.Year).Select(m => m.Year).Distinct();
            if (!string.IsNullOrEmpty(sSem))
            {
                enrollments = enrollments.Where(x => x.Semester == sSem);
            }
            if (sYear != null)
            {
                enrollments = enrollments.Where(x => x.Year == sYear);
            }
            enrollments = enrollments.Include(e => e.Student).Include(e => e.Course);

            var enrollmentsSearchViewModel = new EnrollmentsSearchViewModel
            {
                Enrollments = await enrollments.ToListAsync(),
                Years = new SelectList(await years.ToListAsync()),
                Semesters = new SelectList(await semesters.ToListAsync())
            };

            System.Console.WriteLine(enrollments.ToString());
            return View(enrollmentsSearchViewModel);
        }

        public async Task<IActionResult> EnrollStudents()
        {
            IQueryable<Course> courses = _context.Courses;
            IQueryable<Student> students = _context.Students;
            string[] semesters = new[] { "Летен", "Зимски" };
            int nowYear = System.DateTime.Now.Year;
            int[] years = Enumerable.Range(nowYear - 10, 11).Reverse().ToArray();
            var enrollStudentsViewModel = new EnrollStudentsViewModel
            {
                Courses = new SelectList(await courses.ToListAsync(), "Id", "Title"),
                Students = new SelectList(await students.OrderBy(s => s.FullName).ToListAsync(), "Id", "FullNameId"),
                Years = new SelectList(years.ToList()),
                Semesters = new SelectList(semesters.ToList())
            };
            return View(enrollStudentsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollStudents(EnrollStudentsViewModel entry)
        {
            Course course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == entry.CourseId);
            if (course == null) return NotFound();

            if (ModelState.IsValid)
            {
                foreach (int sId in entry.StudentIds)
                {
                    Enrollment enrollment = await _context.Enrollments
                        .FirstOrDefaultAsync(c => c.CourseId == entry.CourseId && c.StudentId == sId &&
                        c.Year == entry.Year && c.Semester == entry.Semester);
                    if (enrollment == null)
                    {
                        enrollment = new Enrollment
                        {
                            CourseId = entry.CourseId,
                            StudentId = sId,
                            Year = entry.Year,
                            Semester = entry.Semester
                        };
                        _context.Add(enrollment);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { sTitle = course.Title, sSem = entry.Semester, sYear = entry.Year });
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnEnrollStudents(int? courseId, int? year, string semester)
        {
            IQueryable<Enrollment> enrollments = _context.Enrollments;
            List<Course> courses = await _context.Courses.ToListAsync();
            if (courseId != null)
            {
                enrollments = enrollments.Where(s => s.CourseId == courseId);
            }
            IQueryable<int?> years = enrollments.OrderBy(m => m.Year).Select(m => m.Year).Distinct();
            if (year != null)
            {
                enrollments = enrollments.Where(s => s.Year == year);
            }
            IQueryable<string> semesters = enrollments.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();
            if (!string.IsNullOrEmpty(semester))
            {
                enrollments = enrollments.Where(s => s.Semester == semester);
            }
            if (courseId != null && year != null && !string.IsNullOrEmpty(semester))
            {
                enrollments = enrollments.Include(e => e.Student);
            }
            else
                enrollments = null;

            var unenrollStudentsViewModel = new UnEnrollStudentsViewModel
            {
                Courses = new SelectList(courses, "Id", "Title"),
                Enrollments = (enrollments != null ? new SelectList(await enrollments.OrderBy(s => s.Student.FullNameId).ToListAsync(), "Id", "Student.FullNameId") : null),
                EnrollmentIds = (enrollments != null ? await enrollments.Select(e => e.Id).ToListAsync() : null),
                Years = new SelectList(years.ToList()),
                Semesters = new SelectList(semesters.ToList())
            };
            return View(unenrollStudentsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnEnrollStudents(UnEnrollStudentsViewModel entry)
        {
            if (ModelState.IsValid)
            {
                foreach (int eId in entry.EnrollmentIds)
                {
                    Enrollment enrollment = await _context.Enrollments
                        .FirstOrDefaultAsync(c => c.Id == eId);
                    if (enrollment != null)
                    {
                        enrollment.FinishDate = entry.FinishDate;
                        _context.Update(enrollment);
                    }
                }
                await _context.SaveChangesAsync();
                Course course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == entry.CourseId);
                return RedirectToAction(nameof(Index), new { sTitle = course.Title, sSem = entry.Semester, sYear = entry.Year });
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullNameId", enrollment.StudentId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }
    }
}
