using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVCFaculty.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            IdentityResult roleResult;
            //Adding Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }
            roleCheck = await RoleManager.RoleExistsAsync("Student");
            if (!roleCheck)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Student"));
            }
            roleCheck = await RoleManager.RoleExistsAsync("Teacher");
            if (!roleCheck)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Teacher"));
            }

            AppUser user = await UserManager.FindByEmailAsync("admin@mvcfaculty.com");
            if (user == null)
            {
                var User = new AppUser();
                User.Email = "admin@mvcfaculty.com";
                User.UserName = "admin@mvcfaculty.com";
                User.Role = "Admin";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin      
                if (chkUser.Succeeded)
                {
                    var result1 = await UserManager.AddToRoleAsync(User, "Admin");
                }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MVCFacultyContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MVCFacultyContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
                // Look for any movies.
                if (context.Courses.Any() || context.Teachers.Any() || context.Students.Any())
                {
                    return;   // DB has been seeded
                }

                context.Teachers.AddRange(
                    new Teacher { /*Id = 1, */FirstName = "Даниел", LastName = "Денковски", Degree = "Доктор на науки", AcademicRank = "Доцент", OfficeNumber = "121б", HireDate = DateTime.Parse("2017-1-1") },
                    new Teacher { /*Id = 2, */FirstName = "Перо", LastName = "Латкоски", Degree = "Доктор на науки",  AcademicRank = "Ред. професор", OfficeNumber = "ТК", HireDate = DateTime.Parse("2006-1-1") }
                );
                context.SaveChanges();

                context.Courses.AddRange(
                    new Course { 
                        /*Id = 1, */
                        Title = "Мобилни сервиси со Андроид програмирање",
                        Credits = 6,
                        Semester = 6,
                        Programme = "КТИ, ТКИИ",
                        EducationLevel = "Додипломски",
                        FirstTeacherId = 1,
                        SecondTeacherId = 2
                    },
                    new Course { 
                        /*Id = 2, */
                        Title = "Развој на серверски WEB апликации",
                        Credits = 6,
                        Semester = 6,
                        Programme = "КТИ, ТКИИ",
                        EducationLevel = "Додипломски",
                        FirstTeacherId = 1,
                        SecondTeacherId = 2
                    }
                );
                context.SaveChanges();

                context.Students.AddRange(
                    new Student { /*Id = 1, */FirstName = "Давид", LastName = "Петрушевски", StudentId = "116/2017", CurrentSemester = 6, EducationLevel = "Додипломски", EnrollmentDate = DateTime.Parse("2017-9-1") },
                    new Student { /*Id = 2, */FirstName = "Евгенија", LastName = "Богоевска", StudentId = "240/2017", CurrentSemester = 6, EducationLevel = "Додипломски", EnrollmentDate = DateTime.Parse("2017-9-1") },
                    new Student { /*Id = 3, */FirstName = "Жанета", LastName = "Тренчева", StudentId = "9/2017", CurrentSemester = 6, EducationLevel = "Додипломски", EnrollmentDate = DateTime.Parse("2017-9-1") },
                    new Student { /*Id = 4, */FirstName = "Сара", LastName = "Стојаноска", StudentId = "265/2017", CurrentSemester = 6, EducationLevel = "Додипломски", EnrollmentDate = DateTime.Parse("2017-9-1") },
                    new Student { /*Id = 5, */FirstName = "Алисе", LastName = "Вејселова", StudentId = "175/2017", CurrentSemester = 6, EducationLevel = "Додипломски", EnrollmentDate = DateTime.Parse("2017-9-1") },
                    new Student { /*Id = 6, */FirstName = "Петар", LastName = "Илиевски", StudentId = "182/2017", CurrentSemester = 6, EducationLevel = "Додипломски", EnrollmentDate = DateTime.Parse("2017-9-1") },
                    new Student { /*Id = 7, */FirstName = "Виктор", LastName = "Наумоски", StudentId = "198/2017", CurrentSemester = 6, EducationLevel = "Додипломски", EnrollmentDate = DateTime.Parse("2017-9-1") }
                );
                context.SaveChanges();

                context.Enrollments.AddRange(
                    new Enrollment { CourseId = 1, StudentId = 1, Year = 2020, Semester = "Летен" },
                    new Enrollment { CourseId = 1, StudentId = 2, Year = 2020, Semester = "Летен" },
                    new Enrollment { CourseId = 1, StudentId = 3, Year = 2020, Semester = "Летен" },
                    new Enrollment { CourseId = 1, StudentId = 4, Year = 2020, Semester = "Летен" },
                    new Enrollment { CourseId = 2, StudentId = 1, Year = 2020, Semester = "Летен" },
                    new Enrollment { CourseId = 2, StudentId = 2, Year = 2020, Semester = "Летен" },
                    new Enrollment { CourseId = 2, StudentId = 3, Year = 2020, Semester = "Летен" },
                    new Enrollment { CourseId = 2, StudentId = 4, Year = 2020, Semester = "Летен" },
                    new Enrollment { CourseId = 2, StudentId = 5, Year = 2020, Semester = "Летен" },
                    new Enrollment { CourseId = 2, StudentId = 6, Year = 2020, Semester = "Летен" },
                    new Enrollment { CourseId = 2, StudentId = 7, Year = 2020, Semester = "Летен" }
                );
                context.SaveChanges();
            }
        }
    }
}
