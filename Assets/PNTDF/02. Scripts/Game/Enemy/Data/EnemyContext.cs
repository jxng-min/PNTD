using UnityEngine;

namespace PNTD
{
    public class EnemyContext
    {
        public string EnemyID { get; }
        public Color Color { get; }
        public float MoveSpeed { get; }
        public float MaxHp { get; }
        public float PhysicalDefense { get; }
        public float MagicResistance { get; }
        public float SlowResistance { get; }
        public EnemyAbilityData AbilityData { get; }

        public bool HasAbility => AbilityData != null;

        public EnemyContext(EnemyDataTableRow enemyDataTableRow, EnemyAbilityData abilityData)
        {
            EnemyID = enemyDataTableRow.rowID;
            Color = enemyDataTableRow.color;
            MoveSpeed = enemyDataTableRow.moveSpeed;
            MaxHp = enemyDataTableRow.maxHp;
            PhysicalDefense = enemyDataTableRow.physicalDefense;
            MagicResistance = enemyDataTableRow.magicResistance;
            SlowResistance = enemyDataTableRow.slowResistance;
            AbilityData = abilityData;
        }
    }
}