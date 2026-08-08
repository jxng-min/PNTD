using System.Collections;
using JxModule;
using UnityEngine;

namespace PNTD
{
    [ManagedEffect("Flow", "Clear Flow", 1)]
    public class ClearFlowEffect : MonoBehaviour
    {
        [BigHeader("References")]
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private RectTransform particleRoot;
        
        [Space(30f)]
        [BigHeader("Settings")]
        [Header("Clear")]
        [SerializeField] private Color clearParticleColor = Color.red;
        [SerializeField] private float clearParticleScale = 3f;
        [SerializeField] private float typingSpeed = 4f;
        [SerializeField] private float delayTime = 1f;

        public IEnumerator PlayClearFlowEffect(CanvasGroup clearGroup, ShadowableLabel clearLabel)
        {
            clearGroup.alpha = 1f;
            clearGroup.interactable = false;
            clearGroup.blocksRaycasts = false;

            yield return clearLabel.TypeRoutine(typingSpeed);
            
            clearGroup.Hide();
            
            CreateDissolveParticle(clearParticleColor, clearParticleScale, clearLabel.transform as RectTransform);
            SoundManager.Instance.PlaySFX("SFX_Dissolve");
            
            yield return new WaitForSeconds(delayTime);
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