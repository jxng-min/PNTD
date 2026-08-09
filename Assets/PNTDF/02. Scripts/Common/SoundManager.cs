using System;
using System.Collections;
using System.Collections.Generic;
using JxModule;
using JxModule.DataTable;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PNTD
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : GlobalSingleton<SoundManager>
    {
        private AudioSource _bgmSource;
        private AudioSource _sfxPrefab;

        private readonly Dictionary<string, SoundDataTableRow> _soundDict = new();
        private readonly Dictionary<string, List<AudioClip>> _clipDict = new();
        private readonly Dictionary<string, int> _runtimeDict = new();
        private readonly List<AsyncOperationHandle<AudioClip>> _clipHandles = new();

        private string _lastBgmKey;
        private float BgmVolume => PNTDSaveSystem.Settings.BgmVolume;
        private float SfxVolume => PNTDSaveSystem.Settings.SfxVolume;
        
        public bool IsLoaded { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _bgmSource = GetComponent<AudioSource>();
            _sfxPrefab = PrefabManager.CachePrefab<AudioSource>("[PF] Sound Source");
            CacheSoundDataTableRows();
            StartCoroutine(LoadSoundClipsRoutine());
        }

        public void PlayBGM(string soundKey, bool fadeIn = false)
        {
            if (!_soundDict.TryGetValue(soundKey, out var soundDataTableRow))
            {
                DebugExtension.LogColor($"Sound Manager: Can not found SoundDataTableRow. Sound Key: {soundKey}", Color.red);
                return;
            }

            if (soundDataTableRow.type != ESound.BGM)
            {
                DebugExtension.LogColor($"Sound Manager: Can not play SFX to BGM. Sound Key: {soundKey}", Color.red);
                return;
            }

            if (!TryGetAudioClip(soundKey, out var audioClip))
            {
                DebugExtension.LogColor($"Sound Manager: Can not found Audio Clip. Sound Key: {soundKey}", Color.red);
                return;
            }

            if (_lastBgmKey == soundKey)
            {
                return;
            }

            if (fadeIn)
            {
                StartCoroutine(BGMFadeRoutine(soundKey, audioClip, soundDataTableRow.channelCount, soundDataTableRow.loop));
            }
            else
            {
                _lastBgmKey = soundKey;
                _bgmSource.volume = BgmVolume;
                _bgmSource.loop = soundDataTableRow.loop;
                _bgmSource.clip = audioClip;
                _bgmSource.Play();
            }
        }

        public void StopBGM(bool fadeOut = false)
        {
            if (fadeOut)
            {
                StartCoroutine(FadeRoutine(_bgmSource, true, () =>
                {
                    _bgmSource.Stop();
                    _bgmSource.clip = null;
                    _lastBgmKey = string.Empty;
                }));
            }
            else
            {
                _bgmSource.Stop();
                _bgmSource.clip = null;
                _lastBgmKey = string.Empty;
            }
        }

        public void RefreshVolume()
        {
            if (_bgmSource != null)
            {
                _bgmSource.volume = BgmVolume;
            }
        }

        public void PlaySFX(string soundKey)
        {
            if (!_soundDict.TryGetValue(soundKey, out var soundDataTableRow))
            {
                DebugExtension.LogColor($"Sound Manager: Can not found SoundDataTableRow. Sound Key: {soundKey}", Color.red);
                return;
            }

            if (soundDataTableRow.type != ESound.SFX)
            {
                DebugExtension.LogColor($"Sound Manager: Can not play BGM to SFX. Sound Key: {soundKey}", Color.red);
                return;
            }

            if (!TryGetAudioClip(soundKey, out var audioClip))
            {
                DebugExtension.LogColor($"Sound Manager: Can not found Audio Clip. Sound Key: {soundKey}", Color.red);
                return;
            }

            var maxChannelCount = Mathf.Max(1, soundDataTableRow.channelCount);
            if (!_runtimeDict.TryGetValue(soundDataTableRow.rowID, out var targetChannel))
            {
                _runtimeDict[soundDataTableRow.rowID] = 1;
            }
            else
            {
                if (targetChannel >= maxChannelCount)
                {
                    return;
                }
                
                _runtimeDict[soundDataTableRow.rowID]++;
            }

            var sourceObject = ObjectPoolManager.Instance.Get(_sfxPrefab.gameObject);
            if (sourceObject == null)
            {
                DecreaseRuntimeCount(soundDataTableRow.rowID);
                return;
            }
            
            var sfxSource = sourceObject.GetComponent<AudioSource>();
            if (sfxSource == null)
            {
                DecreaseRuntimeCount(soundDataTableRow.rowID);
                ObjectPoolManager.Instance.Return(sourceObject);
                return;
            }
            
            sfxSource.Stop();
            sfxSource.clip = null;
            sfxSource.volume = SfxVolume;
            sfxSource.pitch = 1f;
            sfxSource.clip = audioClip;
            sfxSource.loop = soundDataTableRow.loop;
            sfxSource.Play();
            
            StartCoroutine(ReturnSFX(soundDataTableRow.rowID, sfxSource));
        }

        private void CacheSoundDataTableRows()
        {
            var soundDataTableRows = DataTableManager.FindAllRows<SoundDataTableRow>();
            if (soundDataTableRows == null)
            {
                DebugExtension.LogColor("Sound Manager: Failed to load SoundDataTableRows", Color.red);
                return;
            }

            _soundDict.Clear();
            foreach (var soundDataTableRow in soundDataTableRows)
            {
                _soundDict.Add(soundDataTableRow.rowID, soundDataTableRow);
            }
        }

        private IEnumerator LoadSoundClipsRoutine()
        {
            IsLoaded = false;
            _clipDict.Clear();
            ReleaseClipHandles();

            foreach (var soundDataTableRow in _soundDict.Values)
            {
                if (soundDataTableRow.soundAddresses == null || soundDataTableRow.soundAddresses.Count == 0)
                {
                    DebugExtension.LogColor($"Sound Manager: Sound address is empty. Sound Key: {soundDataTableRow.rowID}", Color.red);
                    continue;
                }

                var audioClips = new List<AudioClip>();
                foreach (var soundAddress in soundDataTableRow.soundAddresses)
                {
                    if (string.IsNullOrWhiteSpace(soundAddress))
                    {
                        continue;
                    }

                    var handle = Addressables.LoadAssetAsync<AudioClip>(soundAddress);
                    yield return handle;

                    if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
                    {
                        audioClips.Add(handle.Result);
                        _clipHandles.Add(handle);
                    }
                    else
                    {
                        DebugExtension.LogColor($"Sound Manager: Failed to load Audio Clip. Sound Key: {soundDataTableRow.rowID}, Address: {soundAddress}", Color.red);
                        Addressables.Release(handle);
                    }
                }

                if (audioClips.Count > 0)
                {
                    _clipDict[soundDataTableRow.rowID] = audioClips;
                }
            }
            
            IsLoaded = true;
        }

        private bool TryGetAudioClip(string soundKey, out AudioClip audioClip)
        {
            audioClip = null;
            if (!_clipDict.TryGetValue(soundKey, out var audioClips) || audioClips == null || audioClips.Count == 0)
            {
                return false;
            }

            audioClip = audioClips.Count == 1
                ? audioClips[0]
                : RandomUtility.GetRandom(audioClips);

            return audioClip != null;
        }

        private IEnumerator BGMFadeRoutine(string soundKey, AudioClip audioClip, int channel, bool loop)
        {
            if (_bgmSource.isPlaying)
            {
                if (_bgmSource.clip != null)
                {
                    if (!string.IsNullOrEmpty(_lastBgmKey) && _runtimeDict.TryGetValue(_lastBgmKey, out var lastChannel))
                    {
                        _runtimeDict[_lastBgmKey] = Mathf.Max(0, lastChannel - 1);
                    }

                }

                yield return StartCoroutine(FadeRoutine(_bgmSource, true));
                yield return new WaitForSeconds(0.2f);
            }

            var maxChannelCount = Mathf.Max(1, channel);
            if (_runtimeDict.TryGetValue(soundKey, out var targetChannel))
            {
                if (targetChannel >= maxChannelCount)
                {
                    yield break;
                }
                
                _runtimeDict[soundKey]++;
            }
            else
            {
                _runtimeDict[soundKey] = 1;
            }
            
            _lastBgmKey = soundKey;
            _bgmSource.loop = loop;
            _bgmSource.clip = audioClip;
            _bgmSource.Play();

            yield return StartCoroutine(FadeRoutine(_bgmSource, false));
        }

        private IEnumerator FadeRoutine(AudioSource source, bool isFadeOut, Action callback = null)
        {
            var elapsedTime = 0f;
            var targetTime = 0.3f;
            var targetVolume = BgmVolume;

            while (elapsedTime < targetTime)
            {
                var delta =  elapsedTime / targetTime;
                source.volume = isFadeOut ? Mathf.Lerp(targetVolume, 0f, delta) : Mathf.Lerp(0f, targetVolume, delta);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            source.volume = isFadeOut ? 0f : targetVolume;
            callback?.Invoke();
        }
        
        private IEnumerator ReturnSFX(string soundKey, AudioSource sfxSource)
        {
            while (sfxSource.isPlaying)
            {
                yield return null;
            }
            
            DecreaseRuntimeCount(soundKey);
            ObjectPoolManager.Instance.Return(sfxSource.gameObject);
        }

        private void DecreaseRuntimeCount(string soundKey)
        {
            if (!_runtimeDict.TryGetValue(soundKey, out var count))
            {
                return;
            }

            _runtimeDict[soundKey] = Mathf.Max(0, count - 1);
        }

        private void ReleaseClipHandles()
        {
            foreach (var clipHandle in _clipHandles)
            {
                if (clipHandle.IsValid())
                {
                    Addressables.Release(clipHandle);
                }
            }

            _clipHandles.Clear();
        }

        private void OnDestroy()
        {
            ReleaseClipHandles();
        }
    }
}
