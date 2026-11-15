using Microsoft.AspNetCore.Mvc;

namespace SplitwiseClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        [HttpGet("{groupId}/balances")]
        public IActionResult GetBalances(int groupId) => Ok();

        [HttpPost("{groupId}/expenses")]
        public IActionResult AddExpense(int groupId) => Ok();

        [HttpGet("{groupId}/expenses")]
        public IActionResult GetExpenses(int groupId) => Ok();

        [HttpPost]
        public IActionResult CreateGroup() => Ok();

        [HttpGet]
        public IActionResult GetGroups() => Ok();

        [HttpGet("{id}")]
        public IActionResult GetGroupById(int id) => Ok();

        [HttpPost("{id}/members")]
        public IActionResult AddMember(int id) => Ok();

        [HttpPost("{groupId}/settlements")]
        public IActionResult AddSettlement(int groupId) => Ok();

        [HttpGet("{groupId}/settlements")]
        public IActionResult GetSettlements(int groupId) => Ok();
    }
}