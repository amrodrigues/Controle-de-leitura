using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadingTracker.Api.DTOs;
using ReadingTracker.Api.Services;

namespace ReadingTracker.Api.Controllers;

[ApiController]
[Route("api/books")]
[Authorize]
public class BookController(IBookService bookService) : ControllerBase
{
    /// <summary>Cadastra um livro para a usuária autenticada.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BookResponse>> Create([FromBody] BookRequest request, CancellationToken ct)
    {
        var created = await bookService.CreateAsync(User.GetUserId(), request, ct);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    /// <summary>Lista os livros cadastrados pela usuária.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<BookResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<BookResponse>>> GetAll(CancellationToken ct) =>
        Ok(await bookService.GetAllAsync(User.GetUserId(), ct));

    /// <summary>Atualiza os dados de um livro da usuária.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookResponse>> Update(Guid id, [FromBody] BookRequest request, CancellationToken ct) =>
        Ok(await bookService.UpdateAsync(User.GetUserId(), id, request, ct));

    /// <summary>Exclui um livro se ele não estiver na estante.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await bookService.DeleteAsync(User.GetUserId(), id, ct);
        return NoContent();
    }
}