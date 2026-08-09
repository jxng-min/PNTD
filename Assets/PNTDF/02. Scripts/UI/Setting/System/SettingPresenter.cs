using JxModule;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PNTD
{
    public class SettingPresenter : MonoBehaviour
    {
        [BigHeader("UI")]
        [SerializeField] private SettingView settingView;

        private PNTDSettingsData _settingsData;

        private void Awake()
        {
            settingView ??= GetComponentInChildren<SettingView>(true);
            BindEvents();
        }

        private void Start()
        {
            _settingsData = PNTDSaveSystem.Settings;
            RefreshView();
        }

        private void BindEvents()
        {
            if (settingView == null)
            {
                return;
            }

            settingView.OnClickedSfxVolume += HandleOnClickedSfxVolume;
            settingView.OnClickedBgmVolume += HandleOnClickedBgmVolume;
            settingView.OnClickedScreenShake += HandleOnClickedScreenShake;
            settingView.OnClickedScreenMovement += HandleOnClickedScreenMovement;
            settingView.OnClickedEnableTooltip += HandleOnClickedEnableTooltip;
            settingView.OnClickedEnableToast += HandleOnClickedEnableToast;
            settingView.OnClickedQuitGame += HandleOnClickedQuitGame;
        }

        private void RefreshView()
        {
            if (settingView == null || _settingsData == null)
            {
                return;
            }

            settingView.SetSfxVolumeText(_settingsData.sfxVolumeLevel);
            settingView.SetBgmVolumeText(_settingsData.bgmVolumeLevel);
            settingView.SetScreenShakeText(_settingsData.enableCameraShake);
            settingView.SetScreenMovementText(_settingsData.enableCameraMovement);
            settingView.SetEnableTooltipText(_settingsData.enableTooltip);
            settingView.SetEnableToastText(_settingsData.enableToast);
            settingView.SetQuitGameText();
        }

        private void SaveAndRefresh()
        {
            PNTDSaveSystem.SaveSettings(_settingsData);
            RefreshView();
        }

        private void HandleOnClickedSfxVolume()
        {
            _settingsData.sfxVolumeLevel = GetNextVolumeLevel(_settingsData.sfxVolumeLevel);
            SaveAndRefresh();
        }

        private void HandleOnClickedBgmVolume()
        {
            _settingsData.bgmVolumeLevel = GetNextVolumeLevel(_settingsData.bgmVolumeLevel);
            SaveAndRefresh();
            FindFirstObjectByType<SoundManager>()?.RefreshVolume();
        }

        private void HandleOnClickedScreenShake()
        {
            _settingsData.enableCameraShake = !_settingsData.enableCameraShake;
            SaveAndRefresh();
        }

        private void HandleOnClickedScreenMovement()
        {
            _settingsData.enableCameraMovement = !_settingsData.enableCameraMovement;
            SaveAndRefresh();
        }

        private void HandleOnClickedEnableTooltip()
        {
            _settingsData.enableTooltip = !_settingsData.enableTooltip;
            SaveAndRefresh();

            if (!_settingsData.enableTooltip)
            {
                TooltipPresenter.Instance?.Hide();
            }
        }

        private void HandleOnClickedEnableToast()
        {
            _settingsData.enableToast = !_settingsData.enableToast;
            SaveAndRefresh();
        }

        private static int GetNextVolumeLevel(int currentLevel)
        {
            return (Mathf.Clamp(currentLevel, 0, 10) + 1) % 11;
        }

        private static void HandleOnClickedQuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnDestroy()
        {
            if (settingView == null)
            {
                return;
            }

            settingView.OnClickedSfxVolume -= HandleOnClickedSfxVolume;
            settingView.OnClickedBgmVolume -= HandleOnClickedBgmVolume;
            settingView.OnClickedScreenShake -= HandleOnClickedScreenShake;
            settingView.OnClickedScreenMovement -= HandleOnClickedScreenMovement;
            settingView.OnClickedEnableTooltip -= HandleOnClickedEnableTooltip;
            settingView.OnClickedEnableToast -= HandleOnClickedEnableToast;
            settingView.OnClickedQuitGame -= HandleOnClickedQuitGame;
        }
    }
}
