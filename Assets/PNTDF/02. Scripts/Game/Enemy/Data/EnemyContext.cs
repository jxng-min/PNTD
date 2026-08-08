using UnityEngine;

namespace PNTD
{
    public class EnemyContext
    {
        public string EnemyID { get; }
        public string DisplayName { get; }
        public Color Color { get; }
        public EEnemyType Type { get; }
        public float MoveSpeed { get; }
        public float MaxHp { get; }
        public float PhysicalDefense { get; }
        public float MagicResistance { get; }
        public float SlowResistance { get; }
        public bool CanDropReward { get; }
        public bool CanDropPlunderGold { get; }
        public bool CountsForWaveClear { get; }
        public EnemyAbilityData AbilityData { get; }

        public bool HasAbility => AbilityData != null;

        public EnemyContext(EnemyDataTableRow enemyDataTableRow, EnemyAbilityData abilityData)
        {
            EnemyID = enemyDataTableRow.rowID;
            DisplayName = enemyDataTableRow.rowID;
            Color = enemyDataTableRow.color;
            Type = enemyDataTableRow.type;
            MoveSpeed = enemyDataTableRow.moveSpeed;
            MaxHp = enemyDataTableRow.maxHp;
            PhysicalDefense = enemyDataTableRow.physicalDefense;
            MagicResistance = enemyDataTableRow.magicResistance;
            SlowResistance = enemyDataTableRow.slowResistance;
            CanDropReward = enemyDataTableRow.grantsKillReward;
            CanDropPlunderGold = enemyDataTableRow.canDropPlunderGold;
            CountsForWaveClear = enemyDataTableRow.countsForWaveClear;
            AbilityData = abilityData;
        }
    }
}
