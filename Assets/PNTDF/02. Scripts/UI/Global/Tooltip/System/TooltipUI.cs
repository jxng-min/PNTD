using JxModule;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PNTD
{
    public class TooltipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [BigHeader("Settings")]
        [SerializeField] private Vector2 tooltipOffset;
        
        private ITooltipProvider _tooltipProvider;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _tooltipProvider = GetComponent<ITooltipProvider>();
            _rectTransform = GetComponent<RectTransform>();

            if (_tooltipProvider == null)
            {
                enabled = false;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_tooltipProvider is not { CanShowTooltip: true })
            {
                return;
            }
            
            var content = _tooltipProvider.GetTooltipContent();
            if (content is not { IsValid: true })
            {
                return;
            }

            var screenPosition = GetScreenPosition(eventData);

            TooltipPresenter.Instance.Show(content, screenPosition, tooltipOffset);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipPresenter.Instance.Hide();
        }
        
        private void OnDisable()
        {
            TooltipPresenter.Instance.Hide();
        }
        
        private Vector2 GetScreenPosition(PointerEventData eventData)
        {
            if (_rectTransform == null)
            {
                return eventData.position;
            }

            return RectTransformUtility.WorldToScreenPoint(eventData.enterEventCamera, _rectTransform.position);
        }
    }
}