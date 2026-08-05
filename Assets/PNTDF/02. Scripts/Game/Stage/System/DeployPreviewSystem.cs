using UnityEngine;

namespace PNTD
{
    public class DeployPreviewSystem
    {
        private static readonly Color ValidColor = new(0.2f, 1f, 0.35f, 1f);
        private static readonly Color InvalidColor = new(1f, 0.2f, 0.2f, 1f);

        private readonly DeploySystem _deploySystem;
        private readonly StageDeployAction _deployAction;
        private readonly StageMap _stageMap;
        private readonly DeployPreviewView _previewView;

        private DeployContext _deployContext;
        private Vector3Int _currentCellPosition;
        private bool _canDeploy;
        private bool _waitUntilPrimaryReleased;

        public DeployPreviewSystem(DeploySystem deploySystem,
                                   StageDeployAction deployAction,
                                   StageMap stageMap,
                                   DeployPreviewView previewView)
        {
            _deploySystem = deploySystem;
            _deployAction = deployAction;
            _stageMap = stageMap;
            _previewView = previewView;
        }

        public void Tick()
        {
            if (_deployContext == null)
            {
                return;
            }

            UpdatePreview();
            HandleInput();
        }

        public void HandleOnEnterDeployMode(DeployContext deployContext)
        {
            ClearPreview();

            _deployContext = deployContext;
            _previewView?.Initialize(deployContext.HeroDataTableRow);
            _previewView?.Show();

            _waitUntilPrimaryReleased = Input.GetMouseButton(0);

            UpdatePreview();
        }

        public void HandleOnExitDeployMode()
        {
            ClearPreview();
        }

        public void Dispose()
        {
            ClearPreview();
        }

        private void UpdatePreview()
        {
            if (_stageMap?.BuildMap == null)
            {
                return;
            }

            var worldPosition = GetMouseWorldPosition();
            _currentCellPosition = _stageMap.BuildMap.WorldToCell(worldPosition);
            _previewView?.SetWorldPosition(_stageMap.BuildMap.GetCellCenterWorld(_currentCellPosition));

            _canDeploy = _deployAction.CanDeploy(_currentCellPosition);
            _previewView?.SetColor(_canDeploy ? ValidColor : InvalidColor);
        }

        private void HandleInput()
        {
            if (_waitUntilPrimaryReleased)
            {
                _waitUntilPrimaryReleased = Input.GetMouseButton(0);
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                _deploySystem.ExitDeployMode();
                return;
            }

            if (Input.GetMouseButtonDown(0) && _canDeploy)
            {
                _deploySystem.RequestTryDeploy(_currentCellPosition);
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            var targetCamera = Camera.main;
            if (targetCamera == null)
            {
                return Vector3.zero;
            }

            var mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(targetCamera.transform.position.z);
            return targetCamera.ScreenToWorldPoint(mousePosition);
        }

        private void ClearPreview()
        {
            _previewView?.Hide();

            _deployContext = null;
            _currentCellPosition = default;
            _canDeploy = false;
            _waitUntilPrimaryReleased = false;
        }
    }
}
