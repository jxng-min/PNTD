using JxModule;
using UnityEngine;

namespace PNTD
{
    public class ToastPresenter : LocalSingleton<ToastPresenter>
    {
        [SerializeField] private ToastView toastView;

        public void Show(string toastText, float duration)
        {
            if (!PNTDSaveSystem.Settings.enableToast)
            {
                return;
            }

            toastView.Show(toastText, duration);
        }
    }
}
