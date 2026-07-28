using System.Collections.Generic;
using UnityEngine;

namespace PNTD
{
    public class LobbyPartyAction
    {
        private readonly LobbyDomain _lobbyDomain;

        public LobbyPartyAction(LobbyDomain lobbyDomain)
        {
            _lobbyDomain = lobbyDomain;
        }

        public void GetPartyCount(out int currentCount, out int maxCount)
        {
            currentCount = _lobbyDomain.PartySystem.HeroContexts.Count;
            maxCount = _lobbyDomain.StatusSystem.HeroCountLimit;
        }

        public void GetPartySlotContext(out IReadOnlyList<HeroContext> heroContexts,
                                        out int heroCount,
                                        out int visibleCount)
        {
            heroContexts = _lobbyDomain.PartySystem.HeroContexts;
            heroCount = heroContexts.Count;
            
            var maxHeroCount = _lobbyDomain.StatusSystem.HeroCountLimit;
            visibleCount = Mathf.Min(heroCount, maxHeroCount);
        }

        public void ReorderParty(List<HeroContext> heroContexts)
        {
            _lobbyDomain.PartySystem.TryReorderParty(heroContexts);
        }

        public void SellHero(HeroContext heroContext, int price)
        {
            if (heroContext == null)
            {
                return;
            }
            
            _lobbyDomain.StatusSystem.UpdateGold(price);
            _lobbyDomain.PartySystem.RemoveHeroContext(heroContext);
        }
    }
}