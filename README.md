# Biblioteka uczelniana – APBD CW8

Prosta aplikacja ASP.NET Core MVC do obsługi książek, autorów i wypożyczeń w małej bibliotece uczelnianej.

---

## Jak uruchomić aplikację

**Wymagania:** .NET 10 SDK

```bash
# 1. Przejdź do katalogu projektu
cd apbd-cw8-s27655

# 2. Zastosuj migrację (tworzy plik library.db)
dotnet ef database update

# 3. Uruchom aplikację
dotnet run
```

Aplikacja będzie dostępna pod adresem `https://localhost:5001` (lub `http://localhost:5000`).

---

## Jak utworzyć lub odtworzyć bazę

```bash
# Usuń plik bazy (jeśli istnieje) i utwórz od nowa:
rm library.db        # lub Delete-Item library.db w PowerShell
dotnet ef database update
```

Baza danych to plik **`library.db`** (SQLite) tworzony w katalogu roboczym aplikacji.

---

## Komenda tworzenia migracji

```bash
dotnet ef migrations add InitialCreate
```

Folder `Migrations/` zawiera wygenerowane pliki migracji.

---

## Gdzie jest DbContext

`Data/LibraryDbContext.cs`

Zawiera:
- `DbSet<Author>` – autorzy
- `DbSet<Book>` – książki
- `DbSet<Borrowing>` – wypożyczenia
- konfigurację relacji i seeding danych w metodzie `OnModelCreating`

Rejestracja w DI: `Program.cs`, linia z `AddDbContext<LibraryDbContext>`.

---

## Gdzie jest konfiguracja relacji (Fluent API)

`Data/LibraryDbContext.cs`, metoda `OnModelCreating`:

| Relacja | Konfiguracja |
|---------|-------------|
| Autor → Książki | `HasOne(b => b.Author).WithMany(a => a.Books).HasForeignKey(b => b.AuthorId)` |
| Książka → Wypożyczenia | `HasOne(br => br.Book).WithMany(b => b.Borrowings).HasForeignKey(br => br.BookId)` |

---

## Gdzie jest seeding

`Data/LibraryDbContext.cs`, metoda `OnModelCreating`, wywołania `HasData`:

- 3 autorzy (Mickiewicz, Prus, Sienkiewicz)
- 5 książek (Pan Tadeusz, Dziady, Lalka, Faraon, Quo Vadis)

Seeding przez `HasData` jest **idempotentny** – EF Core śledzi zastosowane migracje w tabeli `__EFMigrationsHistory` i nie powtarza ich przy kolejnych uruchomieniach.

---

## Odpowiedzi na pytania

### Co oznacza ORM i jaki problem rozwiązuje EF Core?

ORM (Object-Relational Mapper) to warstwa, która mapuje tabele relacyjnej bazy danych na klasy C# i odwrotnie. EF Core eliminuje konieczność ręcznego pisania SQL – programista operuje na obiektach (LINQ), a biblioteka generuje i wykonuje zapytania za niego. Rozwiązuje problem impedance mismatch: różnicy między modelem obiektowym a relacyjnym.

### Jaka jest rola DbContext?

`DbContext` to jednostka pracy (Unit of Work) i punkt wejścia do bazy danych. Zarządza połączeniem, śledzi zmiany w encjach (change tracking), udostępnia `DbSet<T>` dla każdej tabeli i koordynuje zapis (`SaveChanges`). Jeden `DbContext` = jedna transakcja logiczna.

### Czym DbSet różni się od zwykłej listy w C#?

`List<T>` przechowuje dane w pamięci. `DbSet<T>` to proxy do tabeli w bazie – zapytania LINQ są tłumaczone na SQL i wykonywane po stronie bazy. `DbSet` obsługuje lazy loading, change tracking i odroczone wykonanie. Dane trafiają do pamięci dopiero przy materializacji (`.ToList()`, `foreach`, itp.).

### Dlaczego DbContext w aplikacji webowej powinien być Scoped?

Każde żądanie HTTP powinno mieć własny `DbContext`, bo kontekst jest statefull (śledzi zmiany encji) i nie jest thread-safe. `Scoped` gwarantuje jeden egzemplarz na żądanie – wystarczająco długo, żeby obsłużyć transakcję, ale nie dłużej (co zapobiegałoby wyciekom pamięci i zakleszczeniom przy `Singleton`). `Transient` byłby też zły, bo przy wielu wstrzyknięciach w jednym żądaniu stracimy spójność change trackera.

### Co robi migracja EF Core?

Migracja to zapis różnicy między aktualnym modelem C# a stanem bazy danych. `dotnet ef migrations add` generuje plik C# z metodami `Up()` (zastosuj zmianę) i `Down()` (cofnij). `dotnet ef database update` wykonuje `Up()` wszystkich niezbędnych migracji i zapisuje ich identyfikatory w tabeli `__EFMigrationsHistory`.

### Dlaczego seeding powinien być idempotentny?

Aplikacja może być uruchamiana wielokrotnie (restart serwera, nowe deploymenty). Seeder, który nie sprawdza, czy dane już istnieją, zduplikuje rekordy przy każdym starcie – prowadząc do niespójnych danych i błędów. Idempotentny seeder (np. przez `HasData` lub sprawdzenie `Any()` przed `Add`) daje identyczny wynik bez względu na to, ile razy zostanie wywołany.

### Kiedy Code First jest dobrym wyborem, a kiedy lepiej rozważyć Database First?

**Code First** sprawdza się, gdy:
- projekt zaczyna się od zera i baza ma powstawać razem z kodem,
- zespół jest bardziej C#/OOP-centryczny niż DBA-centryczny,
- chcemy trzymać schemat bazy w repozytorium (migracje = historia zmian).

**Database First** jest lepszy, gdy:
- baza danych już istnieje (legacy system, zewnętrzne data warehouse),
- DBA zarządza schematem niezależnie od aplikacji,
- baza ma skomplikowane typy, widoki lub procedury, których EF Core nie odtworzy dokładnie.
