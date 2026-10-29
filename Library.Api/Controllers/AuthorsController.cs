using Library.Application;
using Library.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController(AuthorService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) => 
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct) =>
        await service.GetByIdAsync(id, ct) is { } a ? Ok(a) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Author author, CancellationToken ct)
    {
        var created = await service.CreateAsync(author, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Author author, CancellationToken ct) =>
        await service.UpdateAsync(id, author, ct) is { } a ? Ok(a) : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}

