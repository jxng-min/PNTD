using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    [RequireComponent(typeof(ParticleSystem))]
    public class EnemyDamagedParticle : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private Coroutine _disableCoroutine;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }
        
        private void OnEnable()
        {
            _particleSystem.Play();

            if (_disableCoroutine != null)
            {
                StopCoroutine(_disableCoroutine);
            }

            _disableCoroutine = StartCoroutine(DisableAfterPlayed());
        }

        private IEnumerator DisableAfterPlayed()
        {
            yield return new WaitUntil(() => !_particleSystem.IsAlive(true));

            _disableCoroutine = null;
            ObjectPoolManager.Instance.Return(gameObject);
        }
    }
}