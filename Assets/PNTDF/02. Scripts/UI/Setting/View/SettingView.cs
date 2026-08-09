using System;
using JxModule;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PNTD
{
    public class SettingView : ViewBase
    {
        [BigHeader("UI")]
        [SerializeField] private JxButton sfxVolumeButton;
        [SerializeField] private JxButton bgmVolumeButton;
        [SerializeField] private JxButton screenShakeButton;
        [SerializeField] private JxButton screenMovementButton;
        [SerializeField] private JxButton enableTooltipButton;
        [SerializeField] private JxButton enableToastButton;
        [SerializeField] private JxButton quitGameButton;

        public event Action OnClickedSfxVolume;
        public event Action OnClickedBgmVolume;
        public event Action OnClickedScreenShake;
        public event Action OnClickedScreenMovement;
        public event Action OnClickedEnableTooltip;
        public event Action OnClickedEnableToast;
        public event Action OnClickedQuitGame;

        private void Awake()
        {
            sfxVolumeButton?.AddListener(HandleOnClickedSfxVolume);
            bgmVolumeButton?.AddListener(HandleOnClickedBgmVolume);
            screenShakeButton?.AddListener(HandleOnClickedScreenShake);
            screenMovementButton?.AddListener(HandleOnClickedScreenMovement);
            enableTooltipButton?.AddListener(HandleOnClickedEnableTooltip);
            enableToastButton?.AddListener(HandleOnClickedEnableToast);
            quitGameButton?.AddListener(HandleOnClickedQuitGame);
            Hide();
        }

        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                return;
            }

            Toggle();
        }

        public void Show()
        {
            CanvasGroup.Show();
        }

        public void Hide()
        {
            CanvasGroup.Hide();
        }

        public void Toggle()
        {
            if (CanvasGroup.alpha > 0f)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        public void SetSfxVolumeText(int volumeLevel)
        {
            SetButtonText(sfxVolumeButton, $"sfx volume: {volumeLevel}");
        }

        public void SetBgmVolumeText(int volumeLevel)
        {
            SetButtonText(bgmVolumeButton, $"bgm volume: {volumeLevel}");
        }

        public void SetScreenShakeText(bool isEnabled)
        {
            SetButtonText(screenShakeButton, $"screen shake: {ToYesNo(isEnabled)}");
        }

        public void SetScreenMovementText(bool isEnabled)
        {
            SetButtonText(screenMovementButton, $"screen movement: {ToYesNo(isEnabled)}");
        }

        public void SetEnableTooltipText(bool isEnabled)
        {
            SetButtonText(enableTooltipButton, $"enable tooltip: {ToYesNo(isEnabled)}");
        }

        public void SetEnableToastText(bool isEnabled)
        {
            SetButtonText(enableToastButton, $"enable toast: {ToYesNo(isEnabled)}");
        }

        public void SetQuitGameText()
        {
            SetButtonText(quitGameButton, "quit game");
        }

        private static void SetButtonText(JxButton button, string text)
        {
            if (button == null)
            {
                return;
            }

            var label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                label.text = text;
            }
        }

        private static string ToYesNo(bool value)
        {
            return value ? "yes" : "no";
        }

        private void HandleOnClickedSfxVolume()
        {
            OnClickedSfxVolume?.Invoke();
        }

        private void HandleOnClickedBgmVolume()
        {
            OnClickedBgmVolume?.Invoke();
        }

        private void HandleOnClickedScreenShake()
        {
            OnClickedScreenShake?.Invoke();
        }

        private void HandleOnClickedScreenMovement()
        {
            OnClickedScreenMovement?.Invoke();
        }

        private void HandleOnClickedEnableTooltip()
        {
            OnClickedEnableTooltip?.Invoke();
        }

        private void HandleOnClickedEnableToast()
        {
            OnClickedEnableToast?.Invoke();
        }

        private void HandleOnClickedQuitGame()
        {
            OnClickedQuitGame?.Invoke();
        }

        private void OnDestroy()
        {
            sfxVolumeButton?.RemoveListener(HandleOnClickedSfxVolume);
            bgmVolumeButton?.RemoveListener(HandleOnClickedBgmVolume);
            screenShakeButton?.RemoveListener(HandleOnClickedScreenShake);
            screenMovementButton?.RemoveListener(HandleOnClickedScreenMovement);
            enableTooltipButton?.RemoveListener(HandleOnClickedEnableTooltip);
            enableToastButton?.RemoveListener(HandleOnClickedEnableToast);
            quitGameButton?.RemoveListener(HandleOnClickedQuitGame);
        }
    }
}
