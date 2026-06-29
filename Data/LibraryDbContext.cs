using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Borrowing> Borrowings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API – Author
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(a => a.LastName).IsRequired().HasMaxLength(100);
            entity.Ignore(a => a.FullName);
        });

        // Fluent API – Book
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).IsRequired().HasMaxLength(300);
            entity.Property(b => b.Isbn).IsRequired().HasMaxLength(20);

            // jeden autor → wiele książek
            entity.HasOne(b => b.Author)
                  .WithMany(a => a.Books)
                  .HasForeignKey(b => b.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Fluent API – Borrowing
        modelBuilder.Entity<Borrowing>(entity =>
        {
            entity.HasKey(br => br.Id);
            entity.Property(br => br.BorrowerName).IsRequired().HasMaxLength(200);

            // jedna książka → wiele wypożyczeń
            entity.HasOne(br => br.Book)
                  .WithMany(b => b.Borrowings)
                  .HasForeignKey(br => br.BookId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seeding – stałe Id, bo HasData tego wymaga
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, FirstName = "Adam", LastName = "Mickiewicz" },
            new Author { Id = 2, FirstName = "Bolesław", LastName = "Prus" },
            new Author { Id = 3, FirstName = "Henryk", LastName = "Sienkiewicz" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Pan Tadeusz", Isbn = "978-83-07-03173-2", PublishedYear = 1834, AuthorId = 1 },
            new Book { Id = 2, Title = "Dziady", Isbn = "978-83-07-03174-9", PublishedYear = 1823, AuthorId = 1 },
            new Book { Id = 3, Title = "Lalka", Isbn = "978-83-06-03290-1", PublishedYear = 1890, AuthorId = 2 },
            new Book { Id = 4, Title = "Faraon", Isbn = "978-83-06-03291-8", PublishedYear = 1897, AuthorId = 2 },
            new Book { Id = 5, Title = "Quo Vadis", Isbn = "978-83-07-03175-6", PublishedYear = 1896, AuthorId = 3 }
        );
    }
}
