using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;
using NotesApp.Models.ViewModels;

namespace NotesApp.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();

            return View(notes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new NoteViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsIncognito && string.IsNullOrWhiteSpace(model.PinCode))
                {
                    ModelState.AddModelError("PinCode", "PIN code is required for incognito notes");
                    return View(model);
                }

                var userId = _userManager.GetUserId(User);
                var note = new Note
                {
                    Title = model.Title,
                    Content = model.Content,
                    FontFamily = model.FontFamily,
                    TextColor = model.TextColor,
                    IsIncognito = model.IsIncognito,
                    PinCode = model.IsIncognito ? model.PinCode : null,
                    UserId = userId!,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Notes.Add(note);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Note created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound();
            }

            // If incognito, redirect to verify PIN first
            if (note.IsIncognito)
            {
                return RedirectToAction(nameof(VerifyPin), new { id = note.Id, action = "edit" });
            }

            var model = new NoteViewModel
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                FontFamily = note.FontFamily,
                TextColor = note.TextColor,
                IsIncognito = note.IsIncognito,
                PinCode = note.PinCode
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NoteViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var note = await _context.Notes
                    .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

                if (note == null)
                {
                    return NotFound();
                }

                if (model.IsIncognito && string.IsNullOrWhiteSpace(model.PinCode))
                {
                    ModelState.AddModelError("PinCode", "PIN code is required for incognito notes");
                    return View(model);
                }

                note.Title = model.Title;
                note.Content = model.Content;
                note.FontFamily = model.FontFamily;
                note.TextColor = model.TextColor;
                note.IsIncognito = model.IsIncognito;
                note.PinCode = model.IsIncognito ? model.PinCode : null;
                note.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Note updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound();
            }

            // If incognito, redirect to verify PIN first
            if (note.IsIncognito)
            {
                return RedirectToAction(nameof(VerifyPin), new { id = note.Id, action = "details" });
            }

            return View(note);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Note deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult VerifyPin(int id, string action = "details")
        {
            ViewBag.Action = action;
            return View(new VerifyPinViewModel { NoteId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPin(VerifyPinViewModel model, string action = "details")
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var note = await _context.Notes
                    .FirstOrDefaultAsync(n => n.Id == model.NoteId && n.UserId == userId);

                if (note == null)
                {
                    return NotFound();
                }

                if (note.PinCode == model.PinCode)
                {
                    // Store verified note ID in TempData
                    TempData["VerifiedNoteId"] = note.Id;

                    if (action == "edit")
                    {
                        var viewModel = new NoteViewModel
                        {
                            Id = note.Id,
                            Title = note.Title,
                            Content = note.Content,
                            FontFamily = note.FontFamily,
                            TextColor = note.TextColor,
                            IsIncognito = note.IsIncognito,
                            PinCode = note.PinCode
                        };
                        return View("Edit", viewModel);
                    }
                    else
                    {
                        return View("Details", note);
                    }
                }

                ModelState.AddModelError("PinCode", "Incorrect PIN code");
            }

            ViewBag.Action = action;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetIncognitoNotes()
        {
            var userId = _userManager.GetUserId(User);
            var incognitoNotes = await _context.Notes
                .Where(n => n.UserId == userId && n.IsIncognito)
                .Select(n => new { n.Id, n.Title, n.UpdatedAt })
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();

            return Json(incognitoNotes);
        }
    }
}
