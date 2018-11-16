using Microsoft.AspNetCore.Mvc;

namespace Latsic.IdApi1.Controllers
{
  [Route("/")]
  [ApiController]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class RedirectController : ControllerBase
  {
    [HttpGet]
    [ProducesResponseType(302)]
    public ActionResult<LocalRedirectResult> RedirectToSwagger()
    {
      return Redirect("~/swagger");
    }
  }
}