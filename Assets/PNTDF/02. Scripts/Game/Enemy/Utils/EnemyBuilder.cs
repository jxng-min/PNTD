using UnityEngine;
using JxModule;
using JxModule.DataTable;

namespace PNTD
{
    public class EnemyBuilder
    {
        private readonly DataTable _enemyDataTable;
        private readonly DataTable _enemyAbilityDataTable;
        private readonly DataTable _enragerDataTable;
        private readonly DataTable _blinkerDataTable;
        private readonly DataTable _hexerDataTable;
        private readonly DataTable _summonerDataTable;

        public EnemyBuilder(DataTable enemyDataTable,
                            DataTable enemyAbilityDataTable,
                            DataTable enragerDataTable,
                            DataTable blinkerDataTable,
                            DataTable hexerDataTable,
                            DataTable summonerDataTable)
        {
            _enemyDataTable = enemyDataTable;
            _enemyAbilityDataTable = enemyAbilityDataTable;
            _enragerDataTable = enragerDataTable;
            _blinkerDataTable = blinkerDataTable;
            _hexerDataTable = hexerDataTable;
            _summonerDataTable = summonerDataTable;
        }

        public EnemyContext Build(string enemyId)
        {
            var enemyDataTableRow = _enemyDataTable.Find<EnemyDataTableRow>(row => row.isEnable && row.rowID == enemyId);
            if (enemyDataTableRow == null)
            {
                DebugExtension.LogColor($"Enemy Builder: Enemy not found. Enemy ID: {enemyId}", Color.red);
                return null;
            }

            var abilityContext = BuildAbility(enemyDataTableRow.abilityID);

            return new EnemyContext(enemyDataTableRow, abilityContext);
        }

        private EnemyAbilityData BuildAbility(string abilityId)
        {
            if (string.IsNullOrEmpty(abilityId))
            {
                return null;
            }
            
            var abilityDataTableRow = _enemyAbilityDataTable.Find<EnemyAbilityDataTableRow>(row => row.isEnable && row.rowID == abilityId);
            if (abilityDataTableRow == null)
            {
                DebugExtension.LogColor($"Enemy Builder: Ability not found. Ability ID: {abilityId}", Color.red);
                return null;
            }
            
            return abilityDataTableRow.ability switch
            {
                EEnemyAbility.Enrager       => BuildEnragerAbility(abilityId),
                EEnemyAbility.Blinker       => BuildBlinkerAbility(abilityId),
                EEnemyAbility.Hexer         => BuildHexerAbility(abilityId),
                EEnemyAbility.Summoner      => BuildSummonerAbility(abilityId),
                _                           => null
            };
        }

        private EnemyAbilityData BuildEnragerAbility(string abilityId)
        {
            var enragerDataTableRow = _enragerDataTable.Find<EnragerDataTableRow>(row => row.isEnable && row.rowID == abilityId);
            if (enragerDataTableRow == null)
            {
                DebugExtension.LogColor($"EnemyBuilder: Enrager ability not found. Ability ID: {abilityId}", Color.red);
                return null;
            }

            return new EnragerData(
                enragerDataTableRow.rowID,
                enragerDataTableRow.radius,
                enragerDataTableRow.moveSpeedMultiplier,
                enragerDataTableRow.boostedDuration,
                enragerDataTableRow.recoverDuration,
                enragerDataTableRow.overrideColor
            );
        }

        private EnemyAbilityData BuildBlinkerAbility(string abilityId)
        {
            var row = _blinkerDataTable?.Find<BlinkerDataTableRow>(data => data.isEnable && data.rowID == abilityId);
            if (row == null)
            {
                DebugExtension.LogColor($"EnemyBuilder: Blinker ability not found. Ability ID: {abilityId}", Color.red);
                return null;
            }

            return new BlinkerData(row.rowID,
                                   row.firstCastDelay,
                                   row.coolDown,
                                   row.blinkDistance,
                                   row.invincibleDuration);
        }

        private EnemyAbilityData BuildHexerAbility(string abilityId)
        {
            var row = _hexerDataTable?.Find<HexerDataTableRow>(data => data.isEnable && data.rowID == abilityId);
            if (row == null)
            {
                DebugExtension.LogColor($"EnemyBuilder: Hexer ability not found. Ability ID: {abilityId}", Color.red);
                return null;
            }

            return new HexerData(row.rowID,
                                 row.firstCastDelay,
                                 row.coolDown,
                                 row.castRange,
                                 row.duration,
                                 row.attackDamagePenalty,
                                 row.attackSpeedPenalty,
                                 row.maxActiveHexPerHexer);
        }

        private EnemyAbilityData BuildSummonerAbility(string abilityId)
        {
            var row = _summonerDataTable?.Find<SummonerDataTableRow>(data => data.isEnable && data.rowID == abilityId);
            if (row == null)
            {
                DebugExtension.LogColor($"EnemyBuilder: Summoner ability not found. Ability ID: {abilityId}", Color.red);
                return null;
            }

            return new SummonerData(row.rowID,
                                    row.firstCastDelay,
                                    row.coolDown,
                                    row.spawnCount,
                                    row.maxCasts,
                                    row.spawnedEnemyID,
                                    row.minSpawnOffset,
                                    row.maxSpawnOffset);
        }
    }
}
