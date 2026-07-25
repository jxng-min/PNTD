namespace PNTD
{
    public class LobbyDomain
    {
        public ShopSystem ShopSystem { get; }
        public ShuffleSystem ShuffleSystem { get; }
        public SynergySystem SynergySystem { get; }

        public LobbyDomain(ShopSystem shopSystem, 
                           ShuffleSystem shuffleSystem,
                           SynergySystem synergySystem)
        {
            ShopSystem = shopSystem;
            ShuffleSystem = shuffleSystem;
            SynergySystem = synergySystem;
        }

        public void Initialize()
        {
            ShuffleSystem.Initialize();
            ShopSystem.Initialize();
        }
    }
}