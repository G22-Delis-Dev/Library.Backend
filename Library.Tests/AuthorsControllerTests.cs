using Library.Api.Controllers;
using Library.Application;
using Library.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests;

public class AuthorsControllerTests
{
    private readonly Mock<IAuthorRepository> _authorRepoMock = new();
    private readonly AuthorsController _controller;

    public AuthorsControllerTests()
    {
        var service = new AuthorService(_authorRepoMock.Object);
        _controller = new AuthorsController(service);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithAuthors()
    {
        // Arrange
        var authors = new List<Author>
        {
            new() { Id = 1, IdBook = 1, FirstName = "Martin", LastName = "Fowler" }
        };
        _authorRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsAssignableFrom<IReadOnlyList<AuthorDto>>(okResult.Value);
        Assert.Single(dtos);
        Assert.Equal("Martin", dtos[0].FirstName);
    }

    [Fact]
    public async Task Get_WhenExists_ReturnsOkResult()
    {
        // Arrange
        var author = new Author { Id = 1, FirstName = "Martin", LastName = "Fowler" };
        _authorRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        // Act
        var result = await _controller.Get(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedAuthor = Assert.IsType<Author>(okResult.Value);
        Assert.Equal(1, returnedAuthor.Id);
    }

    [Fact]
    public async Task Get_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _authorRepoMock
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        // Act
        var result = await _controller.Get(999, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        // Arrange
        var author = new Author { Id = 2, FirstName = "Kent", LastName = "Beck", IdBook = 1 };
        _authorRepoMock
            .Setup(r => r.CreateAsync(author, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        // Act
        var result = await _controller.Create(author, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(AuthorsController.Get), createdResult.ActionName);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _authorRepoMock
            .Setup(r => r.DeleteAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(99, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

