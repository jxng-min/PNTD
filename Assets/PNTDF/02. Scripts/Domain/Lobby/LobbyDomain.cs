namespace PNTD
{
    public class LobbyDomain
    {
        public ShopSystem ShopSystem { get; }
        public ShuffleSystem ShuffleSystem { get; }

        public LobbyDomain(ShopSystem shopSystem, 
                           ShuffleSystem shuffleSystem)
        {
            ShopSystem = shopSystem;
            ShuffleSystem = shuffleSystem;
        }

        public void Initialize()
        {
            ShuffleSystem.Initialize();
            ShopSystem.Initialize();
        }
    }
}