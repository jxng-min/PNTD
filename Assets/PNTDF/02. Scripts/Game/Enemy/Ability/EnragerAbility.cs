using UnityEngine;

namespace PNTD
{
    public class EnragerAbility : EnemyAbility
    {
        private EnemyAbilityContext _abilityContext;
        private readonly EnragerData _enragerData;

        public EnragerAbility(EnragerData enragerData)
        {
            _enragerData = enragerData;
        }

        public override void Initialize(EnemyAbilityContext abilityContext)
        {
            Release();
            
            _abilityContext = abilityContext;
            if (_abilityContext.Owner?.Health != null)
            {
                _abilityContext.Owner.Health.OnEnemyDied += HandleOnEnemyDied;
            }
        }

        public override void Release()
        {
            if (_abilityContext == null)
            {
                return;
            }
            
            if (_abilityContext.Owner?.Health != null)
            {
                _abilityContext.Owner.Health.OnEnemyDied -= HandleOnEnemyDied;
            }

            _abilityContext = null;
        }

        private void HandleOnEnemyDied()
        {
            if (_enragerData == null)
            {
                return;
            }

            foreach (var enemy in _abilityContext.EnemyProvider.AliveEnemies)
            {
                if (enemy == null)
                {
                    continue;
                }

                var distance = Vector2.Distance(_abilityContext.Owner.transform.position, enemy.transform.position);
                if (distance > _enragerData.Radius)
                {
                    continue;
                }

                enemy.Status?.AddMoveSpeedEffect("Enrager Died",
                                                 _enragerData.MoveSpeedMultiplier,
                                                 _enragerData.BoostedDuration,
                                                 _enragerData.OverrideColor);
            }
        }
    }
}