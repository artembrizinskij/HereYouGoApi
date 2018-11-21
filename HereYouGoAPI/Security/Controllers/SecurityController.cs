using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Security.Attributes;

namespace Security.Controllers
{
    [UserIdentity]
    [Produces("application/json")]
    public class SecurityController : Controller
    {
    }
}
