using Microsoft.AspNetCore.Mvc;

namespace DrakeLambert.Peerra.WebApi.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public abstract class ApiControllerBase : ControllerBase
    { }
}
