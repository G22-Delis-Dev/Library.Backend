// Library.Infrastructure/DependencyInjection.cs
using Library.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection s, IConfiguration cfg)
    {
        var baseUrl = new Uri(cfg["ExternalApi:BaseUrl"]!);
        s.AddHttpClient<IBookRepository, BookRepository>(c => c.BaseAddress = baseUrl);
        s.AddHttpClient<IAuthorRepository, AuthorRepository>(c => c.BaseAddress = baseUrl);
        s.AddScoped<BookService>();
        s.AddScoped<AuthorService>();
        return s;
    }
}