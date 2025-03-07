using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VoyageAgence.Data;
using VoyageAgence.Models;
using VoyageAgence.services;

namespace VoyageAgence.Controllers
{

    [Authorize]
    public class AgencesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly WhatsAppService _whatsAppService;

        public AgencesController(ApplicationDbContext context, WhatsAppService whatsAppService)
        {
            _context = context;
            _whatsAppService = whatsAppService;

        }

        // GET: Agences
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Agences.Include(a => a.Gestionnaire);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Agences/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agence = await _context.Agences
                .Include(a => a.Gestionnaire)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agence == null)
            {
                return NotFound();
            }

            return View(agence);
        }

        // GET: Agences/Create
        public IActionResult Create()
        {
            ViewData["GestionnaireId"] = new SelectList(_context.Gestionnaires, "Id", "Id");
            return View();
        }

        // POST: Agences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Adresse,Latitude,Longitude,RCCM,Ninea,Notes,GestionnaireId")] Agence agence)
        {
            if (ModelState.IsValid)
            {
                _context.Add(agence);
                await _context.SaveChangesAsync();
                string message = $"Bonjour {agence.Gestionnaire.Prenom} {agence.Gestionnaire.Nom}, votre agence a été crée !";

                var result = await _whatsAppService.SendWhatsAppMessageAsync(agence.Gestionnaire.NumTel, message);

                return RedirectToAction(nameof(Index));
            }
            ViewData["GestionnaireId"] = new SelectList(_context.Gestionnaires, "Id", "Id", agence.GestionnaireId);
            return View(agence);
        }

        // GET: Agences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agence = await _context.Agences.FindAsync(id);
            if (agence == null)
            {
                return NotFound();
            }
            ViewData["GestionnaireId"] = new SelectList(_context.Gestionnaires, "Id", "Id", agence.GestionnaireId);
            return View(agence);
        }

        // POST: Agences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Adresse,Latitude,Longitude,RCCM,Ninea,Notes,GestionnaireId")] Agence agence)
        {
            if (id != agence.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agence);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgenceExists(agence.Id))
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
            ViewData["GestionnaireId"] = new SelectList(_context.Gestionnaires, "Id", "Id", agence.GestionnaireId);
            return View(agence);
        }

        // GET: Agences/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agence = await _context.Agences
                .Include(a => a.Gestionnaire)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agence == null)
            {
                return NotFound();
            }

            return View(agence);
        }

        // POST: Agences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agence = await _context.Agences.FindAsync(id);
            if (agence != null)
            {
                _context.Agences.Remove(agence);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgenceExists(int id)
        {
            return _context.Agences.Any(e => e.Id == id);
        }


        public IActionResult ExportAgencesPdf()
        {
            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            MemoryStream stream = new MemoryStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            // Titre du PDF
            document.Add(new Paragraph("Liste des Agences").SetFont(boldFont).SetFontSize(18));

            // Récupérer les agences
            var agences = _context.Agences.Include(a => a.Gestionnaire).ToList();

            foreach (var agence in agences)
            {
                document.Add(new Paragraph($"📍 Adresse : {agence.Adresse}"));
                document.Add(new Paragraph($"📜 RCCM : {agence.RCCM}"));
                document.Add(new Paragraph($"🆔 Ninea : {agence.Ninea}"));
                document.Add(new Paragraph($"⭐ Notes : {agence.Notes}/5"));
                document.Add(new Paragraph($"👨 Gestionnaire : {agence.Gestionnaire?.Nom}"));
                document.Add(new Paragraph("-----------------------------------------"));
            }

            document.Close();

            return File(stream.ToArray(), "application/pdf", "AgencesReport.pdf");
        }
    }
}
