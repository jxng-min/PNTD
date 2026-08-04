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
        private bool _isPointerOver;

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
            _isPointerOver = true;
            Show(GetScreenPosition(eventData));
        }

        public void Refresh()
        {
            if (!_isPointerOver)
            {
                return;
            }

            if (_tooltipProvider is not { CanShowTooltip: true })
            {
                TooltipPresenter.Instance.Hide();
                return;
            }

            var content = _tooltipProvider.GetTooltipContent();
            if (content is not { IsValid: true })
            {
                TooltipPresenter.Instance.Hide();
                return;
            }

            TooltipPresenter.Instance.Refresh(content);
        }

        private void Show(Vector2 screenPosition)
        {
            if (_tooltipProvider is not { CanShowTooltip: true })
            {
                TooltipPresenter.Instance.Hide();
                return;
            }
            
            var content = _tooltipProvider.GetTooltipContent();
            if (content is not { IsValid: true })
            {
                TooltipPresenter.Instance.Hide();
                return;
            }

            TooltipPresenter.Instance.Show(content, screenPosition, tooltipOffset);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointerOver = false;
            TooltipPresenter.Instance.Hide();
        }
        
        private void OnDisable()
        {
            _isPointerOver = false;
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
