using JxModule;
using UnityEngine;

namespace PNTD
{
    public class ToastPresenter : LocalSingleton<ToastPresenter>
    {
        [SerializeField] private ToastView toastView;

        public void Show(string toastText, float duration)
        {
            toastView.Show(toastText, duration);
        }
    }
}