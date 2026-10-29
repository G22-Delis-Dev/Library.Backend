using Library.Application;
using Library.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(BookService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) => 
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct) =>
        await service.GetByIdAsync(id, ct) is { } b ? Ok(b) : NotFound();

    [HttpGet("{id:int}/authors")]
    public async Task<IActionResult> GetAuthors(int id, CancellationToken ct) => 
        Ok(await service.GetAuthorsAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Book book, CancellationToken ct)
    {
        var created = await service.CreateAsync(book, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Book book, CancellationToken ct) =>
        await service.UpdateAsync(id, book, ct) is { } b ? Ok(b) : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
