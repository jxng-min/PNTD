using JxModule;
using System.Collections;
using UnityEngine;

namespace PNTD
{
    public class GameFlow : LocalSingleton<GameFlow>
    {
        private LobbyModel _lobbyModel;
        private MapRunner _mapRunner;
        private bool _isPlaying;

        public void Initialize(LobbyModel lobbyModel, MapRunner mapRunner)
        {
            _lobbyModel = lobbyModel;
            _mapRunner = mapRunner;
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
            if (_lobbyModel == null || _mapRunner == null)
            {
                yield break;
            }

            _isPlaying = true;

            var stage = _lobbyModel.Domain.StatusSystem.Stage;
            var loadingText = $"<pop>Stage {stage}</pop>";
            
            yield return LoadingManager.Instance.VirtualLoadScene(loadingText, LoadStageRoutine);

            _isPlaying = false;
        }

        private IEnumerator LoadStageRoutine()
        {
            var stage = _lobbyModel.Domain.StatusSystem.Stage;
            var stageId = $"Stage_{stage:00}";
            
            _lobbyModel.Domain.VisibilitySystem.Hide();
            _mapRunner.LoadMap(stageId);

            yield break;
        }
    }
}
