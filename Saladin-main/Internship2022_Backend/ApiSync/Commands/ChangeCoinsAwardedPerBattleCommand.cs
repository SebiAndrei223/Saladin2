using System;

namespace ApiSync.Commands
{
    public class ChangeCoinsAwardedPerBattleCommand
    {
        public int KnightId { get; set; }
        public int CoinsAwardedPerBattle { get; set; }
        public int BattleId { get; set; }
    }
}
