using System;
using UnityEngine;

namespace PNTD
{
    public class EnemyAbiliter : MonoBehaviour
    {
        private Enemy _owner;
        private EnemyAbility _ability;

        public void Initialize(Enemy owner, 
                               EnemyAbilityData abilityData, 
                               IEnemyProvider enemyProvider,
                               IEnemySpawner enemySpawner,
                               IHeroProvider heroProvider)
        {
            Release();
            
            _owner = owner;

            if (abilityData == null)
            {
                return;
            }
            
            _ability = EnemyAbilityFactory.Create(abilityData);
            if (_ability == null)
            {
                return;
            }

            var abilityContext = new EnemyAbilityContext(_owner, enemyProvider, enemySpawner, heroProvider);
            _ability.Initialize(abilityContext);
        }

        private void Update()
        {
            if (_owner == null || _ability == null)
            {
                return;
            }

            var isCooldownPaused = _owner.Status.IsAbilityDisabled;
            _ability.Tick(Time.deltaTime, isCooldownPaused);
        }

        private void FixedUpdate()
        {
            _ability?.FixedTick(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            _ability?.LateTick(Time.deltaTime);
        }

        public void Release()
        {
            _ability?.Release();
            
            _owner = null;
            _ability = null;
        }

        private void OnDisable()
        {
            Release();
        }
    }
}
