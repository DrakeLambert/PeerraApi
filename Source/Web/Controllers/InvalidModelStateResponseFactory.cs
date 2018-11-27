using DrakeLambert.Peerra.WebApi.Web.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DrakeLambert.Peerra.WebApi.Web.Controllers
{
    public static class InvalidModelStateResponseFactory
    {
        public static IActionResult Handle(ActionContext context)
        {
            return new BadRequestObjectResult(new ErrorDto(context.ModelState));
        }
    }
}
