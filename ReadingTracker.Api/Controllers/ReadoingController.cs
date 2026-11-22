using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadingTracker.Api.Domain.Enums;
using ReadingTracker.Api.DTOs;
using ReadingTracker.Api.Services;

namespace ReadingTracker.Api.Controllers;

[ApiController]
[Route("api/readings")]
[Authorize]
public class ReadingController(IReadingService readingService) : ControllerBase
{
    /// <summary>Adiciona um livro à estante com status inicial QueroLer.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReadingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReadingResponse>> AddToShelf([FromBody] AddReadingRequest request, CancellationToken ct)
    {
        var created = await readingService.AddToShelfAsync(User.GetUserId(), request, ct);
        return CreatedAtAction(nameof(GetShelf), new { id = created.Id }, created);
    }

    /// <summary>Lista a estante da usuária, com filtros opcionais por status e ano de conclusão.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ReadingResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReadingResponse>>> GetShelf(
        [FromQuery] ReadingStatus? status,
        [FromQuery] int? year,
        CancellationToken ct) =>
        Ok(await readingService.GetShelfAsync(User.GetUserId(), status, year, ct));

    /// <summary>Avança o status da leitura (QueroLer -> Lendo -> Lido).</summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ReadingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadingResponse>> UpdateStatus(
        Guid id,
        [FromBody] UpdateReadingStatusRequest request,
        CancellationToken ct) =>
        Ok(await readingService.UpdateStatusAsync(User.GetUserId(), id, request, ct));

    /// <summary>Inicia uma nova leitura para um livro já concluído (releitura).</summary>
    [HttpPost("reread/{bookId:guid}")]
    [ProducesResponseType(typeof(ReadingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReadingResponse>> ReRead(Guid bookId, CancellationToken ct)
    {
        var created = await readingService.ReReadAsync(User.GetUserId(), bookId, ct);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    /// <summary>Retira uma leitura da estante.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(Guid id, CancellationToken ct)
    {
        await readingService.RemoveFromShelfAsync(User.GetUserId(), id, ct);
        return NoContent();
    }

    /// <summary>Cadastra ou atualiza a avaliação (nota 1 a 5 e comentário) de uma leitura concluída.</summary>
    [HttpPut("{id:guid}/review")]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewResponse>> UpsertReview(
        Guid id,
        [FromBody] UpsertReviewRequest request,
        CancellationToken ct) =>
        Ok(await readingService.UpsertReviewAsync(User.GetUserId(), id, request, ct));
}