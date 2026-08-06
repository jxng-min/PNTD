using JxModule;
using UnityEngine;

namespace PNTD
{
    public class DeployPreviewView : ViewBase
    {
        [SerializeField] private LabelView manualLabel;
        [SerializeField] private SpriteRenderer virtualRenderer;

        private void Awake()
        {
            CacheReferences();
        }

        public void Initialize(HeroDataTableRow heroDataTableRow)
        {
            CacheReferences();
            SetColor(heroDataTableRow != null ? heroDataTableRow.color : Color.white);
        }

        public void Show(bool withVirtualHero = true)
        {
            manualLabel.CanvasGroup.alpha = 1f;

            if (withVirtualHero)
            {
                SetRendererActive(true);
            }
        }

        public void Hide()
        {
            manualLabel.CanvasGroup.alpha = 0f;
            SetRendererActive(false);
        }

        public void SetWorldPosition(Vector3 worldPosition)
        {
            virtualRenderer.transform.position = worldPosition;
        }

        public void SetColor(Color color)
        {
            CacheReferences();

            if (virtualRenderer != null)
            {
                virtualRenderer.color = color;
            }
        }

        private void CacheReferences()
        {
            if (virtualRenderer != null)
            {
                return;
            }
            
            virtualRenderer = GetComponentInChildren<SpriteRenderer>(true);
        }

        private void SetRendererActive(bool isActive)
        {
            CacheReferences();

            if (virtualRenderer != null)
            {
                virtualRenderer.gameObject.SetActive(isActive);
            }
        }
    }
}
