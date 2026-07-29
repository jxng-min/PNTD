using System.Collections.Generic;

namespace PNTD
{
    public interface IEnemyProvider
    {
        IReadOnlyCollection<Enemy> AliveEnemies { get; }
    }
}