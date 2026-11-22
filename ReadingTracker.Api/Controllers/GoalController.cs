using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadingTracker.Api.DTOs;
using ReadingTracker.Api.Services;

namespace ReadingTracker.Api.Controllers;

[ApiController]
[Route("api/goals")]
[Authorize]
public class GoalController(IGoalService goalService) : ControllerBase
{
    /// <summary>Define a meta anual de livros.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(GoalProgressResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GoalProgressResponse>> Create([FromBody] CreateGoalRequest request, CancellationToken ct)
    {
        var created = await goalService.CreateAsync(User.GetUserId(), request, ct);
        return CreatedAtAction(nameof(GetProgress), new { year = created.Year }, created);
    }

    /// <summary>Altera a meta de livros para um ano existente.</summary>
    [HttpPut("{year:int}")]
    [ProducesResponseType(typeof(GoalProgressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GoalProgressResponse>> Update(int year, [FromBody] UpdateGoalRequest request, CancellationToken ct) =>
        Ok(await goalService.UpdateAsync(User.GetUserId(), year, request, ct));

    /// <summary>Consulta o progresso da meta anual (lidos, percentual e média mensal restante).</summary>
    [HttpGet("{year:int}/progress")]
    [ProducesResponseType(typeof(GoalProgressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GoalProgressResponse>> GetProgress(int year, CancellationToken ct) =>
        Ok(await goalService.GetProgressAsync(User.GetUserId(), year, ct));
}