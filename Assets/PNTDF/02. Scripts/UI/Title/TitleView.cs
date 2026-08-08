using JxModule;
using UnityEngine.EventSystems;

namespace PNTD
{
    public class TitleView : ViewBase
    {
        public void Show()
        {
            CanvasGroup.Show();
        }

        public void Hide()
        {
            CanvasGroup.Hide();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            TitlePresenter.RequestTitleClick();
        }
    }
}
