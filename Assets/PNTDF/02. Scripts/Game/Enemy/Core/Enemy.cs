using System;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class Enemy : MonoBehaviour
    {
        [BigHeader("Enemy Core")]
        [SerializeField] private EnemyStatus status;
        [SerializeField] private EnemyMovement movement;
        [SerializeField] private EnemyHealth health;
        [SerializeField] private EnemyModel model;
        [SerializeField] private EnemyAbiliter abiliter;
        
        [Space(30f)]
        [BigHeader("References")]
        [SerializeField] private Collider2D collider2d;
        
        public event Action<Enemy> OnEnemyDied;
        public event Action<Enemy> OnDestinationReached;
        
        public EnemyContext Context { get; private set; }
        public bool IsResolved { get; private set; }

        public EnemyStatus Status => status;
        public EnemyMovement Movement => movement;
        public EnemyHealth Health => health;
        public EnemyModel Model => model;
        
        public Hero LastHitHero { get; private set; }
        public Collider2D Collider => collider2d;

        public void Initialize(EnemyContext enemyContext, StagePath stagePath, IEnemyProvider enemyProvider)
        {
            Context = enemyContext;
            IsResolved = false;
            LastHitHero = null;
            
            status.Initialize(
                this, 
                enemyContext.MoveSpeed, 
                enemyContext.PhysicalDefense, 
                enemyContext.MagicResistance, 
                enemyContext.SlowResistance
            );
            
            health.Initialize(
                status,
                enemyContext.MaxHp
            );
            
            movement.Initialize(
                status,
                stagePath
            );
            
            model.Initialize(
                enemyContext.Color,
                health,
                status
            );
            
            abiliter.Initialize(
                this,
                enemyContext.AbilityData,
                enemyProvider
            );
            
            health.OnEnemyDied += HandleOnEnemyDied;
            movement.OnDestinationReached += HandleOnDestinationReached;
        }
        
        public void SetLastHitHero(Hero hero)
        {
            LastHitHero = hero;
        }

        private void HandleOnEnemyDied()
        {
            if (IsResolved)
            {
                return;
            }
            
            IsResolved = true;
            
            OnEnemyDied?.Invoke(this);
            ObjectPoolManager.Instance.Return(gameObject);
        }

        private void HandleOnDestinationReached()
        {
            if (IsResolved)
            {
                return;
            }
            
            IsResolved = true;
            
            OnDestinationReached?.Invoke(this);
            ObjectPoolManager.Instance.Return(gameObject);
        }

        private void OnDisable()
        {
            health.OnEnemyDied -= HandleOnEnemyDied;
            movement.OnDestinationReached -= HandleOnDestinationReached;
        }
    }
}