using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LibraryApp.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tytuł jest wymagany.")]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "ISBN jest wymagany.")]
    [MaxLength(20)]
    public string Isbn { get; set; } = string.Empty;

    [Range(1901, 9999, ErrorMessage = "Rok wydania musi być większy niż 1900.")]
    public int PublishedYear { get; set; }

    public int AuthorId { get; set; }
    [ValidateNever]
    public Author Author { get; set; } = null!;

    public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
}
