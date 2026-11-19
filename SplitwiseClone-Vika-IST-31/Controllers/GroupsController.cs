using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitwiseClone.DTOs;
using SplitwiseClone.Services;

namespace SplitwiseClone.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GroupsController(IGroupService groupService, IBalanceService balanceService) : ControllerBase
{
    [HttpPost]
    public IActionResult CreateGroup([FromBody] CreateGroupDto dto)
    {
        var group = groupService.CreateGroup(dto, GetCurrentUserId());
        return CreatedAtAction(nameof(GetGroupById), new { id = group.Id }, group);
    }

    [HttpGet]
    public IActionResult GetGroups()
        => Ok(groupService.GetUserGroups(GetCurrentUserId()));

    [HttpGet("{id:int}")]
    public IActionResult GetGroupById(int id)
        => Ok(groupService.GetGroupById(id, GetCurrentUserId()));

    [HttpPost("{id:int}/members")]
    public IActionResult AddMember(int id, [FromBody] AddMemberDto dto)
    {
        groupService.AddMember(id, dto.UserId, GetCurrentUserId());
        return NoContent();
    }

    [HttpPost("{groupId:int}/expenses")]
    public IActionResult AddExpense(int groupId, [FromBody] CreateExpenseDto dto)
        => Ok(groupService.AddExpense(groupId, dto, GetCurrentUserId()));

    [HttpGet("{groupId:int}/balances")]
    public IActionResult GetBalances(int groupId)
        => Ok(balanceService.GetGroupBalances(groupId, GetCurrentUserId()));

    [HttpPost("{groupId:int}/settlements")]
    public IActionResult SettleUp(int groupId, [FromBody] CreateSettlementDto dto)
        => Ok(groupService.SettleUp(groupId, dto, GetCurrentUserId()));

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(claim))
            throw new UnauthorizedAccessException("User ID claim is missing.");

        if (int.TryParse(claim, out var userId))
            return userId;

        if (Guid.TryParse(claim, out var guid))
            return BitConverter.ToInt32(guid.ToByteArray(), 0) & 0x7FFFFFFF;

        throw new UnauthorizedAccessException("Invalid User ID claim.");
    }
}