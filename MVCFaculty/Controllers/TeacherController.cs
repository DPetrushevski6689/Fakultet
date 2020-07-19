using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCFaculty.Models;
using MVCFaculty.ViewModels;

namespace MVCFaculty.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly MVCFacultyContext _context;
        private readonly UserManager<AppUser> userManager;

        public TeacherController(MVCFacultyContext context, UserManager<AppUser> userMgr)
        {
            _context = context;
            userManager = userMgr;
        }

        // GET: Teachers/Courses/5
        public async Task<IActionResult> Courses(int? id)
        {
            if (id == null)
            {
                AppUser curruser = await userManager.GetUserAsync(User);
                if (curruser.TeacherId != null)
                    return RedirectToAction(nameof(Courses), new { id = curruser.TeacherId });
                else
                    return NotFound();
            }

            Teacher teacher = await _context.Teachers
                .Include(t => t.CoursesFirst).ThenInclude(c => c.SecondTeacher)
                .Include(t => t.CoursesSecond).ThenInclude(c => c.FirstTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            AppUser user = await userManager.GetUserAsync(User);
            if (id != user.TeacherId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            return View(teacher);
        }

        // GET: Teachers/EnrollmentsCourse/5?tId=2
        public async Task<IActionResult> EnrollmentsLastYear(int? id, int? tId)
        {
            if (id == null || tId == null)
            {
                return NotFound();
            }
            int? year = await _context.Enrollments.Where(e => e.CourseId == id).
                                OrderByDescending(m => m.Year).Select(m => m.Year).FirstOrDefaultAsync();
            return RedirectToAction(nameof(EnrollmentsCourse), new { id, tId, sYear = year });
        }

        // GET: Teachers/EnrollmentsCourse/5?tId=2
        public async Task<IActionResult> EnrollmentsCourse(int? id, int? tId, string sInd, string sSem, int? sYear)
        {
            if (id == null || tId == null)
            {
                return NotFound();
            }

            Teacher teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Id == tId);
            Course course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id
                    && (c.FirstTeacherId == tId || c.SecondTeacherId == tId));
            IQueryable<Enrollment> enrollments = _context.Enrollments.Where(e => e.CourseId == id);
            IQueryable<int?> years = enrollments.OrderBy(m => m.Year).Select(m => m.Year).Distinct();
            IQueryable<string> semesters = enrollments.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();

            if (!string.IsNullOrEmpty(sInd))
            {
                enrollments = enrollments.Where(s => s.Student.StudentId.Contains(sInd));
            }
            if (!string.IsNullOrEmpty(sSem))
            {
                enrollments = enrollments.Where(x => x.Semester == sSem);
            }
            if (sYear != null)
            {
                enrollments = enrollments.Where(x => x.Year == sYear);
            }
            enrollments = enrollments.Include(e => e.Student);

            if (teacher == null || course == null)
            {
                return NotFound();
            }

            AppUser user = await userManager.GetUserAsync(User);
            if (tId != user.TeacherId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            var teacherCourseViewModel = new TeacherCourseViewModel
            {
                Teacher = teacher,
                Course = course,
                Enrollments = await enrollments.ToListAsync(),
                Years = new SelectList(await years.ToListAsync()),
                Semesters = new SelectList(await semesters.ToListAsync())
            };

            return View(teacherCourseViewModel);
        }

        // GET: Teachers/EditEnrollment/11?tId=1
        public async Task<IActionResult> EditEnrollment(int? id, int? tId)
        {
            if (id == null || tId == null)
            {
                return NotFound();
            }

            Teacher teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Id == tId);
            Enrollment enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(e => e.Id == id
                    && (e.Course.FirstTeacherId == tId || e.Course.SecondTeacherId == tId));

            if (teacher == null || enrollment == null)
            {
                return NotFound();
            }

            AppUser user = await userManager.GetUserAsync(User);
            if (tId != user.TeacherId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            var teacherEnrollmentViewModel = new TeacherEnrollmentViewModel
            {
                Teacher = teacher,
                Enrollment = enrollment
            };

            return View(teacherEnrollmentViewModel);
        }

        // POST: Teachers/EditEnrollment/11?tId=1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEnrollment(int id, int tId, TeacherEnrollmentViewModel input)
        {
            Enrollment entry = input.Enrollment;
            if (entry.Id != id)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id
                    && (m.Course.FirstTeacherId == tId || m.Course.SecondTeacherId == tId));

            if (enrollment == null)
            {
                return NotFound();
            }

            AppUser user = await userManager.GetUserAsync(User);
            if (tId != user.TeacherId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    enrollment.ExamPoints = entry.ExamPoints;
                    enrollment.SeminalPoints = entry.SeminalPoints;
                    enrollment.ProjectPoints = entry.ProjectPoints;
                    enrollment.AdditionalPoints = entry.AdditionalPoints;
                    enrollment.Grade = entry.Grade;
                    enrollment.FinishDate = entry.FinishDate;
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(EditEnrollment), new { id, tId });
        }

        // GET: Students/DetailsEnrollment/11?tId=1
        public async Task<IActionResult> DetailsEnrollment(int? id, int? tId)
        {
            if (id == null || tId == null)
            {
                return NotFound();
            }

            Teacher teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Id == tId);
            Enrollment enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(e => e.Id == id && (e.Course.FirstTeacherId == tId || e.Course.SecondTeacherId == tId));

            if (teacher == null || enrollment == null)
            {
                return NotFound();
            }

            AppUser user = await userManager.GetUserAsync(User);
            if (tId != user.TeacherId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            var teacherEnrollmentViewModel = new TeacherEnrollmentViewModel
            {
                Teacher = teacher,
                Enrollment = enrollment
            };

            return View(teacherEnrollmentViewModel);
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }
    }
}