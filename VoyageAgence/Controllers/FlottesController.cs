using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoyageAgence.Data;
using VoyageAgence.Models;

namespace VoyageAgence.Controllers
{
    public class FlottesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlottesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Flottes
        public IActionResult Index()
        {
            // Vue qui contient le HTML et JS, pas besoin de charger la liste ici
            return View();
        }

        // GET: Flottes/List
        [HttpGet]
        public async Task<IActionResult> List(string searchTerm = null)
        {
            try
            {
                var query = _context.Flottes.AsQueryable();

                // Si un terme de recherche est fourni, filtrer les résultats
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(f =>
                        f.Type.ToLower().Contains(searchTerm) ||
                        f.Matricule.ToLower().Contains(searchTerm));
                }

                var flottes = await query.ToListAsync();
                return Json(flottes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne: {ex.Message}");
            }
        }

        // GET: Flottes/Search (pour l'autocomplétion)
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>());
            }

            try
            {
                term = term.ToLower();

                // Rechercher dans les types et matricules
                var results = await _context.Flottes
                    .Where(f => f.Type.ToLower().Contains(term) || f.Matricule.ToLower().Contains(term))
                    .Select(f => new
                    {
                        id = f.Id,
                        value = f.Matricule,
                        label = $"{f.Type} - {f.Matricule}",
                        type = f.Type,
                        disponible = f.EstDisponible
                    })
                    .Take(10) // Limiter à 10 résultats
                    .ToListAsync();

                return Json(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la recherche: {ex.Message}");
            }
        }

        // GET: Flottes/Details/5 (pour l'AJAX)
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest("L'identifiant est requis");
            }

            try
            {
                var flotte = await _context.Flottes.FirstOrDefaultAsync(m => m.Id == id);

                if (flotte == null)
                {
                    return NotFound($"Flotte avec ID {id} non trouvée");
                }

                return Json(flotte);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération: {ex.Message}");
            }
        }

        // POST: Flottes/Create (AJAX)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Flotte flotte)
        {
            if (flotte == null)
            {
                return BadRequest("Les données de la flotte sont requises");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Vérifier si le matricule existe déjà
                if (await _context.Flottes.AnyAsync(f => f.Matricule == flotte.Matricule))
                {
                    return BadRequest("Le matricule existe déjà");
                }

                _context.Add(flotte);
                await _context.SaveChangesAsync();

                return Ok(flotte);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la création: {ex.Message}");
            }
        }

        // PUT: Flottes/Edit (AJAX)
        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] Flotte flotte)
        {
            if (flotte == null)
            {
                return BadRequest("Les données de la flotte sont requises");
            }

            if (flotte.Id <= 0)
            {
                return BadRequest("ID de flotte invalide");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingFlotte = await _context.Flottes.FindAsync(flotte.Id);

                if (existingFlotte == null)
                {
                    return NotFound($"Flotte avec ID {flotte.Id} non trouvée");
                }

                // Vérifier si le matricule est déjà utilisé par une autre flotte
                if (await _context.Flottes.AnyAsync(f => f.Matricule == flotte.Matricule && f.Id != flotte.Id))
                {
                    return BadRequest("Le matricule est déjà utilisé par une autre flotte");
                }

                // Mettre à jour les propriétés
                existingFlotte.Type = flotte.Type;
                existingFlotte.Matricule = flotte.Matricule;
                existingFlotte.EstDisponible = flotte.EstDisponible;

                await _context.SaveChangesAsync();

                return Ok(existingFlotte);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlotteExists(flotte.Id))
                {
                    return NotFound($"Flotte avec ID {flotte.Id} non trouvée");
                }
                return StatusCode(500, "Erreur de concurrence lors de la mise à jour");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la mise à jour: {ex.Message}");
            }
        }

        // POST: Flottes/Delete (AJAX)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID de flotte invalide");
            }

            try
            {
                var flotte = await _context.Flottes.FindAsync(id);

                if (flotte == null)
                {
                    return NotFound($"Flotte avec ID {id} non trouvée");
                }

                // Vérifier si la flotte est utilisée ailleurs avant de la supprimer
                // Exemple: if (await _context.Voyages.AnyAsync(v => v.FlotteId == id))
                //          {
                //              return BadRequest("Impossible de supprimer cette flotte car elle est utilisée dans des voyages");
                //          }

                _context.Flottes.Remove(flotte);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression: {ex.Message}");
            }
        }

        private bool FlotteExists(int id)
        {
            return _context.Flottes.Any(e => e.Id == id);
        }
    }
}