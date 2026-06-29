using LibraryApp.Models;
using LibraryApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryApp.Controllers;

public class BooksController : Controller
{
    private readonly ILibraryService _service;

    public BooksController(ILibraryService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _service.GetBooksAsync();
        return View(books);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _service.GetBookWithDetailsAsync(id);
        if (book is null)
            return NotFound();

        return View(book);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateAuthorsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAuthorsAsync();
            return View(book);
        }

        await _service.AddBookAsync(book);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAuthorsAsync()
    {
        var authors = await _service.GetAuthorsAsync();
        ViewBag.Authors = new SelectList(authors, "Id", "FullName");
    }
}
