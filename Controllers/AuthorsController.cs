using LibraryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

public class AuthorsController : Controller
{
    private readonly ILibraryService _service;

    public AuthorsController(ILibraryService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var authors = await _service.GetAuthorsAsync();
        return View(authors);
    }
}
