using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Interface;
using RockPaperScissors.Model.Transfer.Dto;
using Mapster;
using RockPaperScissors.Db.Repository.Interface;
using RockPaperScissors.Model.Entity;
using RockPaperScissors.Model.Matches;
using RockPaperScissors.Model.Matches.Dto;

namespace RockPaperScissors.Controllers
{
    [ApiController]
    [Route("api/matches")]
    public class MatchesController(IMatchRepository matchRepository) : ControllerBase
    {
        /// <summary>
        /// Создание матча с определенной ставкой
        /// </summary>
        /// <returns>Статус матча</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MatchResponse))]
        public async Task<IActionResult> CreateMatch([FromBody] MatchRequestDto requestDto)
        {
            var result = await matchRepository.CreateMatch(requestDto.Adapt<Match>(), HttpContext.RequestAborted);

            return Ok(result);
        }
        
        /// <summary>
        /// Получить все матчи
        /// </summary>
        /// <returns>Список матчей</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Match>))]
        public async Task<IActionResult> GetMatchList()
        {
            var result = await matchRepository.GetAllMatches(HttpContext.RequestAborted);

            return Ok(result);
        }
    }
}
