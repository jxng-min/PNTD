using System;
using UnityEngine;

namespace PNTD
{
    public class DeploySystem
    {
        private DeployContext _deployContext;

        public event Action<DeployContext> OnEnterDeployMode;
        public event Action OnExitDeployMode;
        public event Action<DeployContext, Vector3Int> OnDeployRequested;
        public event Action<DeployContext> OnDeployCompleted;
        
        public bool DeployMode { get; private set; }

        public void EnterDeployMode(DeployContext deployContext)
        {
            if (deployContext == null || deployContext.HeroDataTableRow == null)
            {
                return;
            }
            
            _deployContext = deployContext;
            DeployMode = true;
            OnEnterDeployMode?.Invoke(_deployContext);
        }
        
        public void RequestTryDeploy(Vector3Int cellPosition)
        {
            if (_deployContext == null)
            {
                return;
            }
            
            OnDeployRequested?.Invoke(_deployContext, cellPosition);
        }

        public void ExitDeployMode()
        {
            _deployContext = null;
            DeployMode = false;
            OnExitDeployMode?.Invoke();
        }

        public void CompleteDeploy(DeployContext deployContext)
        {
            OnDeployCompleted?.Invoke(deployContext);
        }
    }
}
