// Library.Application/Abstractions.cs
using Library.Domain;
namespace Library.Application;

public interface IRepository<T>
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct);
    Task<T?> GetByIdAsync(int id, CancellationToken ct);
    Task<T> CreateAsync(T entity, CancellationToken ct);
    Task<T?> UpdateAsync(int id, T entity, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}

public interface IBookRepository : IRepository<Book> { }

public interface IAuthorRepository : IRepository<Author>
{
    Task<IReadOnlyList<Author>> GetByBookAsync(int bookId, CancellationToken ct);
}

public record AuthorDto(int Id, int IdBook, string FirstName, string LastName, int BooksCount);