using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCFaculty.Models;

namespace MVCFaculty.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private UserManager<AppUser> userManager;
        private IPasswordHasher<AppUser> passwordHasher;
        private IPasswordValidator<AppUser> passwordValidator;
        private IUserValidator<AppUser> userValidator;
        private readonly MVCFacultyContext _context;

        public AdminController(UserManager<AppUser> usrMgr, IPasswordHasher<AppUser> passwordHash, IPasswordValidator<AppUser> passwordVal, IUserValidator<AppUser>
userValid, MVCFacultyContext context)
        {
            userManager = usrMgr;
            passwordHasher = passwordHash;
            passwordValidator = passwordVal;
            userValidator = userValid;
            _context = context;
        }

        public IActionResult Index()
        {
            IQueryable<AppUser> users = userManager.Users.OrderBy(u => u.Email);
            return View(users);
        }

        public IActionResult TeacherProfile(int teacherId)
        {
            //AppUser user = await userManager.FindByIdAsync(id);
            AppUser user = userManager.Users.FirstOrDefault(u => u.TeacherId == teacherId);
            Teacher teacher = _context.Teachers.Where(s => s.Id == teacherId).FirstOrDefault();
            if (teacher != null)
            {
                ViewData["FullName"] = teacher.FullName;
                ViewData["TeacherId"] = teacher.Id;
            }
            if (user != null)
                return View(user);
            else
                return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> TeacherProfile(int teacherId, string email, string password, string phoneNumber)
        {
            //AppUser user = await userManager.FindByIdAsync(id);
            AppUser user = userManager.Users.FirstOrDefault(u => u.TeacherId == teacherId);
            if (user != null)
            {
                IdentityResult validUser = null;
                IdentityResult validPass = null;

                user.Email = email;
                user.UserName = email;
                user.PhoneNumber = phoneNumber;

                if (string.IsNullOrEmpty(email))
                    ModelState.AddModelError("", "Email cannot be empty");

                validUser = await userValidator.ValidateAsync(userManager, user);
                if (!validUser.Succeeded)
                    Errors(validUser);

                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, user, password);
                    if (validPass.Succeeded)
                        user.PasswordHash = passwordHasher.HashPassword(user, password);
                    else
                        Errors(validPass);
                }

                if (validUser != null && validUser.Succeeded && (string.IsNullOrEmpty(password) || validPass.Succeeded))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(TeacherProfile), new { teacherId });
                    else
                        Errors(result);
                }
            }
            else
            {
                AppUser newuser = new AppUser();
                IdentityResult validUser = null;
                IdentityResult validPass = null;

                newuser.Email = email;
                newuser.UserName = email;
                newuser.PhoneNumber = phoneNumber;
                newuser.TeacherId = teacherId;
                newuser.Role = "Teacher";

                if (string.IsNullOrEmpty(email))
                    ModelState.AddModelError("", "Email cannot be empty");

                validUser = await userValidator.ValidateAsync(userManager, newuser);
                if (!validUser.Succeeded)
                    Errors(validUser);

                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, newuser, password);
                    if (validPass.Succeeded)
                        newuser.PasswordHash = passwordHasher.HashPassword(newuser, password);
                    else
                        Errors(validPass);
                }
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (validUser != null && validUser.Succeeded && validPass != null && validPass.Succeeded)
                {
                    IdentityResult result = await userManager.CreateAsync(newuser, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newuser, "Teacher");
                        return RedirectToAction(nameof(TeacherProfile), new { teacherId });
                    }
                    else
                        Errors(result);
                }
                user = newuser;
            }
            Teacher teacher = _context.Teachers.Where(s => s.Id == teacherId).FirstOrDefault();
            if (teacher != null)
            {
                ViewData["FullName"] = teacher.FullName;
                ViewData["TeacherId"] = teacher.Id;
            }
            return View(user);
        }

        public IActionResult StudentProfile(int studentId)
        {
            //AppUser user = await userManager.FindByIdAsync(id);
            AppUser user = userManager.Users.FirstOrDefault(u => u.StudentId == studentId);
            Student student = _context.Students.Where(s => s.Id == studentId).FirstOrDefault();
            if (student != null)
            {
                ViewData["FullNameId"] = student.FullNameId;
                ViewData["StudentId"] = student.Id;
            }
            if (user != null)
                return View(user);
            else
                return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> StudentProfile(int studentId, string email, string password, string phoneNumber)
        {
            //AppUser user = await userManager.FindByIdAsync(id);
            AppUser user = userManager.Users.FirstOrDefault(u => u.StudentId == studentId);
            if (user != null)
            {
                IdentityResult validUser = null;
                IdentityResult validPass = null;

                user.Email = email;
                user.UserName = email;
                user.PhoneNumber = phoneNumber;

                if (string.IsNullOrEmpty(email))
                    ModelState.AddModelError("", "Email cannot be empty");

                validUser = await userValidator.ValidateAsync(userManager, user);
                if (!validUser.Succeeded)
                    Errors(validUser);

                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, user, password);
                    if (validPass.Succeeded)
                        user.PasswordHash = passwordHasher.HashPassword(user, password);
                    else
                        Errors(validPass);
                }

                if (validUser != null && validUser.Succeeded && (string.IsNullOrEmpty(password) || validPass.Succeeded))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(StudentProfile), new { studentId });
                    else
                        Errors(result);
                }
            }
            else
            {
                AppUser newuser = new AppUser();
                IdentityResult validUser = null;
                IdentityResult validPass = null;

                newuser.Email = email;
                newuser.UserName = email;
                newuser.PhoneNumber = phoneNumber;
                newuser.StudentId = studentId;
                newuser.Role = "Student";

                if (string.IsNullOrEmpty(email))
                    ModelState.AddModelError("", "Email cannot be empty");

                validUser = await userValidator.ValidateAsync(userManager, newuser);
                if (!validUser.Succeeded)
                    Errors(validUser);

                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, newuser, password);
                    if (validPass.Succeeded)
                        newuser.PasswordHash = passwordHasher.HashPassword(newuser, password);
                    else
                        Errors(validPass);
                }
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (validUser != null && validUser.Succeeded && validPass != null && validPass.Succeeded)
                {
                    IdentityResult result = await userManager.CreateAsync(newuser, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newuser, "Student");
                        return RedirectToAction(nameof(StudentProfile), new { studentId });
                    }
                    else
                        Errors(result);
                }
                user = newuser;
            }
            Student student = _context.Students.Where(s => s.Id == studentId).FirstOrDefault();
            if (student != null)
            {
                ViewData["FullNameId"] = student.FullNameId;
                ViewData["StudentId"] = student.Id;
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", userManager.Users);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}