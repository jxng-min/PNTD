namespace PNTD
{
    public class LobbyDomain
    {
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
                           LobbyVisibilitySystem visibilitySystem)
        {
            ShopSystem = shopSystem;
            ShuffleSystem = shuffleSystem;
            SynergySystem = synergySystem;
            PartySystem = partySystem;
            StatusSystem = statusSystem;
            VisibilitySystem = visibilitySystem;
        }

        public void Initialize()
        {
            ShuffleSystem.Initialize();
            ShopSystem.Initialize();
            PartySystem.Initialize(null);
            SynergySystem.Initialize(PartySystem.HeroContexts);
            StatusSystem.Initialize();
        }
    }
}
