using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Interface;
using RockPaperScissors.Model.Transfer.Dto;
using Mapster;
using RockPaperScissors.Db.Repository.Interface;
using RockPaperScissors.Model.Entity;

namespace RockPaperScissors.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController(IUserRepository repository) : ControllerBase
    {
        /// <summary>
        /// Создать пользователя
        /// </summary>
        /// <returns>Созданный пользователь транзакции</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        public async Task<IActionResult> Create(string username)
        {
            var result = await repository.CreateUser(username, HttpContext.RequestAborted);

            return Ok(result);
        }
        
        /// <summary>
        /// Пополнение баланса
        /// </summary>
        /// <returns>Статус транзакции</returns>
        [HttpPost("recharge")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<IActionResult> RechargeBalance(Guid userId, decimal amount)
        {
            var result = await repository.UpdateUserBalance(userId, amount,  HttpContext.RequestAborted);

            return Ok(result);
        }
    }
}
