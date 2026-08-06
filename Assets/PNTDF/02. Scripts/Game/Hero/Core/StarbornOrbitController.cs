using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class StarbornOrbitController : MonoBehaviour
    {
        private const string OrbPrefabName = "[PF] Starborn Orb";
        private const int MaxOrbCount = 6;

        private readonly List<StarbornOrb> _orbs = new();

        private Hero _owner;
        private StarbornAttackData _data;
        private float _angleOffset;
        private float _radiusCycleTime;
        private int _baseOrbCount;
        private int _synergyOrbBonus;
        private bool _canDamage = true;
        private bool _isOrbitPaused;
        private bool _isInitialized;

        public Hero Owner => _owner;
        public StarbornAttackData Data => _data;
        public bool CanDamage => _isInitialized && _canDamage && _owner != null && !_owner.IsSkillSealed;
        public int FinalOrbCount => Mathf.Clamp(_baseOrbCount + _synergyOrbBonus, 0, MaxOrbCount);

        public void Initialize(Hero owner, StarbornAttackData data)
        {
            Release();

            _owner = owner;
            _data = data;
            _baseOrbCount = Mathf.Clamp(owner != null ? owner.Level : 0, 0, MaxOrbCount);
            _synergyOrbBonus = 0;
            _angleOffset = 0f;
            _radiusCycleTime = 0f;
            _canDamage = true;
            _isOrbitPaused = false;
            _isInitialized = _owner != null && _data != null;

            if (_owner?.Dragger != null)
            {
                _owner.Dragger.OnBeginDragRequested += HandleOnBeginDragRequested;
                _owner.Dragger.OnEndDragRequested += HandleOnEndDragRequested;
            }

            RefreshOrbs();
        }

        public void SetSynergyOrbBonus(int bonusOrbCount)
        {
            _synergyOrbBonus = Mathf.Max(0, bonusOrbCount);
            RefreshOrbs();
        }

        public void Release()
        {
            if (_owner?.Dragger != null)
            {
                _owner.Dragger.OnBeginDragRequested -= HandleOnBeginDragRequested;
                _owner.Dragger.OnEndDragRequested -= HandleOnEndDragRequested;
            }

            for (var index = _orbs.Count - 1; index >= 0; index--)
            {
                if (_orbs[index] != null)
                {
                    _orbs[index].ReturnToPool();
                }
            }

            _orbs.Clear();
            _owner = null;
            _data = null;
            _isInitialized = false;
            _canDamage = true;
            _isOrbitPaused = false;
        }

        private void RefreshOrbs()
        {
            if (!_isInitialized)
            {
                return;
            }

            var finalOrbCount = FinalOrbCount;
            while (_orbs.Count < finalOrbCount)
            {
                var orb = CreateOrb(_orbs.Count);
                if (orb == null)
                {
                    break;
                }

                _orbs.Add(orb);
            }

            for (var index = _orbs.Count - 1; index >= finalOrbCount; index--)
            {
                if (_orbs[index] != null)
                {
                    _orbs[index].ReturnToPool();
                }

                _orbs.RemoveAt(index);
            }

            for (var index = 0; index < _orbs.Count; index++)
            {
                _orbs[index]?.Refresh(index);
            }

            UpdateOrbPositions();
        }

        private StarbornOrb CreateOrb(int orbIndex)
        {
            var orbPrefab = PrefabManager.CachePrefab<StarbornOrb>(OrbPrefabName);
            if (orbPrefab == null)
            {
                DebugExtension.LogColor($"Starborn: StarbornOrb prefab '{OrbPrefabName}' is missing.", Color.red);
                return null;
            }

            var orbObject = ObjectPoolManager.Instance.Get(orbPrefab.gameObject);
            if (orbObject == null)
            {
                return null;
            }

            orbObject.transform.SetParent(transform, false);

            var orb = orbObject.GetComponent<StarbornOrb>();
            if (orb == null)
            {
                ObjectPoolManager.Instance.Return(orbObject);
                return null;
            }

            orb.Initialize(this, orbIndex);
            return orb;
        }

        private void Update()
        {
            if (!_isInitialized || _data == null || _isOrbitPaused)
            {
                return;
            }

            _angleOffset = Mathf.Repeat(_angleOffset + _data.orbitSpeed * Time.deltaTime, 360f);
            _radiusCycleTime += Time.deltaTime;
            UpdateOrbPositions();
        }

        private void UpdateOrbPositions()
        {
            var orbCount = _orbs.Count;
            if (orbCount <= 0 || _data == null)
            {
                return;
            }

            var angleStep = 360f / orbCount;
            var radius = GetCurrentRadius();

            for (var index = 0; index < orbCount; index++)
            {
                var orb = _orbs[index];
                if (orb == null)
                {
                    continue;
                }

                var angle = _angleOffset + angleStep * index;
                if (_data.orbitShape == EOrbitShape.Ellipse)
                {
                    var ellipseRotationStep = _data.distributeEllipseRotationByOrb ? 180f / orbCount : 0f;
                    var position = GetEllipsePosition(angle, ellipseRotationStep * index);
                    orb.transform.localPosition = position;
                    continue;
                }

                orb.SetOrbitPosition(angle, radius);
            }
        }

        private Vector3 GetEllipsePosition(float angle, float orbRotationOffset)
        {
            var radians = angle * Mathf.Deg2Rad;
            var position = new Vector2(Mathf.Cos(radians) * _data.ellipseRadiusX,
                                       Mathf.Sin(radians) * _data.ellipseRadiusY);

            var rotationAngle = _data.distributeEllipseRotationByOrb ? orbRotationOffset : 0f;

            if (_data.alignEllipseToTarget && _owner != null)
            {
                var target = _owner.Caster?.FindNearestTarget();
                if (target != null)
                {
                    var direction = target.transform.position - _owner.transform.position;
                    direction.z = 0f;
                    if (direction.sqrMagnitude > Mathf.Epsilon)
                    {
                        rotationAngle += Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    }
                }
            }

            if (Mathf.Abs(rotationAngle) <= Mathf.Epsilon)
            {
                return position;
            }

            var rotation = Quaternion.Euler(0f, 0f, rotationAngle);
            return rotation * position;
        }

        private float GetCurrentRadius()
        {
            if (_data.orbitShape != EOrbitShape.PulsingCircle)
            {
                return _data.orbitRadius;
            }

            var innerHold = Mathf.Max(0f, _data.innerHoldDuration);
            var outerHold = Mathf.Max(0f, _data.outerHoldDuration);
            var transition = Mathf.Max(0.001f, _data.orbitTransitionDuration);
            var cycleDuration = innerHold + transition + outerHold + transition;
            var time = Mathf.Repeat(_radiusCycleTime, cycleDuration);

            if (time < innerHold)
            {
                return _data.orbitRadius;
            }

            time -= innerHold;
            if (time < transition)
            {
                return Mathf.Lerp(_data.orbitRadius, _data.secondaryOrbitRadius, time / transition);
            }

            time -= transition;
            if (time < outerHold)
            {
                return _data.secondaryOrbitRadius;
            }

            time -= outerHold;
            return Mathf.Lerp(_data.secondaryOrbitRadius, _data.orbitRadius, time / transition);
        }

        private void HandleOnBeginDragRequested(Hero hero, UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (hero == _owner)
            {
                _canDamage = false;
                _isOrbitPaused = true;
                SetOrbCollisions(false);
            }
        }

        private void HandleOnEndDragRequested(Hero hero, UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (hero == _owner)
            {
                _canDamage = true;
                _isOrbitPaused = false;
                SetOrbCollisions(true);
            }
        }

        private void SetOrbCollisions(bool isEnabled)
        {
            foreach (var orb in _orbs)
            {
                orb?.SetCollisionEnabled(isEnabled);
            }
        }

        private void OnDestroy()
        {
            Release();
        }
    }
}
