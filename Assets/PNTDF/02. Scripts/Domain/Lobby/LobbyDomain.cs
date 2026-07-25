namespace PNTD
{
    public class LobbyDomain
    {
        public ShopSystem ShopSystem { get; }

        public LobbyDomain(ShopSystem shopSystem)
        {
            ShopSystem = shopSystem;
        }

        public void Initialize()
        {
            ShopSystem.Initialize();
        }
    }
}