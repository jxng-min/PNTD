using System.Collections.Generic;

namespace PNTD
{
    public class WaveContext
    {
        public string WaveId { get; }
        public float EndDelay { get; }
        public IReadOnlyList<TurnContext> Turns { get; }

        public WaveContext(string waveId, float endDelay, IReadOnlyList<TurnContext> turnContexts)
        {
            WaveId = waveId;
            EndDelay = endDelay;
            Turns = turnContexts;
        }
    }
}