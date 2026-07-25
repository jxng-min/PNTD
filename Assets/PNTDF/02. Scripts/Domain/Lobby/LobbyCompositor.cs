namespace PNTD
{
    public class LobbyCompositor
    {
        private readonly LobbyDomain _lobbyDomain;

        private readonly ShopPresenter _shopPresenter;
        private readonly SynergyPresenter _synergyPresenter;
        
        private readonly LobbyShopAction _lobbyShopAction;

        public LobbyCompositor(LobbyDomain lobbyDomain,
                               ShopPresenter shopPresenter,
                               SynergyPresenter synergyPresenter)
        {
            _lobbyDomain = lobbyDomain;
            
            _shopPresenter = shopPresenter;
            _synergyPresenter = synergyPresenter;
            
            _lobbyShopAction = new LobbyShopAction(_lobbyDomain);
        }

        public void Initialize()
        {
            _shopPresenter.Initialize(_lobbyDomain.ShopSystem);
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
            // TODO: 슬롯 시너지 갱신
        }

        private void HandleOnClickedShopSlot(HeroDataTableRow heroDataTableRow, int slotIndex)
        {
            if (!_lobbyShopAction.TryPurchaseHero(heroDataTableRow))
            {
                return;
            }

            _shopPresenter.SetSoldOut(slotIndex);
            // TODO: 슬롯 시너지 갱신
        }

        private void HandleOnClickedShopReroll()
        {
            if (!_lobbyShopAction.TryRerollShop(out var contexts))
            {
                return;
            }
            
            _shopPresenter.HandleOnRerollShop(contexts);
            // TODO: 슬롯 시너지 갱신
        }

        private void HandleOnRequestShopLevelUp()
        {
            _lobbyShopAction.TryLevelUp();
        }

        private void HandleOnSynergyUpdated(SynergyContext synergyContext)
        {
            _synergyPresenter.UpdateSynergySlots(synergyContext, _lobbyDomain.SynergySystem.SynergyDataTableRows);
            // TODO: 슬롯 시너지 갱신
        }
#endregion

#region Event Bindings
        public void BindEvents()
        {
            BindLobbyShopEvents();
            BindLobbySynergyEvents();
        }

        public void ReleaseEvents()
        {
            ReleaseLobbyShopEvents();
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
#endregion
    }
}