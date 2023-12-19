using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Application.Core;
using API.Extensions;

namespace API.Controllers
{
    [AllowAnonymous]
    [EnableCors("CorsPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    // [Route("api/[controller]/{bankid?}/{page?}/{pagesize?}")]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null) return NotFound();
            if (result.IsSuccess)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }
        protected ActionResult HandlePagedResult<T>(Result<PagedList<T>> result)
        {
            if (result == null) return NotFound();
            if (result.IsSuccess && result.Value != null)
            {
                Response.AddPaginationHeader(result.Value.CurrentPage, result.Value.PageSize,
                    result.Value.TotalCount, result.Value.TotalPages);
                return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }

    }
}
