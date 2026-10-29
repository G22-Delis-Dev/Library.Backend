// Library.Application/Services.cs
using Library.Domain;
namespace Library.Application;

public class BookService(IBookRepository books, IAuthorRepository authors)
{
    public Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct) => books.GetAllAsync(ct);
    public Task<Book?> GetByIdAsync(int id, CancellationToken ct) => books.GetByIdAsync(id, ct);
    public Task<IReadOnlyList<Author>> GetAuthorsAsync(int bookId, CancellationToken ct) => authors.GetByBookAsync(bookId, ct);
    public Task<Book> CreateAsync(Book b, CancellationToken ct) => books.CreateAsync(b, ct);
    public Task<Book?> UpdateAsync(int id, Book b, CancellationToken ct) => books.UpdateAsync(id, b, ct);
    public Task<bool> DeleteAsync(int id, CancellationToken ct) => books.DeleteAsync(id, ct);
}

public class AuthorService(IAuthorRepository authors)
{
    public async Task<IReadOnlyList<AuthorDto>> GetAllAsync(CancellationToken ct)
    {
        var list = await authors.GetAllAsync(ct);
        var counts = list
            .GroupBy(a => $"{a.FirstName} {a.LastName}".Trim().ToLowerInvariant())
            .ToDictionary(g => g.Key, g => g.Select(a => a.IdBook).Distinct().Count());

        return list.Select(a => new AuthorDto(a.Id, a.IdBook, a.FirstName, a.LastName,
            counts[$"{a.FirstName} {a.LastName}".Trim().ToLowerInvariant()])).ToList();
    }

    public Task<Author?> GetByIdAsync(int id, CancellationToken ct) => authors.GetByIdAsync(id, ct);
    public Task<Author> CreateAsync(Author a, CancellationToken ct) => authors.CreateAsync(a, ct);
    public Task<Author?> UpdateAsync(int id, Author a, CancellationToken ct) => authors.UpdateAsync(id, a, ct);
    public Task<bool> DeleteAsync(int id, CancellationToken ct) => authors.DeleteAsync(id, ct);
}