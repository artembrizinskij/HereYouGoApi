using Microsoft.AspNetCore.Mvc;
using Security.Attributes;

namespace Security.Controllers
{
    [UserIdentity]
    [Produces("application/json")]
    public class SecurityController : Controller {}
}
