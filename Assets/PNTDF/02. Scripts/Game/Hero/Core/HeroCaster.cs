using System.Collections.Generic;
using JxModule;
using System.Linq;
using UnityEngine;

namespace PNTD
{
    public class HeroCaster : MonoBehaviour
    {
        [BigHeader("Settings")]
        [SerializeField] private LayerMask enemyLayer;

        private Hero _hero;
        private Collider2D[] _overlapBuffer;

        public void Initialize(Hero hero)
        {
            if (hero == null)
            {
                DebugExtension.LogColor("Hero Caster: Hero is null.", Color.red);
                return;
            }
            
            _hero = hero;
        }

        public Enemy FindNearestTarget()
        {
            var enemies = FindTargetsInRange();

            Enemy nearestEnemy = null;
            var nearestDistanceSqr = float.MaxValue;
            var heroPosition = _hero.transform.position;

            foreach (var enemy in enemies)
            {
                if (!IsValidTarget(enemy))
                {
                    continue;
                }
                
                var distanceSqr = (enemy.transform.position - heroPosition).sqrMagnitude;
                if (distanceSqr >= nearestDistanceSqr)
                {
                    continue;
                }
                
                nearestDistanceSqr = distanceSqr;
                nearestEnemy = enemy;
            }

            return nearestEnemy;
        }

        public List<Enemy> FindVulnerableTargets(int count)
        {
            if (count <= 0)
            {
                return new List<Enemy>();
            }

            var enemies = FindTargetsInRange();
            
            enemies.Sort((a, b) => a.Health.Rate.CompareTo(b.Health.Rate));
            var targets = enemies.GetRange(0, Mathf.Min(count, enemies.Count));

            return targets;
        }

        public List<Enemy> FindLowestCurrentHpTargets(int count)
        {
            if (count <= 0)
            {
                return new List<Enemy>();
            }

            var enemies = FindTargetsInRange()
                .Where(IsValidTarget)
                .Distinct()
                .ToList();

            enemies.Sort((a, b) => a.Health.CurrentHp.CompareTo(b.Health.CurrentHp));
            return enemies.GetRange(0, Mathf.Min(count, enemies.Count));
        }

        public List<Enemy> FindTargetsInRange()
        {
            var enemies = new List<Enemy>();

            if (_hero == null || _hero.Stat == null)
            {
                return enemies;
            }
            
            _overlapBuffer = Physics2D.OverlapCircleAll(transform.position, _hero.Stat.FinalAttackRange, enemyLayer.value);
            
            foreach (var enemy in _overlapBuffer)
            {
                var targetEnemy = enemy.GetComponentInParent<Enemy>();
                if (targetEnemy != null)
                {
                    enemies.Add(targetEnemy);
                }
            }

            return enemies;
        }
        
        private static bool IsValidTarget(Enemy enemy)
        {
            return enemy != null &&
                   enemy.isActiveAndEnabled &&
                   enemy.Health != null;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_hero?.Stat == null)
            {
                return;
            }

            Gizmos.DrawWireSphere(
                transform.position,
                _hero.Stat.FinalAttackRange);
        }
#endif
    }
}
