using JxModule;
using JxModule.DataTable;
using UnityEngine;

namespace PNTD
{
    public class LobbyRunner : MonoBehaviour
    {
        [BigHeader("Presenter")]
        [SerializeField] private ShopPresenter shopPresenter;
        [SerializeField] private SynergyPresenter synergyPresenter;
        [SerializeField] private PartyPresenter partyPresenter;
        [SerializeField] private IndexerPresenter indexerPresenter;

        private LobbyModel _model;

        private DataTable _heroDataTable;
        private DataTable _synergyDataTable;
        private DataTable _shopRateDataTable;

        private void Awake()
        {
            _heroDataTable = DataTableManager.FindTable<HeroDataTableRow>("DT_Hero");
            _synergyDataTable = DataTableManager.FindTable<SynergyDataTableRow>("DT_Synergy");
            _shopRateDataTable = DataTableManager.FindTable<ShopRateDataTableRow>("DT_ShopRate");
        }

        private void Start()
        {
            var shopSystem = new ShopSystem();
            var shuffleSystem = new ShuffleSystem(_heroDataTable, _synergyDataTable, _shopRateDataTable);
            var synergySystem = new SynergySystem(_synergyDataTable.FindAll<SynergyDataTableRow>().ToArray());
            var partySystem = new PartySystem();
            
            var domain = new LobbyDomain(shopSystem, shuffleSystem, synergySystem, partySystem);
            var compositor = new LobbyCompositor(domain, shopPresenter, synergyPresenter, partyPresenter, indexerPresenter);

            _model = new LobbyModel(domain, compositor);
            _model.Initialize();
        }

        public void Roll()
        {
            _model?.Domain.ShopSystem.Initialize();
        }
        
        private void OnDestroy()
        {
            _model?.Dispose();
        }
    }
}