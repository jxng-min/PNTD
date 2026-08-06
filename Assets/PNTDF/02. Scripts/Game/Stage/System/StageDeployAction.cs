using UnityEngine;

namespace PNTD
{
    public class StageDeployAction
    {
        private readonly BoardSystem _boardSystem;
        private readonly DeploySystem _deploySystem;
        private readonly HeroFactory _heroFactory;
        private readonly HeroMoveSystem _heroMoveSystem;
        private readonly ClericSanctuarySystem _clericSanctuarySystem;
        private readonly StageMap _stageMap;

        public StageDeployAction(BoardSystem boardSystem,
                                 DeploySystem deploySystem,
                                 HeroFactory heroFactory,
                                 HeroMoveSystem heroMoveSystem,
                                 ClericSanctuarySystem clericSanctuarySystem,
                                 StageMap stageMap)
        {
            _boardSystem = boardSystem;
            _deploySystem = deploySystem;
            _heroFactory = heroFactory;
            _heroMoveSystem = heroMoveSystem;
            _clericSanctuarySystem = clericSanctuarySystem;
            _stageMap = stageMap;
        }

        public bool TryDeploy(DeployContext deployContext, Vector3Int cellPosition)
        {
            if (!CanDeploy(cellPosition))
            {
                return false;
            }

            var position = _stageMap.BuildMap.GetCellCenterWorld(cellPosition);
            var hero = _heroFactory.Create(deployContext, position);
            if (hero == null)
            {
                return false;
            }

            if (!_boardSystem.TryOccupy(cellPosition, hero))
            {
                _heroFactory.Release(hero);
                return false;
            }
            
            hero.NotifyDeployed(cellPosition);
            _heroMoveSystem?.Register(hero);
            _clericSanctuarySystem?.Register(hero);
            _clericSanctuarySystem?.Refresh();

            _deploySystem.CompleteDeploy(deployContext);
            _deploySystem.ExitDeployMode();
            return true;
        }

        public bool CanDeploy(Vector3Int cellPosition)
        {
            if (_stageMap?.BuildMap == null)
            {
                return false;
            }

            return _stageMap.BuildMap.HasTile(cellPosition) &&
                   _boardSystem.CanOccupy(cellPosition);
        }
    }
}
