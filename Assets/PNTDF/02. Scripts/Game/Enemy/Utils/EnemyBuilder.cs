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

        public EnemyBuilder(DataTable enemyDataTable,
                            DataTable enemyAbilityDataTable,
                            DataTable enragerDataTable)
        {
            _enemyDataTable = enemyDataTable;
            _enemyAbilityDataTable = enemyAbilityDataTable;
            _enragerDataTable = enragerDataTable;
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
    }
}