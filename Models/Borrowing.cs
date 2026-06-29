using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LibraryApp.Models;

public class Borrowing
{
    public int Id { get; set; }

    public int BookId { get; set; }
    [ValidateNever]
    public Book Book { get; set; } = null!;

    [Required(ErrorMessage = "Imię i nazwisko wypożyczającego jest wymagane.")]
    [MaxLength(200)]
    public string BorrowerName { get; set; } = string.Empty;

    public DateTime BorrowedAt { get; set; }

    public DateTime? ReturnedAt { get; set; }
}
