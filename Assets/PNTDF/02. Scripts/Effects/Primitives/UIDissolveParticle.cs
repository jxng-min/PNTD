using JxModule;
using UnityEngine;

namespace PNTD
{
    public class UIDissolveParticle : MonoBehaviour
    {
        [Header("Main Particles")]
        [SerializeField] private ParticleSystem dissolveParticle;
        [SerializeField] private ParticleSystem fragmentParticle;
        
        [Header("Shadow Particles")]
        [SerializeField] private ParticleSystem dissolveShadowParticle;
        [SerializeField] private ParticleSystem fragmentShadowParticle;
        
        private void OnEnable()
        {
            if (fragmentParticle == null || fragmentShadowParticle == null)
            {
                return;
            }
            
            var seed = (uint)Random.Range(1, int.MaxValue);
            ApplySeed(fragmentParticle, seed);
            ApplySeed(fragmentShadowParticle, seed);

            SetScale(1f);
        }

        public void Play()
        {
            RestartParticle(dissolveParticle);
            RestartParticle(fragmentParticle);
            RestartParticle(fragmentShadowParticle);
        }
        
        public void SetColor(Color color)
        {
            var dissolveModule = dissolveParticle.main;
            dissolveModule.startColor = color;
            
            var fragmentModule = fragmentParticle.main;
            fragmentModule.startColor = color;
        }

        public void SetScale(float scale)
        {
            dissolveParticle.transform.localScale = Vector3.one * scale;
            fragmentParticle.transform.localScale = Vector3.one * scale;
            dissolveShadowParticle.transform.localScale = Vector3.one * scale;
            fragmentShadowParticle.transform.localScale = Vector3.one * scale;
        }
        
        private void ApplySeed(ParticleSystem particle, uint seed)
        {
            var main = particle.main;
            main.useUnscaledTime = false;

            particle.useAutoRandomSeed = false;
            particle.randomSeed = seed;
        }

        private void RestartParticle(ParticleSystem particle)
        {
            particle?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particle?.Clear();
            particle?.Play();
        }

        private void OnParticleSystemStopped()
        {
            ObjectPoolManager.Instance.Return(gameObject);
        }
    }
}