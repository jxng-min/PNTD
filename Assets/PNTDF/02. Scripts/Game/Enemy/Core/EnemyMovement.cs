using System;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class EnemyMovement : MonoBehaviour
    {
        [BigHeader("Rotation")]
        [SerializeField] private Transform rotationAxis;
        [SerializeField] private float rotationSpeed = 360f;
        [SerializeField] private float arriveDistance = 0.01f;

        private EnemyStatus _status;
        private StagePath _stagePath;

        private int _currentPointIndex;
        private bool _isReached;

        public event Action OnDestinationReached;
        
        public bool IsInitialize => _status != null && _stagePath != null;
        public bool IsReached => _isReached;
        public bool IsMoving => IsInitialize && !_isReached && _status.FinalMoveSpeed > 0f;
        public int CurrentPointIndex => _currentPointIndex;

        public Vector3 TargetPosition
        {
            get
            {
                if (!HasValidTarget())
                {
                    return transform.position;
                }
                
                return _stagePath.GetPointPosition(_currentPointIndex);
            }
        }

        public bool IsNextEnd => _stagePath != null && _currentPointIndex + 1 >= _stagePath.PointCount;

        public void Initialize(EnemyStatus enemytStatus, StagePath stagePath)
        {
            _status = enemytStatus;
            _stagePath = stagePath;

            _currentPointIndex = 1;
            _isReached = false;

            if (rotationAxis != null)
            {
                rotationAxis.rotation = Quaternion.identity;
            }

            if (_stagePath == null ||
                _stagePath.PointCount <= 0 ||
                _stagePath.SpawnPoint == null)
            {
                return;
            }

            transform.position = _stagePath.SpawnPoint.position;
            if (_currentPointIndex >= _stagePath.PointCount)
            {
                ReachDestination();
            }
        }

        private void MoveToCurrentTarget(float deltaTime)
        {
            var targetPosition = _stagePath.GetPointPosition(_currentPointIndex);
            targetPosition.z = transform.position.z;
            
            var direction = targetPosition - transform.position;
            direction.z = 0f;

            var moveSpeed = _status.FinalMoveSpeed;
            if (moveSpeed <= 0f)
            {
                return;
            }

            UpdateRotation(direction, deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * deltaTime);

            if (HasArrived(targetPosition))
            {
                MoveToNextPoint();
            }
        }

        private void UpdateRotation(Vector3 direction, float deltaTime)
        {
            if (rotationAxis == null || direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }
            
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var targetRotation = Quaternion.Euler(0f, 0f, angle);
            
            rotationAxis.rotation = Quaternion.RotateTowards(rotationAxis.rotation, targetRotation, rotationSpeed * deltaTime);
        }

        private bool HasArrived(Vector3 targetPosition)
        {
            var sqrArriveDistance = arriveDistance * arriveDistance;
            
            return (transform.position - targetPosition).sqrMagnitude <= sqrArriveDistance;
        }

        private void MoveToNextPoint()
        {
            _currentPointIndex++;

            if (_currentPointIndex >= _stagePath.PointCount)
            {
                ReachDestination();
            }
        }

        private void ReachDestination()
        {
            if (_isReached)
            {
                return;
            }

            _isReached = true;
            OnDestinationReached?.Invoke();
        }

        private bool HasValidTarget()
        {
            if (!IsInitialize || _isReached)
            {
                return false;
            }
            
            return _currentPointIndex >= 0 && _currentPointIndex < _stagePath.PointCount;
        }
        
        private void Update()
        {
            if (!HasValidTarget())
            {
                return;
            }

            MoveToCurrentTarget(Time.deltaTime);
        }
    }
}