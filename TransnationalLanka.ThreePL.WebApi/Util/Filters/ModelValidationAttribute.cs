using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TransnationalLanka.ThreePL.Core.Exceptions;

namespace TransnationalLanka.ThreePL.WebApi.Util.Filters
{
    public class ModelValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (actionContext.ModelState.IsValid) return;

            var errors = actionContext.ModelState
                .SelectMany(f => f.Value.Errors);

            actionContext.Result = new BadRequestObjectResult(new
            {
                errors = errors.Select(e => new ErrorMessage(){ Code = string.Empty, Message = e.ErrorMessage})
            });

        }
    }
}
