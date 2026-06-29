using LibraryApp.Models;
using LibraryApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryApp.Controllers;

public class BorrowingsController : Controller
{
    private readonly ILibraryService _service;

    public BorrowingsController(ILibraryService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Create(int? bookId)
    {
        await PopulateBooksAsync(bookId);
        var borrowing = new Borrowing();
        if (bookId.HasValue)
            borrowing.BookId = bookId.Value;

        return View(borrowing);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Borrowing borrowing)
    {
        if (!ModelState.IsValid)
        {
            await PopulateBooksAsync(borrowing.BookId);
            return View(borrowing);
        }

        var success = await _service.AddBorrowingAsync(borrowing);
        if (!success)
        {
            ModelState.AddModelError("BookId", "Wybrana książka nie istnieje.");
            await PopulateBooksAsync(borrowing.BookId);
            return View(borrowing);
        }

        return RedirectToAction("Details", "Books", new { id = borrowing.BookId });
    }

    private async Task PopulateBooksAsync(int? selectedId = null)
    {
        var books = await _service.GetBooksAsync();
        ViewBag.Books = new SelectList(books, "Id", "Title", selectedId);
    }
}
