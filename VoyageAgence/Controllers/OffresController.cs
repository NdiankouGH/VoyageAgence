using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VoyageAgence.Data;
using VoyageAgence.Models;

namespace VoyageAgence.Controllers
{
    public class OffresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OffresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Offres
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Offres.Include(o => o.Agence);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Offres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offre = await _context.Offres
                .Include(o => o.Agence)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offre == null)
            {
                return NotFound();
            }

            return View(offre);
        }

        // GET: Offres/Create
        public IActionResult Create()
        {
            ViewData["AgenceId"] = new SelectList(_context.Agences, "Id", "Adresse");
            return View();
        }

        // POST: Offres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Prix,Disponible,AgenceId,DateCreation")] Offre offre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(offre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AgenceId"] = new SelectList(_context.Agences, "Id", "Adresse", offre.AgenceId);
            return View(offre);
        }

        // GET: Offres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offre = await _context.Offres.FindAsync(id);
            if (offre == null)
            {
                return NotFound();
            }
            ViewData["AgenceId"] = new SelectList(_context.Agences, "Id", "Adresse", offre.AgenceId);
            return View(offre);
        }

        // POST: Offres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Prix,Disponible,AgenceId,DateCreation")] Offre offre)
        {
            if (id != offre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OffreExists(offre.Id))
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
            ViewData["AgenceId"] = new SelectList(_context.Agences, "Id", "Adresse", offre.AgenceId);
            return View(offre);
        }

        // GET: Offres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offre = await _context.Offres
                .Include(o => o.Agence)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offre == null)
            {
                return NotFound();
            }

            return View(offre);
        }

        // POST: Offres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offre = await _context.Offres.FindAsync(id);
            if (offre != null)
            {
                _context.Offres.Remove(offre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // Méthode pour récupérer la liste des agences en JSON
        [HttpGet]
        public async Task<IActionResult> GetAgences()
        {
            var agences = await _context.Agences.Select(a => new { a.Id, a.Ninea }).ToListAsync();
            return Json(agences);
        }
        private bool OffreExists(int id)
        {
            return _context.Offres.Any(e => e.Id == id);
        }
    }
}
