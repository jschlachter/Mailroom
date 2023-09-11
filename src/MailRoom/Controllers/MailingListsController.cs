using Microsoft.AspNetCore.Mvc;

namespace Mailroom.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MailingListsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAsync()
    {
        throw new NotImplementedException("Method not implmented");
    }

    [HttpGet("{id}")]
    public IActionResult GetAsync(int id)
    {
        throw new NotImplementedException("Method not implemented");
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAsync(int id)
    {
        throw new NotImplementedException("Method not implemented");
    }
}