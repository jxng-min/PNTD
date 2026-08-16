using System;
using System.Collections.Generic;
using JxModule;

namespace PNTD
{
    [Serializable]
    public class PNTDSaveData : ISerializeData
    {
        public PNTDGameSaveData gameData = new();
        public PNTDSettingsData settingsData = new();
    }

    [Serializable]
    public class PNTDGameSaveData
    {
        public bool hasGameData;
        public int currentSeed;
        public int currentStage = 1;
        public int currentGold = 5;
        public int loopCount;
        public int shopLevel = 1;
        public int shopExp;
        public bool shopIsLock;
        public List<PNTDHeroSaveData> party = new();
    }

    [Serializable]
    public class PNTDHeroSaveData
    {
        public string heroId;
        public int level = 1;
        public int exp;
    }

    [Serializable]
    public class PNTDSettingsData
    {
        public int sfxVolumeLevel = 5;
        public int bgmVolumeLevel = 5;
        public bool enableCameraShake = true;
        public bool enableCameraMovement = true;
        public bool enableTooltip = true;
        public bool enableToast = true;

        public float SfxVolume => ToVolume(sfxVolumeLevel);
        public float BgmVolume => ToVolume(bgmVolumeLevel);

        private static float ToVolume(int volumeLevel)
        {
            return Math.Clamp(volumeLevel, 0, 10) * 0.1f;
        }
    }
}
