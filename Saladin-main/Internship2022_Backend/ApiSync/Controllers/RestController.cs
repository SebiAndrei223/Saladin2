using ApiSync.Commands;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Internship2021_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestController : ControllerBase
    {
        private readonly DataService _dataService;

        public RestController(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        [Route("knights")]
        public async Task<IActionResult> GetKnights(CancellationToken cancellationToken)
        {
            string queryString = @"SELECT K.KnightId, K.Name, KT.DictionaryKnightTypeName , K.Name LegionName, B.Name BattleName, KB.CoinsAwarded CoinsAwardedPerBattle
                                   FROM Knight K
                                   JOIN LEGION L on K.LegionId = L.LegionId
                                   JOIN DictionaryKnightType KT on K.DictionaryKnightTypeId = KT.DictionaryKnightTypeId
                                   LEFT JOIN KnightXBattle KB on K.KnightId = KB.KnightId
                                   LEFT JOIN Battle B on KB.BattleId = B.BattleId
                                   ORDER BY K.Name";

            List<Knight> drivers = await _dataService.GetKights(queryString, cancellationToken);

            return Ok(drivers);
        }

        [HttpPost]
        [Route("knights")]
        public async Task<IActionResult> ChangeCoinsAwardedPerBattle([FromBody] ChangeCoinsAwardedPerBattleCommand command, CancellationToken cancellationToken)
       {
            string commandString = "";

            if (commandString == "")
                return NotFound("Must be implemented!");

            await _dataService.ChangeCoinsAwardedPerBattle(commandString, cancellationToken);

            return Ok("Rank succesfuly updated!");
        }
    }
}
