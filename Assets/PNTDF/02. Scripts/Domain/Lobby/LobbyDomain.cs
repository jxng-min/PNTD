namespace PNTD
{
    public class LobbyDomain
    {
        public ShopSystem ShopSystem { get; }
        public ShuffleSystem ShuffleSystem { get; }
        public SynergySystem SynergySystem { get; }
        public PartySystem PartySystem { get; }

        public LobbyDomain(ShopSystem shopSystem, 
                           ShuffleSystem shuffleSystem,
                           SynergySystem synergySystem,
                           PartySystem partySystem)
        {
            ShopSystem = shopSystem;
            ShuffleSystem = shuffleSystem;
            SynergySystem = synergySystem;
            PartySystem = partySystem;
        }

        public void Initialize()
        {
            ShuffleSystem.Initialize();
            ShopSystem.Initialize();
            PartySystem.Initialize(null);
            SynergySystem.Initialize(PartySystem.HeroContexts);
        }
    }
}