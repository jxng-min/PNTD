using System;
using System.Collections.Generic;
using JxModule;
using JxModule.DataTable;
using UnityEngine;

namespace PNTD
{
    public static class PNTDSaveSystem
    {
        private const string SaveDirectory = "PNTD";
        private const string SaveFileName = "Save.json";

        private static PNTDSaveData _cachedSaveData;

        public static int CurrentSeed { get; private set; }
        public static PNTDSettingsData Settings => LoadOrCreate().settingsData;
        public static bool HasGameData => LoadOrCreate().gameData is { hasGameData: true };

        public static PNTDSaveData LoadOrCreate()
        {
            if (_cachedSaveData != null)
            {
                return _cachedSaveData;
            }

            if (!SaveSystem.TryLoad<PNTDSaveData>(SaveFileName, SaveDirectory, out var saveData) || saveData == null)
            {
                saveData = new PNTDSaveData();
            }

            saveData.gameData ??= new PNTDGameSaveData();
            saveData.settingsData ??= new PNTDSettingsData();
            saveData.gameData.party ??= new List<PNTDHeroSaveData>();

            _cachedSaveData = saveData;

            if (_cachedSaveData.gameData.hasGameData)
            {
                CurrentSeed = _cachedSaveData.gameData.currentSeed;
            }
            else
            {
                CurrentSeed = GenerateSeed();
            }

            UnityEngine.Random.InitState(CurrentSeed);
            return _cachedSaveData;
        }

        public static void ApplyGameData(LobbyDomain lobbyDomain, DataTable heroDataTable)
        {
            var saveData = LoadOrCreate();
            var gameData = saveData.gameData;
            if (lobbyDomain == null || gameData is not { hasGameData: true })
            {
                return;
            }

            CurrentSeed = gameData.currentSeed;
            UnityEngine.Random.InitState(CurrentSeed);

            lobbyDomain.StatusSystem.SetState(gameData.currentStage, gameData.currentGold);
            lobbyDomain.ShopSystem.SetState(gameData.shopLevel, gameData.shopExp, gameData.shopIsLock);
            lobbyDomain.PartySystem.Initialize(CreateHeroContexts(gameData.party, heroDataTable));
            lobbyDomain.SynergySystem.RefreshSynergies(lobbyDomain.PartySystem.HeroContexts);
        }

        public static void SaveGameData(LobbyDomain lobbyDomain, bool prettyPrint = true)
        {
            if (lobbyDomain == null)
            {
                return;
            }

            var saveData = LoadOrCreate();
            saveData.gameData = CreateGameData(lobbyDomain);
            Save(saveData, prettyPrint);
        }

        public static void ResetGameData(bool prettyPrint = true)
        {
            var saveData = LoadOrCreate();
            saveData.gameData = new PNTDGameSaveData();
            CurrentSeed = GenerateSeed();
            UnityEngine.Random.InitState(CurrentSeed);
            Save(saveData, prettyPrint);
        }

        public static void SaveSettings(PNTDSettingsData settingsData, bool prettyPrint = true)
        {
            if (settingsData == null)
            {
                return;
            }

            var saveData = LoadOrCreate();
            saveData.settingsData = settingsData;
            Save(saveData, prettyPrint);
        }

        private static void Save(PNTDSaveData saveData, bool prettyPrint)
        {
            _cachedSaveData = saveData;
            SaveSystem.Save(saveData, SaveFileName, SaveDirectory, prettyPrint);
        }

        private static PNTDGameSaveData CreateGameData(LobbyDomain lobbyDomain)
        {
            return new PNTDGameSaveData
            {
                hasGameData = true,
                currentSeed = CurrentSeed,
                currentStage = lobbyDomain.StatusSystem.Stage,
                currentGold = lobbyDomain.StatusSystem.Gold,
                shopLevel = lobbyDomain.ShopSystem.Level,
                shopExp = lobbyDomain.ShopSystem.Exp,
                shopIsLock = lobbyDomain.ShopSystem.IsLock,
                party = CreateHeroSaveData(lobbyDomain.PartySystem.HeroContexts),
            };
        }

        private static List<PNTDHeroSaveData> CreateHeroSaveData(IReadOnlyList<HeroContext> heroContexts)
        {
            var saveData = new List<PNTDHeroSaveData>();
            if (heroContexts == null)
            {
                return saveData;
            }

            foreach (var heroContext in heroContexts)
            {
                if (heroContext?.HeroDataTableRow == null)
                {
                    continue;
                }

                saveData.Add(new PNTDHeroSaveData
                {
                    heroId = heroContext.HeroDataTableRow.rowID,
                    level = heroContext.Level,
                    exp = heroContext.Exp,
                });
            }

            return saveData;
        }

        private static IReadOnlyList<HeroContext> CreateHeroContexts(IReadOnlyList<PNTDHeroSaveData> heroSaveData,
                                                                     DataTable heroDataTable)
        {
            var heroContexts = new List<HeroContext>();
            if (heroSaveData == null || heroDataTable == null)
            {
                return heroContexts;
            }

            foreach (var heroSave in heroSaveData)
            {
                if (heroSave == null || string.IsNullOrWhiteSpace(heroSave.heroId))
                {
                    continue;
                }

                var heroDataTableRow = heroDataTable.Find<HeroDataTableRow>(row => row.isEnable && row.rowID == heroSave.heroId);
                if (heroDataTableRow == null)
                {
                    continue;
                }

                heroContexts.Add(new HeroContext(heroDataTableRow,
                                                Mathf.Clamp(heroSave.level, 1, 3),
                                                Mathf.Clamp(heroSave.exp, 0, 2)));
            }

            return heroContexts;
        }

        private static int GenerateSeed()
        {
            return HashCode.Combine(DateTime.UtcNow.Ticks, Guid.NewGuid());
        }
    }
}
