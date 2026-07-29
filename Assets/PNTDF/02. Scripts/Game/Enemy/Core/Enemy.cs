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
        
        public Hero HitHero { get; private set; }
        public Collider2D Collider => collider2d;

        public void Initialize(EnemyContext enemyContext, StagePath stagePath, IEnemyProvider enemyProvider)
        {
            
        }
    }
}