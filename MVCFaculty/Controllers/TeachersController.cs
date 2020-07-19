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
    public class TeachersController : Controller
    {
        private readonly MVCFacultyContext _context;

        public TeachersController(MVCFacultyContext context)
        {
            _context = context;
        }

        // GET: Teachers
        public async Task<IActionResult> Index(string sFullName, string sDegree, string sRank)
        {
            IQueryable<Teacher> teachers = _context.Teachers.AsQueryable();
            IQueryable<string> degrees = _context.Teachers.OrderBy(m => m.Degree).Select(m => m.Degree).Distinct();
            IQueryable<string> ranks = _context.Teachers.OrderBy(m => m.AcademicRank).Select(m => m.AcademicRank).Distinct();

            if (!string.IsNullOrEmpty(sFullName)) {
                teachers = teachers.Where(s => (s.FirstName + " " + s.LastName).ToLower().Contains(sFullName.ToLower()));
            }
            if (!string.IsNullOrEmpty(sDegree)) {
                teachers = teachers.Where(x => x.Degree.ToLower().Contains(sDegree.ToLower()));
            }
            if (!string.IsNullOrEmpty(sRank)) {
                teachers = teachers.Where(x => x.AcademicRank.ToLower().Contains(sRank.ToLower()));
            }

            var teacherSearchViewModel = new TeachersSearchViewModel
            {
                Degrees = new SelectList(await degrees.ToListAsync()),
                AcademicRanks = new SelectList(await ranks.ToListAsync()),
                Teachers = await teachers.ToListAsync()
            };

            System.Console.WriteLine(teachers.ToString());
            return View(teacherSearchViewModel);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
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
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
