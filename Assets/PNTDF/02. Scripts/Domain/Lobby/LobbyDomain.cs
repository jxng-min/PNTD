using System.Collections.Generic;

namespace PNTD
{
    public class LobbyDomain
    {
        private readonly IEnumerable<HeroContext> _initialParty;

        public ShopSystem ShopSystem { get; }
        public ShuffleSystem ShuffleSystem { get; }
        public SynergySystem SynergySystem { get; }
        public PartySystem PartySystem { get; }
        public StatusSystem StatusSystem { get; }
        public LobbyVisibilitySystem VisibilitySystem { get; }

        public LobbyDomain(ShopSystem shopSystem,
                           ShuffleSystem shuffleSystem,
                           SynergySystem synergySystem,
                           PartySystem partySystem,
                           StatusSystem statusSystem,
                           LobbyVisibilitySystem visibilitySystem,
                           IEnumerable<HeroContext> initialParty = null)
        {
            ShopSystem = shopSystem;
            ShuffleSystem = shuffleSystem;
            SynergySystem = synergySystem;
            PartySystem = partySystem;
            StatusSystem = statusSystem;
            VisibilitySystem = visibilitySystem;
            _initialParty = initialParty;
        }

        public void Initialize()
        {
            ShuffleSystem.Initialize();
            ShopSystem.Initialize();
            PartySystem.Initialize(_initialParty);
            SynergySystem.Initialize(PartySystem.HeroContexts);
            StatusSystem.Initialize();
        }
    }
}
