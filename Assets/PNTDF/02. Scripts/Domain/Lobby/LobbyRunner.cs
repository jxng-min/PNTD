using JxModule;
using JxModule.DataTable;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private MapRunner mapRunner;
        [SerializeField] private StageRunner stageRunner;
        [SerializeField] private CanvasGroup[] lobbyCanvasGroups;

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
            var statusSystem = new StatusSystem();
            var visibilitySystem = new LobbyVisibilitySystem(GetLobbyCanvasGroups());
            
            var domain = new LobbyDomain(shopSystem, shuffleSystem, synergySystem, partySystem, statusSystem, visibilitySystem);
            var compositor = new LobbyCompositor(domain, shopPresenter, synergyPresenter, partyPresenter, indexerPresenter);

            _model = new LobbyModel(domain, compositor);
            _model.Initialize();
            
            mapRunner ??= FindFirstObjectByType<MapRunner>();
            stageRunner ??= FindFirstObjectByType<StageRunner>();
            GameFlow.Instance.Initialize(_model, mapRunner, stageRunner);
        }

        public void Roll()
        {
            _model?.Domain.ShopSystem.Initialize();
        }

        private CanvasGroup[] GetLobbyCanvasGroups()
        {
            if (lobbyCanvasGroups is { Length: > 0 })
            {
                return lobbyCanvasGroups;
            }

            var presenters = new MonoBehaviour[]
            {
                shopPresenter,
                synergyPresenter,
                partyPresenter,
                indexerPresenter
            };

            var canvasGroups = new List<CanvasGroup>();
            foreach (var presenter in presenters)
            {
                if (presenter == null)
                {
                    continue;
                }

                var canvasGroup = presenter
                    .GetComponentsInChildren<CanvasGroup>(true)
                    .FirstOrDefault(group => group != null && group.name.EndsWith("Area"));

                if (canvasGroup != null)
                {
                    canvasGroups.Add(canvasGroup);
                }
            }

            return canvasGroups.ToArray();
        }
        
        private void OnDestroy()
        {
            _model?.Dispose();
        }
    }
}
