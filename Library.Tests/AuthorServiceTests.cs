using Library.Application;
using Library.Domain;
using Moq;
using Xunit;

namespace Library.Tests;

public class AuthorServiceTests
{
    private readonly Mock<IAuthorRepository> _authorRepoMock = new();
    private readonly AuthorService _service;

    public AuthorServiceTests()
    {
        _service = new AuthorService(_authorRepoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_CalculatesDistinctBooksCountPerAuthorCorrectly()
    {
        // Arrange: John Doe has 2 different books (Book 1 and Book 2), Jane Smith has 1 book (Book 1)
        var authors = new List<Author>
        {
            new() { Id = 1, IdBook = 1, FirstName = "John", LastName = "Doe" },
            new() { Id = 2, IdBook = 2, FirstName = "John", LastName = "Doe" },
            new() { Id = 3, IdBook = 1, FirstName = "Jane", LastName = "Smith" }
        };

        _authorRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        // Act
        var result = await _service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);

        var johnDto1 = result.First(a => a.Id == 1);
        Assert.Equal(2, johnDto1.BooksCount);

        var johnDto2 = result.First(a => a.Id == 2);
        Assert.Equal(2, johnDto2.BooksCount);

        var janeDto = result.First(a => a.Id == 3);
        Assert.Equal(1, janeDto.BooksCount);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAuthorExists_ReturnsAuthor()
    {
        // Arrange
        var author = new Author { Id = 1, IdBook = 10, FirstName = "Eric", LastName = "Evans" };
        _authorRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        // Act
        var result = await _service.GetByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Eric", result.FirstName);
        Assert.Equal("Evans", result.LastName);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedAuthor()
    {
        // Arrange
        var author = new Author { FirstName = "Kent", LastName = "Beck", IdBook = 3 };
        _authorRepoMock
            .Setup(r => r.CreateAsync(author, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        // Act
        var result = await _service.CreateAsync(author, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Kent", result.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryAndReturnsResult()
    {
        // Arrange
        _authorRepoMock
            .Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result);
    }
}

