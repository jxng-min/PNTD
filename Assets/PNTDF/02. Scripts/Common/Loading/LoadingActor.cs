using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JxModule.CharFX;
using TMPro;

namespace PNTD
{
    public class LoadingActor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LoadingManager loadingManager;

        [Header("UI")]
        [SerializeField] private Image loadingImage;
        [SerializeField] private CharFxRunner loadingLabel;
        
        [Header("VFX")]
        [SerializeField] private float beginRadius = 0f;
        [SerializeField] private float endRadius = 1.5f;
        [SerializeField] private float duration = 1f;

        private Material _runtimeMaterial;
        private static readonly int RadiusID = Shader.PropertyToID("_Radius");
        
        private void Awake()
        {
            if (loadingManager == null || loadingImage == null || loadingImage.material == null)
            {
                enabled = false;
                return;
            }

            _runtimeMaterial = Instantiate(loadingImage.material);
            loadingImage.material = _runtimeMaterial;
            
            _runtimeMaterial.SetFloat(RadiusID, beginRadius);

            loadingManager.OnLoadingBegin = BeginLoadRoutine;
            loadingManager.OnLoadingEnd = EndLoadRoutine;
        }

        private IEnumerator BeginLoadRoutine(string loadingText)
        {
            yield return LoadRoutine(true, beginRadius, endRadius, duration, loadingText);
        }

        private IEnumerator EndLoadRoutine()
        {
            yield return LoadRoutine(false, endRadius, beginRadius, duration);
        }

        private IEnumerator LoadRoutine(bool isIn, float from, float to, float targetTime, string loadingText = null)
        {
            if (loadingText != null)
            {
                loadingLabel.SetText(loadingText);
            }
            
            var elapsedTime = 0f;

            while (elapsedTime < targetTime)
            {
                elapsedTime += Time.deltaTime;
                var delta = Mathf.Clamp01(elapsedTime / targetTime);

                var radius = Mathf.Lerp(from, to, delta);
                _runtimeMaterial.SetFloat(RadiusID, radius);

                UpdateLabelAlpha(isIn, radius);

                yield return null;
            }
            
            _runtimeMaterial.SetFloat(RadiusID, to);
            UpdateLabelAlpha(isIn, to);
        }

        private void UpdateLabelAlpha(bool isIn, float radius)
        {
            if (loadingLabel == null)
            {
                return;
            }

            if (isIn && radius > 0f)
            {
                loadingLabel.gameObject.SetActive(true);
            }
            else if(!isIn && radius <= 0.1f)
            {
                loadingLabel.gameObject.SetActive(false);
            }
        }
    }
}