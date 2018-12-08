using Microsoft.AspNetCore.Mvc;
using Security.Attributes;

namespace Security.Controllers
{
    [UserIdentity]
    [Produces("application/json")]
    [ApiController]
    public class SecurityController : ControllerBase {}
}
