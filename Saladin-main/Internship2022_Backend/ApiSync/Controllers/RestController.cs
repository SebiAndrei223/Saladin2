using ApiSync.Commands;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            string queryString = @"SELECT K.KnightId, K.Name, KT.DictionaryKnightTypeName , L.Name LegionName, B.Name BattleName,B.BattleId, KB.CoinsAwarded CoinsAwardedPerBattle
                                   FROM Knight K
                                   JOIN LEGION L on K.LegionId = L.LegionId
                                   JOIN DictionaryKnightType KT on K.DictionaryKnightTypeId = KT.DictionaryKnightTypeId
                                   LEFT JOIN KnightXBattle KB on K.KnightId = KB.KnightId
                                   JOIN Battle B on KB.BattleId = B.BattleId
                                   ORDER BY K.Name";

            List<Knight> drivers = await _dataService.GetKights(queryString, cancellationToken);

            return Ok(drivers);
        }

        [HttpPost]
        [Route("knights")]
        public async Task<IActionResult> ChangeCoinsAwardedPerBattle([FromBody] ChangeCoinsAwardedPerBattleCommand command, CancellationToken cancellationToken)
       {
            string commandString = @"UPDATE KnightXBattle SET CoinsAwarded = "+command.CoinsAwardedPerBattle+" where BattleId="+command.BattleId+" and KnightId="+command.KnightId+"";

            await _dataService.ChangeCoinsAwardedPerBattle(commandString, cancellationToken);
            string commandString2 = @"Update Knight SET DictionaryKnightTypeId=(select DictionaryKnightTypeId from DictionaryKnightType where (select Sum(CoinsAwarded) as Coins from Knight K join KnightXBattle KXB on KXB.KnightId=K.KnightId where K.KnightId="+command.KnightId+") between CoinsFrom and CoinsTo) where KnightId = " + command.KnightId+"";
            await _dataService.ChangeCoinsAwardedPerBattle(commandString2, cancellationToken);
            return Ok("Rank succesfuly updated!");
        }
    }
}
