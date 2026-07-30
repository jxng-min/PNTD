using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    public class EnemyModel : MonoBehaviour
    {
        [BigHeader("Visual")]
        [Header("Body")]
        [SerializeField] private SpriteRenderer bodyRenderer;
        
        [Header("Shadow")]
        [SerializeField] private SpriteRenderer shadowRenderer;
        
        [Header("Hp Bar")]
        [SerializeField] private Image hpBarBackground;
        [SerializeField] private Image hpBarInner;
        
        [Space(30f)]
        [BigHeader("Effect")]
        [SerializeField] private EnemyVisualEffect visualEffect;

        private EnemyHealth _health;
        private EnemyStatus _status;
        
        private Color _modelColor;
        private Color _currentColor;

        private Tween _colorTween;

        public void Initialize(Color modelColor, EnemyHealth enemyHealth, EnemyStatus enemyStatus)
        {
            if (enemyStatus != null)
            {
                _status = enemyStatus;
                _status.OnChanged += HandleOnStatusUpdated;
            }

            if (enemyHealth != null)
            {
                _health = enemyHealth;
                _health.OnEnemyDamaged += HandleOnEnemyDamaged;
                _health.OnEnemyDied += HandleOnEnemyDied;
            }

            _modelColor = modelColor;
            hpBarInner.fillAmount = 1f;
            RefreshColor();
        }
        
        private void HandleOnStatusUpdated()
        {
            RefreshColor();
        }

        private void HandleOnEnemyDamaged(float currentHp, float maxHp)
        {
            _colorTween?.Kill();
            _colorTween = visualEffect.PlayOnDamagedEffect(bodyRenderer, hpBarInner, _currentColor, () =>
            {
                _colorTween = null;
                RefreshColor();
            });
            
            hpBarInner.fillAmount = Mathf.Clamp01(currentHp / maxHp);
        }

        private void HandleOnEnemyDied()
        {
            // TODO: 적 사망 연출 구현
        }
        
        private void RefreshColor()
        {
            _currentColor = _status?.FinalOverrideColor ?? _modelColor;

            if (_colorTween != null)
            {
                return;
            }
            
            bodyRenderer.color = _currentColor;
            hpBarInner.color = _currentColor;
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _status.OnChanged -= RefreshColor;
            }

            if (_status != null)
            {
                _health.OnEnemyDamaged -= HandleOnEnemyDamaged;
                _health.OnEnemyDied -= HandleOnEnemyDied;
            }

            _colorTween?.Kill();
            _colorTween = null;
        }
    }
}