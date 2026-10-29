using Library.Application;
using Library.Domain;
using Moq;
using Xunit;

namespace Library.Tests;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepoMock = new();
    private readonly Mock<IAuthorRepository> _authorRepoMock = new();
    private readonly BookService _service;

    public BookServiceTests()
    {
        _service = new BookService(_bookRepoMock.Object, _authorRepoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBooks()
    {
        // Arrange
        var books = new List<Book>
        {
            new() { Id = 1, Title = "Book 1", PageCount = 100 },
            new() { Id = 2, Title = "Book 2", PageCount = 200 }
        };
        _bookRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        // Act
        var result = await _service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Book 1", result[0].Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBookExists_ReturnsBook()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Domain-Driven Design", PageCount = 500 };
        _bookRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _service.GetByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Domain-Driven Design", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBookDoesNotExist_ReturnsNull()
    {
        // Arrange
        _bookRepoMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _service.GetByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedBook()
    {
        // Arrange
        var newBook = new Book { Title = "Refactoring", PageCount = 400 };
        var createdBook = new Book { Id = 10, Title = "Refactoring", PageCount = 400 };

        _bookRepoMock
            .Setup(r => r.CreateAsync(newBook, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdBook);

        // Act
        var result = await _service.CreateAsync(newBook, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Refactoring", result.Title);
    }

    [Fact]
    public async Task UpdateAsync_WhenFound_ReturnsUpdatedBook()
    {
        // Arrange
        var updateData = new Book { Id = 1, Title = "Updated Title", PageCount = 250 };
        _bookRepoMock
            .Setup(r => r.UpdateAsync(1, updateData, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateData);

        // Act
        var result = await _service.UpdateAsync(1, updateData, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
    }

    [Fact]
    public async Task DeleteAsync_WhenFound_ReturnsTrue()
    {
        // Arrange
        _bookRepoMock
            .Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetAuthorsAsync_ReturnsAuthorsForBook()
    {
        // Arrange
        var authors = new List<Author>
        {
            new() { Id = 1, IdBook = 5, FirstName = "Martin", LastName = "Fowler" }
        };
        _authorRepoMock
            .Setup(r => r.GetByBookAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        // Act
        var result = await _service.GetAuthorsAsync(5, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Martin", result[0].FirstName);
    }
}

