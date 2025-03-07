using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VoyageAgence.Data;
using VoyageAgence.Models;
using X.PagedList.Extensions;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Layout.Properties;


namespace VoyageAgence.Controllers
{
    public class VoyagesController : Controller
    {
        private readonly ApplicationDbContext _context;
    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
    public VoyagesController(ApplicationDbContext context) 
        {
            _context = context;
        }

        // GET: Voyages
        public async Task<IActionResult> Index(int? page)
        {
            var applicationDbContext = _context.Voyages.Include(v => v.Chauffeur).Include(v => v.Flotte);
            int pageSize = 5;
            int pageNumber = page ?? 1; // Numéro de page actuel (1 par défaut)

            var voyages = _context.Voyages
                .Include(v => v.Chauffeur)
                .Include(v => v.Flotte)
                .OrderBy(v => v.DateDepart)
                .ToPagedList(pageNumber, pageSize);
            return View(voyages);
        }

        // GET: Voyages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voyage = await _context.Voyages
                .Include(v => v.Chauffeur)
                .Include(v => v.Flotte)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (voyage == null)
            {
                return NotFound();
            }

            return View(voyage);
        }

        // GET: Voyages/Create
        public IActionResult Create()
        {
            ViewData["ChauffeurId"] = new SelectList(_context.Chauffeurs, "Id", "Nom");
            ViewData["FlotteId"] = new SelectList(_context.Flottes, "Id", "Matricule");
            return View();
        }

        // POST: Voyages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,DateDepart,DateRetour,Prix,ChauffeurId,FlotteId")] Voyage voyage)
        {
            try
            {
                _context.Add(voyage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log l'erreur ou affichez-la pour le débogage
                Console.WriteLine(ex.Message);
                ModelState.AddModelError(string.Empty, "Une erreur s'est produite lors de l'enregistrement du voyage.");
                ViewData["ChauffeurId"] = new SelectList(_context.Chauffeurs, "Id", "Nom", voyage.ChauffeurId);
                ViewData["FlotteId"] = new SelectList(_context.Flottes, "Id", "Matricule", voyage.FlotteId);
                return View(voyage);
            }

        }

        // GET: Voyages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voyage = await _context.Voyages.FindAsync(id);
            if (voyage == null)
            {
                return NotFound();
            }
            ViewData["ChauffeurId"] = new SelectList(_context.Chauffeurs, "Id", "Nom", voyage.ChauffeurId);
            ViewData["FlotteId"] = new SelectList(_context.Flottes, "Id", "Matricule", voyage.FlotteId);
            return View(voyage);
        }

        // POST: Voyages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,DateDepart,DateRetour,Prix,ChauffeurId,FlotteId")] Voyage voyage)
        {
            if (id != voyage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    voyage.DateDepart = DateTime.SpecifyKind(voyage.DateDepart, DateTimeKind.Utc);
                    voyage.DateRetour = DateTime.SpecifyKind(voyage.DateRetour, DateTimeKind.Utc);
                    _context.Update(voyage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoyageExists(voyage.Id))
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
            ViewData["ChauffeurId"] = new SelectList(_context.Chauffeurs, "Id", "Nom", voyage.ChauffeurId);
            ViewData["FlotteId"] = new SelectList(_context.Flottes, "Id", "Matricule", voyage.FlotteId);
            return View(voyage);
        }

        // GET: Voyages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voyage = await _context.Voyages
                .Include(v => v.Chauffeur)
                .Include(v => v.Flotte)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (voyage == null)
            {
                return NotFound();
            }

            return View(voyage);
        }

        // POST: Voyages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voyage = await _context.Voyages.FindAsync(id);
            if (voyage != null)
            {
                _context.Voyages.Remove(voyage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoyageExists(int id)
        {
            return _context.Voyages.Any(e => e.Id == id);
        }

        //Export to PDF

        public IActionResult ExportVoyagePdf()
        {
            try
            {
                // Créer un flux mémoire pour stocker le PDF
                MemoryStream stream = new MemoryStream();
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Définir une police en gras pour le titre
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Ajouter un titre au PDF
                document.Add(new Paragraph("Liste des Voyages")
                    .SetFont(boldFont)
                    .SetFontSize(18)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                // Récupérer les voyages avec les données associées (Chauffeur et Flotte)
                var voyages = _context.Voyages
                    .Include(v => v.Chauffeur)
                    .Include(v => v.Flotte)
                    .ToList();

                // Créer un tableau avec 5 colonnes
                Table table = new Table(5); // 5 colonnes : Prix, Chauffeur, Date départ, Date retour, Flotte
                table.SetWidth(UnitValue.CreatePercentValue(100)); // Largeur du tableau à 100%

                // Ajouter les en-têtes du tableau
                table.AddHeaderCell("Prix").SetFont(boldFont);
                table.AddHeaderCell("Chauffeur").SetFont(boldFont);
                table.AddHeaderCell("Date de départ").SetFont(boldFont);
                table.AddHeaderCell("Date de retour").SetFont(boldFont);
                table.AddHeaderCell("Flotte").SetFont(boldFont);

                // Remplir le tableau avec les données des voyages
                foreach (var voyage in voyages)
                {
                    table.AddCell(voyage.Prix.ToString());
                    table.AddCell(voyage.Chauffeur?.Nom ?? "N/A"); // Gestion des valeurs nulles
                    table.AddCell(voyage.DateDepart.ToString("dd/MM/yyyy"));
                    table.AddCell(voyage.DateRetour.ToString("dd/MM/yyyy"));
                    table.AddCell(voyage.Flotte?.Matricule ?? "N/A"); // Gestion des valeurs nulles
                }

                // Ajouter le tableau au document
                document.Add(table);

                // Fermer le document
                document.Close();

                // Retourner le fichier PDF en tant que réponse
                return File(stream.ToArray(), "application/pdf", "VoyagesReport.pdf");
            }
            catch (Exception ex)
            {
                // Gérer les erreurs et retourner une réponse d'erreur
                return StatusCode(500, $"Une erreur s'est produite lors de la génération du PDF : {ex.Message}");
            }
        }


    }
}
