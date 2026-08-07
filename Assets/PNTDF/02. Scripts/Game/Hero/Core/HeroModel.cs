using JxModule;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    public class HeroModel : MonoBehaviour
    {
        [BigHeader("Visual")]
        [Header("Body")]
        [SerializeField] private SpriteRenderer bodyRenderer;
        [SerializeField] private Transform rotationAxis;
        [SerializeField] private float facingAngleOffset = 180f;
        
        [Header("Cool Bar")]
        [SerializeField] private Image coolBarBackground;
        [SerializeField] private Image coolBarInner;

        private Hero _hero;
        private bool _isRotationPaused;
        
        public Color Color { get; private set; }
        public Transform RotationAxis => rotationAxis;

        public void Initialize(Hero hero)
        {
            if (hero == null)
            {
                DebugExtension.LogColor("Hero Model: Hero is null.", Color.red);
                return;
            }

            _hero = hero;
            _hero.Attack.OnUpdateCool += UpdateCooldownBar;

            Color = _hero.HeroDataTableRow.color;
            bodyRenderer.color = Color;
            
            coolBarBackground.gameObject.SetActive(!_hero.Attack.NonCool);
            coolBarInner.gameObject.SetActive(!_hero.Attack.NonCool);

            if (!_hero.Attack.NonCool)
            {
                coolBarInner.color = Color;
            }

            if (_hero.Skill.IsContinuous)
            {
                UpdateCooldownBar(1f, 1f);
            }
        }

        public void SetSealed(bool isSealed)
        {
            coolBarInner.color = isSealed ? Color.gray : Color;
            bodyRenderer.color = isSealed ? Color.gray : Color;
        }

        public void SetRotationPaused(bool isPaused)
        {
            _isRotationPaused = isPaused;
        }

        public void FaceDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            var target = rotationAxis != null ? rotationAxis : transform;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + facingAngleOffset;
            target.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        public void FaceTarget(Enemy target)
        {
            if (_hero == null || target == null)
            {
                return;
            }

            FaceDirection(target.transform.position - _hero.transform.position);
        }
        
        private void UpdateCooldownBar(float currentCoolTime, float maxCoolTime)
        {
            coolBarInner.fillAmount = Mathf.Clamp01(currentCoolTime / maxCoolTime);
        }

        private void Update()
        {
            if (_isRotationPaused)
            {
                return;
            }

            var target = _hero?.Caster?.FindNearestTarget();
            if (target == null)
            {
                return;
            }

            FaceTarget(target);
        }

        private void OnDestroy()
        {
            if (_hero != null && _hero.Attack != null)
            {
                _hero.Attack.OnUpdateCool -= UpdateCooldownBar;
            }
        }
    }
}
