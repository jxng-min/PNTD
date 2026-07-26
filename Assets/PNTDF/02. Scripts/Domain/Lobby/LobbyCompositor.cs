using System.Collections.Generic;

namespace PNTD
{
    public class LobbyCompositor
    {
        private readonly LobbyDomain _lobbyDomain;

        private readonly ShopPresenter _shopPresenter;
        private readonly SynergyPresenter _synergyPresenter;
        private readonly PartyPresenter _partyPresenter;
        private readonly IndexerPresenter _indexerPresenter;
        
        private readonly LobbyShopAction _lobbyShopAction;
        private readonly LobbyPartyAction _lobbyPartyAction;

        public LobbyCompositor(LobbyDomain lobbyDomain,
                               ShopPresenter shopPresenter,
                               SynergyPresenter synergyPresenter,
                               PartyPresenter partyPresenter,
                               IndexerPresenter indexerPresenter)
        {
            _lobbyDomain = lobbyDomain;
            
            _shopPresenter = shopPresenter;
            _synergyPresenter = synergyPresenter;
            _partyPresenter = partyPresenter;
            _indexerPresenter = indexerPresenter;
            
            _lobbyShopAction = new LobbyShopAction(_lobbyDomain);
            _lobbyPartyAction = new LobbyPartyAction(_lobbyDomain);
        }

        public void Initialize()
        {
            _shopPresenter.Initialize(_lobbyDomain.ShopSystem);
            _indexerPresenter.Initialize();
        }

#region Event Handlings
        private void HandleOnRequestShopRoll()
        {
            if (_lobbyDomain.ShopSystem.IsLock)
            {
                return;
            }

            var contexts = _lobbyShopAction.CreateShopSlotContexts();
            _shopPresenter.HandleOnRerollShop(contexts);
            _shopPresenter.RefreshSlotsSynergies(_lobbyDomain.SynergySystem.CurrentContext,
                                                 _lobbyDomain.PartySystem.HeroContexts);
        }

        private void HandleOnClickedShopSlot(HeroDataTableRow heroDataTableRow, int slotIndex)
        {
            if (!_lobbyShopAction.TryPurchaseHero(heroDataTableRow))
            {
                return;
            }

            _shopPresenter.SetSoldOut(slotIndex);
            _shopPresenter.RefreshSlotsSynergies(_lobbyDomain.SynergySystem.CurrentContext,
                                                 _lobbyDomain.PartySystem.HeroContexts);
        }

        private void HandleOnClickedShopReroll()
        {
            if (!_lobbyShopAction.TryRerollShop(out var contexts))
            {
                return;
            }
            
            _shopPresenter.HandleOnRerollShop(contexts);
            _shopPresenter.RefreshSlotsSynergies(_lobbyDomain.SynergySystem.CurrentContext,
                                                 _lobbyDomain.PartySystem.HeroContexts);
        }

        private void HandleOnRequestShopLevelUp()
        {
            _lobbyShopAction.TryLevelUp();
        }

        private void HandleOnSynergyUpdated(SynergyContext synergyContext)
        {
            _synergyPresenter.UpdateSynergySlots(synergyContext, _lobbyDomain.SynergySystem.SynergyDataTableRows);
            _shopPresenter.RefreshSlotsSynergies(_lobbyDomain.SynergySystem.CurrentContext,
                                                 _lobbyDomain.PartySystem.HeroContexts);
        }

        private void HandleUpdateSynergies()
        {
            _lobbyDomain.SynergySystem.RefreshSynergies(_lobbyDomain.PartySystem.HeroContexts);
        }

        private void HandleOnUpdateCountRequested()
        {
            _lobbyPartyAction.GetPartyCount(out var currentCount, out var maxCount);
            _partyPresenter.UpdateCountLabel(currentCount, maxCount);
        }

        private void HandleOnClickedPartySlot(HeroContext heroContext, int price)
        {
            _lobbyPartyAction.SellHero(heroContext, price);
        }

        private void HandleOnReorderPartyRequested(List<HeroContext> heroContexts)
        {
            _lobbyPartyAction.ReorderParty(heroContexts);
        }
#endregion

#region Event Bindings
        public void BindEvents()
        {
            BindLobbyShopEvents();
            BindLobbyPartyEvents();
            BindLobbySynergyEvents();
        }

        public void ReleaseEvents()
        {
            ReleaseLobbyShopEvents();
            ReleaseLobbyPartyEvents();
            ReleaseLobbySynergyEvents();
        }

        private void BindLobbyShopEvents()
        {
            // TODO: 스테이터스 추가 시, 골드 업데이트 연결
            
            _lobbyDomain.ShopSystem.OnRequestShopRoll += HandleOnRequestShopRoll;
            _lobbyDomain.ShopSystem.OnUpdateLevel += _shopPresenter.HandleOnUpdateLevel;
            
            // TODO: 스테이터스 추가 시, 시너지 변경 연결

            _shopPresenter.OnClickedSlot += HandleOnClickedShopSlot;
            _shopPresenter.OnClickedReroll += HandleOnClickedShopReroll;
            _shopPresenter.OnChangedLock += _lobbyDomain.ShopSystem.UpdateLock;
            _shopPresenter.OnRequestLevelUp += HandleOnRequestShopLevelUp;
        }
        
        private void ReleaseLobbyShopEvents()
        {
            // TODO: 스테이터스 추가 시, 골드 업데이트 해제
            
            _lobbyDomain.ShopSystem.OnRequestShopRoll -= HandleOnRequestShopRoll;
            _lobbyDomain.ShopSystem.OnUpdateLevel -= _shopPresenter.HandleOnUpdateLevel;
            
            // TODO: 스테이터스 추가 시, 시너지 변경 해제

            _shopPresenter.OnClickedSlot -= HandleOnClickedShopSlot;
            _shopPresenter.OnClickedReroll -= HandleOnClickedShopReroll;
            _shopPresenter.OnChangedLock -= _lobbyDomain.ShopSystem.UpdateLock;
            _shopPresenter.OnRequestLevelUp -= HandleOnRequestShopLevelUp;
        }

        private void BindLobbySynergyEvents()
        {
            _lobbyDomain.SynergySystem.OnSynergyUpdated += HandleOnSynergyUpdated;
        }

        private void ReleaseLobbySynergyEvents()
        {
            _lobbyDomain.SynergySystem.OnSynergyUpdated -= HandleOnSynergyUpdated;
        }

        private void BindLobbyPartyEvents()
        {
            _lobbyDomain.PartySystem.OnHeroAdded += _partyPresenter.HandleHeroAdded;
            _lobbyDomain.PartySystem.OnHeroRemoved += _partyPresenter.HandlePartyChanged;
            _lobbyDomain.PartySystem.OnPartyChanged += _partyPresenter.HandlePartyChanged;
            _lobbyDomain.PartySystem.OnPartyChanged += HandleUpdateSynergies;
            _lobbyDomain.PartySystem.OnHeroLevelUpdated += _partyPresenter.HandlePartyChanged;
            
            // TODO: 스테이터스 추가 시, 최대 파티 수 업데이트 연결

            _partyPresenter.OnUpdateCountRequested += HandleOnUpdateCountRequested;
            _partyPresenter.OnClickedPartySlot += HandleOnClickedPartySlot;
            _partyPresenter.OnReorderPartyRequested += HandleOnReorderPartyRequested;
        }

        private void ReleaseLobbyPartyEvents()
        {
            _lobbyDomain.PartySystem.OnHeroAdded -= _partyPresenter.HandleHeroAdded;
            _lobbyDomain.PartySystem.OnHeroRemoved -= _partyPresenter.HandlePartyChanged;
            _lobbyDomain.PartySystem.OnPartyChanged -= _partyPresenter.HandlePartyChanged;
            _lobbyDomain.PartySystem.OnPartyChanged -= HandleUpdateSynergies;
            _lobbyDomain.PartySystem.OnHeroLevelUpdated -= _partyPresenter.HandlePartyChanged;
            
            // TODO: 스테이터스 추가 시, 최대 파티 수 업데이트 해제

            _partyPresenter.OnUpdateCountRequested -= HandleOnUpdateCountRequested;
            _partyPresenter.OnClickedPartySlot -= HandleOnClickedPartySlot;
            _partyPresenter.OnReorderPartyRequested -= HandleOnReorderPartyRequested;
        }
#endregion
    }
}