using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    [ManagedEffect("Flow", "Slow Motion", 3)]
    public class TimeSlowEffect : MonoBehaviour
    {
        [BigHeader("Settings")]
        [Header("Enemy Reached")]
        [SerializeField] private float reachScale = 0.25f;
        [SerializeField] private float reachDuration = 0.3f;
        
        private Coroutine _slowRoutine;

        public void PlayReachEffect()
        {
            if (_slowRoutine != null)
            {
                StopCoroutine(_slowRoutine);
            }

            _slowRoutine = StartCoroutine(SlowRoutine(reachScale, reachDuration));
        }

        private IEnumerator SlowRoutine(float timeScale, float duration)
        {
            Time.timeScale = Mathf.Clamp01(timeScale);
            
            yield return new WaitForSecondsRealtime(duration);
            
            Time.timeScale = 1f;
            _slowRoutine = null;
        }
        
        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}