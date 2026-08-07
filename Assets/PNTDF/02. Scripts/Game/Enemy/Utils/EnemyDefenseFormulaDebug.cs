using UnityEngine;

namespace PNTD
{
    public static class EnemyDefenseFormulaDebug
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("PNTD/Debug/Verify Enemy Defense Formula")]
        private static void Verify()
        {
            const float baseDamage = 100f;
            var damageAt15Defense = EnemyHealth.CalculateDamageForDebug(baseDamage, 15f);
            var damageAt80Defense = EnemyHealth.CalculateDamageForDebug(baseDamage, 80f);

            Debug.Log($"Enemy Defense Formula Debug: Defense 15 => {damageAt15Defense:F2} damage from {baseDamage:F0}.");
            Debug.Log($"Enemy Defense Formula Debug: Defense 80 => {damageAt80Defense:F2} damage from {baseDamage:F0}.");
        }
#endif
    }
}
