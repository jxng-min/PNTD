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
            
            var canIncreaseHeroExp = _lobbyDomain.PartySystem.CanIncreaseHeroExp(heroDataTableRow.rowID);
            var currentHeroCount = _lobbyDomain.PartySystem.HeroContexts.Count;
            var maxHeroCount = _lobbyDomain.StatusSystem.HeroCountLimit;
            if (!canIncreaseHeroExp && currentHeroCount >= maxHeroCount)
            {
                return false;
            }

            if (_lobbyDomain.StatusSystem.Gold < heroDataTableRow.cost)
            {
                return false;
            }
            
            var heroContext = new HeroContext(heroDataTableRow);
            if (!_lobbyDomain.PartySystem.TryAddHeroContext(heroContext))
            {
                return false;
            }
            
            _lobbyDomain.StatusSystem.UpdateGold(-heroDataTableRow.cost);

            return true;
        }

        public bool TryRerollShop(out List<ShopSlotContext> contexts)
        {
            contexts = null;

            if (_lobbyDomain.ShopSystem.IsLock)
            {
                return false;
            }
            
            if (_lobbyDomain.StatusSystem.Gold < _lobbyDomain.ShopSystem.RerollCost)
            {
                return false;
            }
            
            _lobbyDomain.StatusSystem.UpdateGold(-_lobbyDomain.ShopSystem.RerollCost);

            contexts = CreateShopSlotContexts();
            return true;
        }

        public bool TryLevelUp()
        {
            if (_lobbyDomain.ShopSystem.IsMaxLevel)
            {
                return false;
            }
            
            if (_lobbyDomain.StatusSystem.Gold < _lobbyDomain.ShopSystem.LevelCost)
            {
                return false;
            }
            
            _lobbyDomain.StatusSystem.UpdateGold(-_lobbyDomain.ShopSystem.LevelCost);
            
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
