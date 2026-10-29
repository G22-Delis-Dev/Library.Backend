// Library.Infrastructure/ExternalApiRepository.cs
using System.Net;
using System.Net.Http.Json;
using Library.Application;
using Library.Domain;

namespace Library.Infrastructure;

public abstract class ExternalApiRepository<T>(HttpClient http, string resource) : IRepository<T> where T : class
{
    protected readonly HttpClient Http = http;
    protected readonly string Resource = resource;

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct) =>
        await Http.GetFromJsonAsync<List<T>>(Resource, ct) ?? [];

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct)
    {
        var res = await Http.GetAsync($"{Resource}/{id}", ct);
        if (res.StatusCode == HttpStatusCode.NotFound) return null;
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<T>(ct);
    }

    public async Task<T> CreateAsync(T entity, CancellationToken ct)
    {
        var res = await Http.PostAsJsonAsync(Resource, entity, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<T>(ct))!;
    }

    public async Task<T?> UpdateAsync(int id, T entity, CancellationToken ct)
    {
        var res = await Http.PutAsJsonAsync($"{Resource}/{id}", entity, ct);
        if (res.StatusCode == HttpStatusCode.NotFound) return null;
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<T>(ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var res = await Http.DeleteAsync($"{Resource}/{id}", ct);
        if (res.StatusCode == HttpStatusCode.NotFound) return false;
        res.EnsureSuccessStatusCode();
        return true;
    }
}

public class BookRepository(HttpClient http) : ExternalApiRepository<Book>(http, "Books"), IBookRepository;

public class AuthorRepository(HttpClient http) : ExternalApiRepository<Author>(http, "Authors"), IAuthorRepository
{
    public async Task<IReadOnlyList<Author>> GetByBookAsync(int bookId, CancellationToken ct) =>
        await Http.GetFromJsonAsync<List<Author>>($"Authors/authors/books/{bookId}", ct) ?? [];
}