using Microsoft.AspNetCore.Mvc;

namespace shockz.msa.fileManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DummyController : ControllerBase
{
  private readonly ILogger<DummyController> _logger;

  public DummyController(ILogger<DummyController> logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }
}
