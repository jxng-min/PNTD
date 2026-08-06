using JxModule;
using JxModule.DataTable;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PNTD
{
    public class LobbyRunner : MonoBehaviour
    {
        [System.Serializable]
        private class HeroPartySeed
        {
            public string heroId;
            [Range(1, 3)] public int level = 1;
        }

        [BigHeader("Presenter")]
        [SerializeField] private ShopPresenter shopPresenter;
        [SerializeField] private SynergyPresenter synergyPresenter;
        [SerializeField] private PartyPresenter partyPresenter;
        [SerializeField] private IndexerPresenter indexerPresenter;
        [SerializeField] private MapRunner mapRunner;
        [SerializeField] private StageRunner stageRunner;
        [SerializeField] private CanvasGroup[] lobbyCanvasGroups;

        [Space(30f)]
        [BigHeader("Debug")]
        [SerializeField] private bool seedRangerParty = true;
        [SerializeField] private bool seedMageParty = false;
        [SerializeField] private bool seedStarbornParty = false;
        [SerializeField] private List<HeroPartySeed> rangerPartySeeds = new()
        {
            new HeroPartySeed { heroId = "Hero_Archer", level = 1 },
            new HeroPartySeed { heroId = "Hero_Handgunner", level = 1 },
            new HeroPartySeed { heroId = "Hero_Shotgunner", level = 1 },
            new HeroPartySeed { heroId = "Hero_Artillery", level = 1 },
            new HeroPartySeed { heroId = "Hero_Sniper", level = 1 },
            new HeroPartySeed { heroId = "Hero_Trickshooter", level = 1 },
        };
        [SerializeField] private List<HeroPartySeed> magePartySeeds = new()
        {
            new HeroPartySeed { heroId = "Hero_Magician", level = 1 },
            new HeroPartySeed { heroId = "Hero_Wizard", level = 1 },
            new HeroPartySeed { heroId = "Hero_Explomancer", level = 1 },
            new HeroPartySeed { heroId = "Hero_Telekinetic", level = 1 },
            new HeroPartySeed { heroId = "Hero_Transmuter", level = 1 },
            new HeroPartySeed { heroId = "Hero_Artificer", level = 1 },
        };
        [SerializeField] private List<HeroPartySeed> starbornPartySeeds = new()
        {
            new HeroPartySeed { heroId = "Hero_Martian", level = 1 },
            new HeroPartySeed { heroId = "Hero_Venusian", level = 1 },
            new HeroPartySeed { heroId = "Hero_Jovian", level = 1 },
            new HeroPartySeed { heroId = "Hero_Saturnian", level = 1 },
            new HeroPartySeed { heroId = "Hero_Uranian", level = 1 },
        };

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
            
            var initialParty = CreateSeedParty();
            var domain = new LobbyDomain(shopSystem,
                                         shuffleSystem,
                                         synergySystem,
                                         partySystem,
                                         statusSystem,
                                         visibilitySystem,
                                         initialParty);
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
        
        private IReadOnlyList<HeroContext> CreateSeedParty()
        {
            if (seedMageParty)
            {
                return CreateMageParty();
            }
            
            if (seedStarbornParty)
            {
                return CreateStarbornParty();
            }

            return seedRangerParty ? CreateRangerParty() : null;
        }

        private IReadOnlyList<HeroContext> CreateRangerParty()
        {
            return CreateParty(rangerPartySeeds, CreateDefaultRangerPartySeeds());
        }
        
        private IReadOnlyList<HeroContext> CreateMageParty()
        {
            return CreateParty(magePartySeeds, CreateDefaultMagePartySeeds());
        }
        
        private IReadOnlyList<HeroContext> CreateStarbornParty()
        {
            return CreateParty(starbornPartySeeds, CreateDefaultStarbornPartySeeds());
        }
        
        private IReadOnlyList<HeroContext> CreateParty(IReadOnlyList<HeroPartySeed> configuredSeeds,
                                                       IReadOnlyList<HeroPartySeed> defaultSeeds)
        {
            var seeds = configuredSeeds is { Count: > 0 } ? configuredSeeds : defaultSeeds;

            var heroContexts = new List<HeroContext>(seeds.Count);
            foreach (var seed in seeds)
            {
                if (seed == null || string.IsNullOrWhiteSpace(seed.heroId))
                {
                    continue;
                }

                var heroDataTableRow = _heroDataTable?.Find<HeroDataTableRow>(row => row.isEnable && row.rowID == seed.heroId);
                if (heroDataTableRow != null)
                {
                    heroContexts.Add(new HeroContext(heroDataTableRow, Mathf.Clamp(seed.level, 1, 3), 0));
                }
            }

            return heroContexts;
        }

        private static List<HeroPartySeed> CreateDefaultRangerPartySeeds()
        {
            return new List<HeroPartySeed>
            {
                new() { heroId = "Hero_Archer", level = 1 },
                new() { heroId = "Hero_Handgunner", level = 1 },
                new() { heroId = "Hero_Shotgunner", level = 1 },
                new() { heroId = "Hero_Artillery", level = 1 },
                new() { heroId = "Hero_Sniper", level = 1 },
                new() { heroId = "Hero_Trickshooter", level = 1 },
            };
        }
        
        private static List<HeroPartySeed> CreateDefaultMagePartySeeds()
        {
            return new List<HeroPartySeed>
            {
                new() { heroId = "Hero_Magician", level = 1 },
                new() { heroId = "Hero_Wizard", level = 1 },
                new() { heroId = "Hero_Explomancer", level = 1 },
                new() { heroId = "Hero_Telekinetic", level = 1 },
                new() { heroId = "Hero_Transmuter", level = 1 },
                new() { heroId = "Hero_Artificer", level = 1 },
            };
        }
        
        private static List<HeroPartySeed> CreateDefaultStarbornPartySeeds()
        {
            return new List<HeroPartySeed>
            {
                new() { heroId = "Hero_Martian", level = 1 },
                new() { heroId = "Hero_Venusian", level = 1 },
                new() { heroId = "Hero_Jovian", level = 1 },
                new() { heroId = "Hero_Saturnian", level = 1 },
                new() { heroId = "Hero_Uranian", level = 1 },
            };
        }
        
        private void OnDestroy()
        {
            _model?.Dispose();
        }
    }
}
