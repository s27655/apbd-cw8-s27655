using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class LibraryService : ILibraryService
{
    private readonly LibraryDbContext _db;

    public LibraryService(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Author>> GetAuthorsAsync()
        => await _db.Authors
                    .Include(a => a.Books)
                    .OrderBy(a => a.LastName)
                    .ToListAsync();

    public async Task<Author?> GetAuthorByIdAsync(int id)
        => await _db.Authors.FindAsync(id);

    public async Task<IEnumerable<Book>> GetBooksAsync()
        => await _db.Books
                    .Include(b => b.Author)
                    .OrderBy(b => b.Title)
                    .ToListAsync();

    public async Task<Book?> GetBookWithDetailsAsync(int id)
        => await _db.Books
                    .Include(b => b.Author)
                    .Include(b => b.Borrowings.OrderByDescending(br => br.BorrowedAt))
                    .FirstOrDefaultAsync(b => b.Id == id);

    public async Task AddBookAsync(Book book)
    {
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> BookExistsAsync(int id)
        => await _db.Books.AnyAsync(b => b.Id == id);

    public async Task<bool> AddBorrowingAsync(Borrowing borrowing)
    {
        if (!await BookExistsAsync(borrowing.BookId))
            return false;

        borrowing.BorrowedAt = DateTime.Now;
        _db.Borrowings.Add(borrowing);
        await _db.SaveChangesAsync();
        return true;
    }
}
