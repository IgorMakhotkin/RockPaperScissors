using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Interface;
using RockPaperScissors.Model.Transfer.Dto;
using Mapster;

namespace RockPaperScissors.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController(ITransactionsService transactionsService) : ControllerBase
    {
        /// <summary>
        /// Проведение денежной транзакции между двумя игроками
        /// </summary>
        /// <returns>Статус транзакции</returns>
        [HttpPost("transfer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TransactionResponse))]
        public async Task<IActionResult> Transfer([FromBody] TransactionRequestDto requestDto)
        {
            var result = await transactionsService.Transfer(requestDto.Adapt<Transaction>(), HttpContext.RequestAborted);

            return Ok(result);
        }
    }
}
