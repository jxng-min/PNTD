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
        
        [Header("Cool Bar")]
        [SerializeField] private Image coolBarBackground;
        [SerializeField] private Image coolBarInner;

        private Hero _hero;
        
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
        
        private void UpdateCooldownBar(float currentCoolTime, float maxCoolTime)
        {
            coolBarInner.fillAmount = Mathf.Clamp01(currentCoolTime / maxCoolTime);
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