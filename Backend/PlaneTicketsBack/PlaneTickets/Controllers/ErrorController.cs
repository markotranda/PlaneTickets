using Microsoft.AspNetCore.Mvc;

namespace PlaneTickets.Controllers;

public class ErrorController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error()
    {
        return Problem();
    }
}