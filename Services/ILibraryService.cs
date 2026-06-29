using LibraryApp.Models;

namespace LibraryApp.Services;

public interface ILibraryService
{
    // Authors
    Task<IEnumerable<Author>> GetAuthorsAsync();
    Task<Author?> GetAuthorByIdAsync(int id);

    // Books
    Task<IEnumerable<Book>> GetBooksAsync();
    Task<Book?> GetBookWithDetailsAsync(int id);
    Task AddBookAsync(Book book);
    Task<bool> BookExistsAsync(int id);

    // Borrowings
    Task<bool> AddBorrowingAsync(Borrowing borrowing);
}
