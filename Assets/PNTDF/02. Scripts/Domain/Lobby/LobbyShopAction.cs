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
            
            var heroContext = new HeroContext(heroDataTableRow);
            if (!_lobbyDomain.PartySystem.TryAddHeroContext(heroContext))
            {
                return false;
            }
            
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

            for (var i = 0; i < 3; i++)
            {
                var tier = _lobbyDomain.ShuffleSystem.GetTier(_lobbyDomain.ShopSystem.Level);
                var hero = _lobbyDomain.ShuffleSystem.GetHeroDataTableRow(tier);

                if (hero == null)
                {
                    continue;
                }
                
                var synergies = _lobbyDomain.ShuffleSystem.GetSynergyDataTableRows(hero.synergy);
                var alreadyOwned = _lobbyDomain.PartySystem.GetHeroContexts(hero.rowID)?.Count > 0;
                var canIncreaseSynergy = !alreadyOwned && synergies.Count > 0;

                contexts.Add(new ShopSlotContext(hero, synergies, canIncreaseSynergy));
            }

            return contexts;
        }
    }
}