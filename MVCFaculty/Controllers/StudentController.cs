using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCFaculty.Models;
using MVCFaculty.ViewModels;

namespace MVCFaculty.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly MVCFacultyContext _context;
        private readonly UserManager<AppUser> userManager;
        private readonly IHostingEnvironment webHostEnvironment;

        public StudentController(MVCFacultyContext context, IHostingEnvironment hostEnvironment, UserManager<AppUser> userMgr)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
            userManager = userMgr;
        }

        // GET: Student/Enrollments/5
        public async Task<IActionResult> Enrollments(int? id)
        {
            if (id == null)
            {
                AppUser curruser = await userManager.GetUserAsync(User);
                if (curruser.StudentId != null)
                    return RedirectToAction(nameof(Enrollments), new { id = curruser.StudentId });
                else
                    return NotFound();
            }

            Student student = await _context.Students
                .Include(s => s.Enrollments).ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            AppUser user = await userManager.GetUserAsync(User);
            if (student.Id != user.StudentId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            return View(student);
        }

        // GET: Student/EditEnrollment/11
        public async Task<IActionResult> EditEnrollment(int? id)
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

            AppUser user = await userManager.GetUserAsync(User);
            if (enrollment.StudentId != user.StudentId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            var memory = new MemoryStream();

            var enrollmentUploadViewModel = new EnrollmentUploadViewModel
            {
                Enrollment = enrollment,
                SeminalFile = null
            };

            return View(enrollmentUploadViewModel);
        }

        // POST: Student/EditEnrollment/11
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEnrollment(int id, EnrollmentUploadViewModel entry)
        {
            if (entry.Enrollment.Id != id)
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

            AppUser user = await userManager.GetUserAsync(User);
            if (enrollment.StudentId != user.StudentId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    enrollment.ProjectUrl = entry.Enrollment.ProjectUrl;
                    enrollment.SeminalUrl = UploadedFile(entry);
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
            return RedirectToAction(nameof(EditEnrollment));
        }

        // GET: Student/DetailsEnrollment/11
        public async Task<IActionResult> DetailsEnrollment(int? id)
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

            AppUser user = await userManager.GetUserAsync(User);
            if (enrollment.StudentId != user.StudentId)
            {
                return RedirectToAction("AccessDenied", "Account", null);
            }

            return View(enrollment);
        }

        private string UploadedFile(EnrollmentUploadViewModel model)
        {
            string uniqueFileName = null;

            if (model.SeminalFile != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "documents");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.SeminalFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.SeminalFile.CopyTo(fileStream);
                }
                uniqueFileName = "/documents/" + uniqueFileName;
            }
            return uniqueFileName;
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }
    }
}