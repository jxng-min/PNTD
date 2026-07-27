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
            // TODO: 스테이터스 추가 시, 최대 개수 연결
            maxCount = currentCount;
        }

        public void GetPartySlotContext(out IReadOnlyList<HeroContext> heroContexts,
                                        out int heroCount,
                                        out int visibleCount)
        {
            heroContexts = _lobbyDomain.PartySystem.HeroContexts;
            heroCount = heroContexts.Count;
            
            // TODO: 스테이터스 추가 시, 최대 개수 연결
            var maxHeroCount = heroContexts.Count;
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
            
            // TODO: 스테이터스 추가 시, 판매 가격만큼 추가
            _lobbyDomain.PartySystem.RemoveHeroContext(heroContext);
        }
    }
}