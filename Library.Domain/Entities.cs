// Library.Domain/Entities.cs
namespace Library.Domain;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int PageCount { get; set; }
    public string Excerpt { get; set; } = "";
    public DateTime PublishDate { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public int IdBook { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
}