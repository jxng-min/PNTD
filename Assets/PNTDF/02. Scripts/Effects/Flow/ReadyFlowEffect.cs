using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    [ManagedEffect("Flow", "Ready Flow", 0)]
    public class ReadyFlowEffect : MonoBehaviour
    {
        [BigHeader("References")]
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private RectTransform particleRoot;
        
        [Space(30f)]
        [BigHeader("Settings")]
        [Header("Ready")]
        [SerializeField] private Color readyParticleColor = Color.black;
        [SerializeField] private float readyParticleScale = 1f;
    
        [Header("Count")]
        [SerializeField] private Color countParticleColor = Color.red;
        [SerializeField] private float countParticleScale = 2f;

        public IEnumerator PlayReadyFlowEffect(int delay, 
                                               CanvasGroup readyGroup, 
                                               ShadowableLabel readyLabel,
                                               ShadowableLabel countLabel)
        {
            readyGroup.alpha = 1f;
            readyGroup.blocksRaycasts = false;
            readyGroup.interactable = false;

            for (var second = delay; second > 0; second--)
            {
                SoundManager.Instance.PlaySFX("SFX_Count");
                countLabel.SetText($"{second}");
                yield return new WaitForSeconds(1f);
            }

            readyGroup.Hide();
            
            CreateDissolveParticle(readyParticleColor, readyParticleScale, readyLabel.transform as RectTransform);
            CreateDissolveParticle(countParticleColor, countParticleScale, countLabel.transform as RectTransform);
        }

        private void CreateDissolveParticle(Color color, float scale, RectTransform rectTransform)
        {
            var particlePrefab = PrefabManager.CachePrefab<UIDissolveParticle>("[PF] UI Dissolve Particle");
            if (particlePrefab == null)
            {
                return;
            }
            
            var particleObject = ObjectPoolManager.Instance.Get(particlePrefab.gameObject);
            if (particleObject == null)
            {
                return;
            }

            var dissolveParticle = particleObject.GetComponent<UIDissolveParticle>();
            if (dissolveParticle == null)
            {
                return;
            }
            
            if (particleObject.transform is RectTransform particleRectTransform &&
                TryGetLocalPosition(rectTransform, out var localPosition))
            {
                particleRectTransform.anchoredPosition = localPosition;
            }
            else
            {
                particleObject.transform.position = rectTransform.position;
            }

            dissolveParticle.SetColor(color);
            dissolveParticle.SetScale(scale);
            dissolveParticle.Play();
        }
        
        private bool TryGetLocalPosition(RectTransform targetRectTransform, out Vector2 localPosition)
        {
            var canvasCamera = GetCanvasCamera();
            var screenPosition = RectTransformUtility.WorldToScreenPoint(canvasCamera, targetRectTransform.position);
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(particleRoot, screenPosition, canvasCamera, out localPosition);
        }
        
        private Camera GetCanvasCamera()
        {
            if (rootCanvas == null)
            {
                return null;
            }

            return rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;
        }
    }
}