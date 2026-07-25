using System.Collections.Generic;

namespace PNTD
{
    public class LobbyShopAction
    {
        private readonly LobbyDomain _lobbyDomain;

        public LobbyShopAction(LobbyDomain lobbyDomain)
        {
            _lobbyDomain = lobbyDomain;
        }

        public bool TryPurchaseHero(HeroDataTableRow heroDataTableRow)
        {
            if (heroDataTableRow == null)
            {
                return false;
            }
            
            // TODO: 현재 파티 목록 확인
            
            // TODO: 골드 확인
            
            // TODO: 히어로 추가
            
            // TODO: 골드 차감

            return true;
        }

        public bool TryRerollShop(out List<ShopSlotContext> contexts)
        {
            contexts = null;

            if (_lobbyDomain.ShopSystem.IsLock)
            {
                return false;
            }
            
            // TODO: 골드 확인
            
            // TODO; 골드 차감

            contexts = CreateShopSlotContexts();
            return true;
        }

        public bool TryLevelUp()
        {
            if (_lobbyDomain.ShopSystem.IsMaxLevel)
            {
                return false;
            }

            // TODO: 골드 확인
            
            // TODO: 골드 차감
            
            _lobbyDomain.ShopSystem.UpdateLevel();
            return true;
        }

        public List<ShopSlotContext> CreateShopSlotContexts()
        {
            var contexts = new List<ShopSlotContext>();

            if (_lobbyDomain.ShopSystem.IsLock)
            {
                return contexts;
            }

            // TODO: 시너지 컨텍스트 구성

            return contexts;
        }
    }
}