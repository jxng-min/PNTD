using System;
using DG.Tweening;
using JxModule;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    [ManagedEffect("Game", "Enemy", 0)]
    public class EnemyVisualEffect : MonoBehaviour
    {
        [BigHeader("Settings")]
        [Header("On Damaged")]
        [SerializeField] private Color hitColor = Color.white;
        [SerializeField] private float damageDuration = 0.1f;

        public Tween PlayOnDamagedEffect(SpriteRenderer bodyRenderer, Image hpBar, Color originColor, Action callback = null)
        {
            CreateDamageParticle(bodyRenderer.transform.position, originColor);
            
            var tColor = DOTween.Sequence();

            tColor.Join(
                bodyRenderer.DOColor(hitColor, damageDuration)
            );

            tColor.Join(
                hpBar.DOColor(hitColor, damageDuration)
            );

            tColor.Append(
                bodyRenderer.DOColor(originColor, damageDuration)
            );

            tColor.Join(
                hpBar.DOColor(originColor, damageDuration)
            );
            
            tColor.OnComplete(() => callback?.Invoke());
            
            return tColor;
        }

        private void CreateDamageParticle(Vector3 targetPosition, Color originColor)
        {
            var particlePrefab = PrefabManager.CachePrefab<EnemyDamagedParticle>("[PF] Enemy Damaged Particle");
            if (particlePrefab == null)
            {
                return;
            }

            var particleObject = ObjectPoolManager.Instance.Get(particlePrefab.gameObject);
            if (particleObject == null)
            {
                return;
            }

            var damagedParticle = particleObject.GetComponent<ParticleSystem>();
            if (damagedParticle == null)
            {
                return;
            }

            damagedParticle.transform.position = targetPosition;
            damagedParticle.transform.rotation = Quaternion.identity;

            var module = damagedParticle.main;
            module.startColor = originColor;
        }
    }
}