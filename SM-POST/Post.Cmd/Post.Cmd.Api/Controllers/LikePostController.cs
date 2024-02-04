using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;


namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LikePostController: ControllerBase
{
    private readonly ILogger<LikePostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public LikePostController(ICommandDispatcher commandDispatcher, ILogger<LikePostController> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    [HttpPut]
    public async Task<ActionResult> LikePostAsync(LikePostCommand command)
    {
        var id = Guid.NewGuid();
        try
        {
            command.Id = id;
            
            await _commandDispatcher.SendAsync(command);

            return Ok(new BaseResponse
            {
                Message = "Like post request completed successfully"
            });

        }
        catch (AggregateNotFoundException ex) 
        {
            _logger.Log(LogLevel.Warning, ex, "Could not retrieve aggregate, client passed an incorrect postId targetting the aggregate");
            return BadRequest(new BaseResponse
            {
                Message = ex.Message    
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Client made a bad request");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch(Exception ex) 
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to like a post";
            _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse
            {
                Id = id,
                Message = SAFE_ERROR_MESSAGE
            });
        }
    }
}
