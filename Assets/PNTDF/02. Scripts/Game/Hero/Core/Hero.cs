using JxModule;
using UnityEngine;

namespace PNTD
{
    public class Hero : MonoBehaviour
    {
        [BigHeader("Hero Core")]
        [SerializeField] private HeroAttack attack;
        [SerializeField] private HeroCaster caster;
        [SerializeField] private HeroModel model;
        [SerializeField] private HeroDragger dragger;
        [SerializeField] private HeroEffector effector;
        
        private bool _isInitialized;

        public HeroDataTableRow HeroDataTableRow { get; private set; }
        public int Level { get; private set; }
        
        public HeroStat Stat { get; private set; }
        public HeroSkill Skill { get; private set; }
        public HeroAttack Attack => attack;
        public HeroCaster Caster => caster;
        public HeroModel Model => model;
        public HeroDragger Dragger => dragger;
        public HeroEffector Effector => effector;

        public bool IsSkillSealed => Effector != null && Effector.IsSkillSealed;

        public void Initialize(HeroDataTableRow heroDataTableRow,
                               HeroStat heroStat,
                               HeroSkill heroSkill,
                               int level)
        {
            if (heroDataTableRow == null)
            {
                DebugExtension.LogColor($"Hero: HeroDataTableRow is null.", Color.red);
                return;
            }

            if (heroStat == null)
            {
                DebugExtension.LogColor($"Hero: HeroStat is null.", Color.red);
                return;
            }

            if (heroSkill == null)
            {
                DebugExtension.LogColor($"Hero: HeroSkill is null.", Color.red);
                return;
            }

            Release();

            HeroDataTableRow = heroDataTableRow;
            Stat = heroStat;
            Skill = heroSkill;
            Level = level;
            
            Effector?.Initialize(this);
            if (Effector != null)
            {
                Effector.OnSkillSealChanged += HandleOnSkillSealChanged;
            }

            Caster?.Initialize(this);
            Attack?.Initialize(this);
            Model?.Initialize(this);
            Dragger?.Initialize(this);
            Skill?.Attach(this);
            
            _isInitialized = true;
            HandleOnSkillSealChanged(IsSkillSealed);
        }

        public bool TryAddEffect(HeroEffect heroEffect)
        {
            return Effector != null && Effector.TryAdd(heroEffect);
        }

        public bool TryRemoveEffect(HeroEffect heroEffect)
        {
            return Effector != null && Effector.TryRemove(heroEffect);
        }

        public void ClearEffects()
        {
            Effector?.Clear();
        }

        public bool TryDamagedToEnemy(Enemy enemy, 
                                      DamageContext damageContext, 
                                      bool triggerOnHitEffect = true)
        {
            if (!_isInitialized || enemy == null || enemy.Health == null)
            {
                return false;
            }
            
            enemy.SetLastHitHero(this);
            enemy.Health.TakeDamage(damageContext);

            if (triggerOnHitEffect)
            {
                NotifyHitEnemy(enemy);
            }

            return true;
        }

        public void NotifyHitEnemy(Enemy enemy)
        {
            if (enemy == null)
            {
                return;
            }
            
            Effector?.NotifyHitEnemy(enemy);
        }

        public void Release()
        {
            if (Effector != null)
            {
                Effector.OnSkillSealChanged -= HandleOnSkillSealChanged;

                if (_isInitialized)
                {
                    Effector.Clear();
                }
            }
            
            HeroDataTableRow = null;
            Level = 0;
            Stat = null;
            Skill = null;
            _isInitialized = false;
        }
        
        private void HandleOnSkillSealChanged(bool isSealed)
        {
            Attack?.SetSealed(isSealed);
            Model?.SetSealed(isSealed);
        }

        private void OnDisable()
        {
            Release();
        }
    }
}