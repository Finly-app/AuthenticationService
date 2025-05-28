using Authentication.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("Policies")]
public class PolicyController : ControllerBase {
    private readonly IPolicyService _policyService;
    public PolicyController(IPolicyService policyService) {
        _policyService = policyService;
    }
    //TODO: Improve controller with FROM BOdy, look at user service
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _policyService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id) {
        var Policy = await _policyService.GetByIdAsync(id);
        return Policy == null ? NotFound() : Ok(Policy);
    }

    [HttpPost]
    public async Task<IActionResult> Create(PolicyDto dto) {
        await _policyService.CreateAsync(dto);
        return Created();
    }

    [HttpPost("/bulk")]
    public async Task<IActionResult> BulkCreatePolicies([FromBody] BulkPolicyCreateDto dto) {
        await _policyService.BulkCreatePolicies(dto);

        return Ok();
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, PolicyDto dto) {
        if (id != dto.Id) return BadRequest();
        await _policyService.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) {
        await _policyService.DeleteAsync(id);
        return NoContent();
    }
}
