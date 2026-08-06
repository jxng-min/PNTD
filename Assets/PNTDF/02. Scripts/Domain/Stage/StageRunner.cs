using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JxModule;
using JxModule.DataTable;
using UnityEngine;

namespace PNTD
{
    public class StageRunner : MonoBehaviour
    {
        [BigHeader("Presenter")]
        [SerializeField] private FlowPresenter flowPresenter;
        [SerializeField] private ProgressView progressView;
        [SerializeField] private ResultPresenter resultPresenter;
        [SerializeField] private PalettePresenter palettePresenter;
        [SerializeField] private CanvasGroup[] stageCanvasGroups;
        
        [Space(30f)]
        [BigHeader("References")]
        [SerializeField] private Transform heroRoot;
        [SerializeField] private DeployPreviewView deployPreviewView;

        private StageModel _model;

        private DataTable _enemyDataTable;
        private DataTable _enemyAbilityDataTable;
        private DataTable _enragerDataTable;
        private DataTable _heroDataTable;
        private DataTable _heroAttackDataTable;
        private int _interest;
        
        public StageModel.EStageResult StageResult => _model?.StageResult ?? StageModel.EStageResult.None;
        public int RewardGold => _model?.RewardGold ?? 0;
        public int BonusGold => _model?.BonusGold ?? 0;
        public int Interest => _interest;

        private void Awake()
        {
            _enemyDataTable = DataTableManager.FindTable<EnemyDataTableRow>("DT_Enemy");
            _enemyAbilityDataTable = DataTableManager.FindTable<EnemyAbilityDataTableRow>("DT_EnemyAbility");
            _enragerDataTable = DataTableManager.FindTable<EnragerDataTableRow>("DT_Enrager");
            _heroDataTable = DataTableManager.FindTable<HeroDataTableRow>("DT_Hero");
            _heroAttackDataTable = DataTableManager.FindTable<HeroAttackDataTableRow>("DT_HeroAttack");
        }

        public void Initialize(MapContext mapContext,
                               int stage,
                               int interest,
                               IReadOnlyList<HeroContext> party,
                               Func<SynergyContext> synergyContextProvider)
        {
            DisposeModel();
            
            if (mapContext?.StageContext == null || mapContext.Map == null)
            {
                return;
            }

            _interest = interest;

            var enemyBuilder = new EnemyBuilder(_enemyDataTable, _enemyAbilityDataTable, _enragerDataTable);
            var enemyFactory = new EnemyFactory(enemyBuilder);
            var stageSystem = new StageSystem();
            var waveSystem = new WaveSystem();
            var visibilitySystem = new StageVisibilitySystem(GetStageCanvasGroups());
            var boardSystem = new BoardSystem();
            var deploySystem = new DeploySystem();
            var deployContextFactory = new DeployContextFactory();
            var runtimeStageContext = new RStageContext();
            var goldSpawner = new GoldSpawner(runtimeStageContext, heroRoot);
            var plunderSystem = new PlunderSystem(goldSpawner, synergyContextProvider);
            var stagePooledObjectCleaner = new StagePooledObjectCleaner();
            var skillContext = new HeroSkillContext(mapContext.Map.BuildMap, heroRoot, boardSystem, goldSpawner);
            var heroPrefab = PrefabManager.CachePrefab<Hero>("[PF] Hero");
            var magitechRobotPrefab = PrefabManager.CachePrefab<Hero>("[PF] Magitech Robot");
            var heroFactory = new HeroFactory(heroPrefab, magitechRobotPrefab, _heroDataTable, _heroAttackDataTable, skillContext, heroRoot);
            var clericSanctuarySystem = new ClericSanctuarySystem(boardSystem, synergyContextProvider);
            var heroMoveSystem = new HeroMoveSystem(boardSystem,
                                                    deploySystem,
                                                    mapContext.Map,
                                                    clericSanctuarySystem,
                                                    deployPreviewView);
            var deployAction = new StageDeployAction(boardSystem, deploySystem, heroFactory, heroMoveSystem, clericSanctuarySystem, mapContext.Map);
            var deployPreviewSystem = new DeployPreviewSystem(deploySystem,
                                                              deployAction,
                                                              mapContext.Map,
                                                              deployPreviewView);
            
            enemyFactory.Initialize(mapContext.Map.StagePath, waveSystem);
            
            var domain = new StageDomain(stageSystem,
                                         waveSystem,
                                         visibilitySystem,
                                         boardSystem,
                                         deploySystem,
                                         heroFactory,
                                         deployPreviewSystem,
                                         heroMoveSystem,
                                         clericSanctuarySystem,
                                         plunderSystem,
                                         goldSpawner,
                                         stagePooledObjectCleaner);
            var compositor = new StageCompositor(domain, runtimeStageContext, progressView, flowPresenter, deployAction);

            _model = new StageModel(domain, compositor, runtimeStageContext);
            _model.Initialize(enemyFactory, mapContext.StageContext, stage);
            palettePresenter?.Initialize(party, domain.DeploySystem, deployContextFactory, synergyContextProvider);
            _model.Show();
        }

        public IEnumerator PlayStageRoutine()
        {
            if (_model == null)
            {
                yield break;
            }

            _model.StartStage();
            yield return _model.WaitUntilStageEnd();

            if (_model.StageResult == StageModel.EStageResult.Clear && flowPresenter != null)
            {
                yield return flowPresenter.Clear();
            }

            if (resultPresenter == null)
            {
                yield break;
            }

            switch (_model.StageResult)
            {
                case StageModel.EStageResult.Clear:
                    yield return resultPresenter.StageClear(_model.RewardGold, _model.BonusGold, _interest);
                    break;
                
                case StageModel.EStageResult.Over:
                    yield return resultPresenter.StageOver(_model.ReachedStage);
                    break;
            }
        }

        private void Update()
        {
            _model?.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            DisposeModel();
        }

        public void DisposeStage()
        {
            _model?.Hide();
            DisposeModel();
        }

        private void DisposeModel()
        {
            palettePresenter?.Release();
            _model?.Dispose();
            _model = null;
        }

        private CanvasGroup[] GetStageCanvasGroups()
        {
            if (stageCanvasGroups is { Length: > 0 })
            {
                return stageCanvasGroups;
            }

            var presenters = new MonoBehaviour[]
            {
                flowPresenter,
                progressView,
                resultPresenter,
                palettePresenter
            };

            var canvasGroups = new List<CanvasGroup>();
            foreach (var presenter in presenters)
            {
                if (presenter == null)
                {
                    continue;
                }

                var canvasGroup = presenter.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroups.Add(canvasGroup);
                    continue;
                }

                canvasGroup = presenter
                    .GetComponentsInParent<CanvasGroup>(true)
                    .FirstOrDefault(group => group != null);

                if (canvasGroup != null)
                {
                    canvasGroups.Add(canvasGroup);
                }
            }

            return canvasGroups.Distinct().ToArray();
        }
    }
}
