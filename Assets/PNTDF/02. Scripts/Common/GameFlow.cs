using JxModule;
using System.Collections;
using UnityEngine;

namespace PNTD
{
    public class GameFlow : LocalSingleton<GameFlow>
    {
        private LobbyModel _lobbyModel;
        private MapRunner _mapRunner;
        private StageRunner _stageRunner;
        private MapContext _currentMapContext;
        private bool _isPlaying;

        public void Initialize(LobbyModel lobbyModel, MapRunner mapRunner, StageRunner stageRunner)
        {
            _lobbyModel = lobbyModel;
            _mapRunner = mapRunner;
            _stageRunner = stageRunner;
        }

        public void Play()
        {
            if (_isPlaying)
            {
                return;
            }

            StartCoroutine(PlayRoutine());
        }

        private IEnumerator PlayRoutine()
        {
            if (_lobbyModel == null || _mapRunner == null || _stageRunner == null)
            {
                yield break;
            }

            _isPlaying = true;

            var stage = _lobbyModel.Domain.StatusSystem.Stage;
            var loadingText = $"<pop>Stage {stage}</pop>";
            
            yield return LoadingManager.Instance.VirtualLoadScene(loadingText, LoadStageRoutine);
            yield return _stageRunner.PlayStageRoutine();

            _isPlaying = false;
        }

        private IEnumerator LoadStageRoutine()
        {
            var stage = _lobbyModel.Domain.StatusSystem.Stage;
            var stageId = $"Stage_{stage:00}";
            
            _lobbyModel.Hide();
            
            _currentMapContext = _mapRunner.LoadMap(stageId);
            if (_currentMapContext != null)
            {
                var interest = _lobbyModel.Domain.StatusSystem.Interest;
                var party = _lobbyModel.Domain.PartySystem.HeroContexts;
                
                _stageRunner.Initialize(_currentMapContext,
                                        stage,
                                        interest,
                                        party,
                                        () => _lobbyModel.Domain.SynergySystem.CurrentContext);
            }

            yield break;
        }
    }
}
