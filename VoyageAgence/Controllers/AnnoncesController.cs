using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VoyageAgence.Data;
using VoyageAgence.Models;

namespace VoyageAgence.Controllers
{
    public class AnnoncesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnnoncesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Annonces
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Annonce.Include(a => a.Flotte);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Annonces/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annonce = await _context.Annonce
                .Include(a => a.Flotte)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (annonce == null)
            {
                return NotFound();
            }

            return View(annonce);
        }

        // GET: Annonces/Create
        public IActionResult Create()
        {
            ViewData["FlotteId"] = new SelectList(_context.Flottes, "Id", "Matricule");
            return View();
        }

        // POST: Annonces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Statut,Prix,DateDepart,DateArrivee,LocaliteDepart,FlotteId")] Annonce annonce)
        {
            if (ModelState.IsValid)
            {
                _context.Add(annonce);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlotteId"] = new SelectList(_context.Flottes, "Id", "Matricule", annonce.FlotteId);
            return View(annonce);
        }

        // GET: Annonces/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annonce = await _context.Annonce.FindAsync(id);
            if (annonce == null)
            {
                return NotFound();
            }
            ViewData["FlotteId"] = new SelectList(_context.Flottes, "Id", "Matricule", annonce.FlotteId);
            return View(annonce);
        }

        // POST: Annonces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Statut,Prix,DateDepart,DateArrivee,LocaliteDepart,FlotteId")] Annonce annonce)
        {
            if (id != annonce.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(annonce);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnonceExists(annonce.Id))
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
            ViewData["FlotteId"] = new SelectList(_context.Flottes, "Id", "Matricule", annonce.FlotteId);
            return View(annonce);
        }

        // GET: Annonces/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annonce = await _context.Annonce
                .Include(a => a.Flotte)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (annonce == null)
            {
                return NotFound();
            }

            return View(annonce);
        }

        // POST: Annonces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var annonce = await _context.Annonce.FindAsync(id);
            if (annonce != null)
            {
                _context.Annonce.Remove(annonce);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnnonceExists(int id)
        {
            return _context.Annonce.Any(e => e.Id == id);
        }
    }
}
