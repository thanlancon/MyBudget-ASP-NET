using Application.Categories;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CategoriesController : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult> List([FromQuery] CategoryParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCategory(Guid id)
        {
            return HandleResult( await Mediator.Send(new Details.Query { Id = id }));
        }

        [HttpPost]
        public async Task<ActionResult> CreateCategory(Category category)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Category = category }));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> EditCategory(Guid id, Category category)
        {
            if(id!=category.Id)
            {
                return HandleResult(Result<string>.Failure(ResponseConstants.RequestMissMatch));
            }
            return HandleResult(await Mediator.Send(new Edit.Command { Category = category }));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}
