using System.Collections.Generic;

namespace PNTD
{
    public class StageContext
    {
        public string StageId { get; }
        public int MaxLife { get; }
        public int RewardGold { get; }
        public string MapId { get; }
        public IReadOnlyList<WaveContext> Waves { get; }

        public StageContext(string stageId, int maxLife, int rewardGold, string mapId, IReadOnlyList<WaveContext> waveContexts)
        {
            StageId = stageId;
            MaxLife = maxLife;
            RewardGold = rewardGold;
            MapId = mapId;
            Waves = waveContexts;
        }
    }
}