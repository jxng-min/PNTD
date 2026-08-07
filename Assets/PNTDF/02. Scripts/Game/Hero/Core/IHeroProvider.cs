using System.Collections.Generic;

namespace PNTD
{
    public interface IHeroProvider
    {
        IReadOnlyCollection<Hero> Heroes { get; }
    }
}
