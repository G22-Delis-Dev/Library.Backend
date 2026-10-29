using Library.Api.Controllers;
using Library.Application;
using Library.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests;

public class BooksControllerTests
{
    private readonly Mock<IBookRepository> _bookRepoMock = new();
    private readonly Mock<IAuthorRepository> _authorRepoMock = new();
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        var service = new BookService(_bookRepoMock.Object, _authorRepoMock.Object);
        _controller = new BooksController(service);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithBooks()
    {
        // Arrange
        _bookRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book> { new() { Id = 1, Title = "Test Book" } });

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var books = Assert.IsAssignableFrom<IReadOnlyList<Book>>(okResult.Value);
        Assert.Single(books);
    }

    [Fact]
    public async Task Get_WhenBookExists_ReturnsOkResult()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Clean Architecture" };
        _bookRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _controller.Get(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBook = Assert.IsType<Book>(okResult.Value);
        Assert.Equal(1, returnedBook.Id);
    }

    [Fact]
    public async Task Get_WhenBookDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        _bookRepoMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _controller.Get(99, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var inputBook = new Book { Title = "New Book", PageCount = 150 };
        var createdBook = new Book { Id = 5, Title = "New Book", PageCount = 150 };
        _bookRepoMock
            .Setup(r => r.CreateAsync(inputBook, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdBook);

        // Act
        var result = await _controller.Create(inputBook, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(BooksController.Get), createdResult.ActionName);
        var returnVal = Assert.IsType<Book>(createdResult.Value);
        Assert.Equal(5, returnVal.Id);
    }

    [Fact]
    public async Task Delete_WhenFound_ReturnsNoContent()
    {
        // Arrange
        _bookRepoMock
            .Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _bookRepoMock
            .Setup(r => r.DeleteAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(99, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

