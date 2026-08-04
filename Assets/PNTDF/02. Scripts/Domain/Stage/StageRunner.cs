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
        [SerializeField] private CanvasGroup[] stageCanvasGroups;

        private StageModel _model;

        private DataTable _enemyDataTable;
        private DataTable _enemyAbilityDataTable;
        private DataTable _enragerDataTable;

        private void Awake()
        {
            _enemyDataTable = DataTableManager.FindTable<EnemyDataTableRow>("DT_Enemy");
            _enemyAbilityDataTable = DataTableManager.FindTable<EnemyAbilityDataTableRow>("DT_EnemyAbility");
            _enragerDataTable = DataTableManager.FindTable<EnragerDataTableRow>("DT_Enrager");
        }

        public void Initialize(MapContext mapContext, int stage)
        {
            DisposeModel();
            
            if (mapContext?.StageContext == null || mapContext.Map == null)
            {
                return;
            }

            var enemyBuilder = new EnemyBuilder(_enemyDataTable, _enemyAbilityDataTable, _enragerDataTable);
            var enemyFactory = new EnemyFactory(enemyBuilder);
            var stageSystem = new StageSystem();
            var waveSystem = new WaveSystem();
            var visibilitySystem = new StageVisibilitySystem(GetStageCanvasGroups());
            
            enemyFactory.Initialize(mapContext.Map.StagePath, waveSystem);
            
            var domain = new StageDomain(stageSystem, waveSystem, visibilitySystem);
            var compositor = new StageCompositor(domain, new RStageContext(), progressView, flowPresenter);

            _model = new StageModel(domain, compositor);
            _model.Initialize(enemyFactory, mapContext.StageContext, stage);
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

            if (flowPresenter != null)
            {
                yield return flowPresenter.Clear();
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

        private void DisposeModel()
        {
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
                progressView
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
