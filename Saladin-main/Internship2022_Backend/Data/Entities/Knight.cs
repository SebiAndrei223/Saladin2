using System;

namespace Data.Entities
{
    public class Knight
    {
        public int KnightId { get; set; }
        public string Name { get; set; }
        public string DictionaryKnightTypeName { get; set; }
        public string LegionName { get; set; }
        public string BattleName { get; set; }
        public int CoinsAwardedPerBattle { get; set; }
    }
}
