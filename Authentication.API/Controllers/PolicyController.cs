using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("Policies")]
public class PolicyController : ControllerBase {
    private readonly IPolicyService _PolicyService;
    public PolicyController(IPolicyService PolicyService) {
        _PolicyService = PolicyService;
    }
    //TODO: Improve controller with FROM BOdy, look at user service
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _PolicyService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id) {
        var Policy = await _PolicyService.GetByIdAsync(id);
        return Policy == null ? NotFound() : Ok(Policy);
    }

    [HttpPost]
    public async Task<IActionResult> Create(PolicyDto dto) {
        await _PolicyService.CreateAsync(dto);
        return Created();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, PolicyDto dto) {
        if (id != dto.Id) return BadRequest();
        await _PolicyService.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) {
        await _PolicyService.DeleteAsync(id);
        return NoContent();
    }
}
