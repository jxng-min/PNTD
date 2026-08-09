using System.Collections.Generic;
using System.Linq;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class Hero : MonoBehaviour, ITooltipProvider
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
        public bool IsSummoned { get; private set; }
        public Hero Summoner { get; private set; }
        
        public HeroStat Stat { get; private set; }
        public HeroSkill Skill { get; private set; }
        public HeroAttack Attack => attack;
        public HeroCaster Caster => caster;
        public HeroModel Model => model;
        public HeroDragger Dragger => dragger;
        public HeroEffector Effector => effector;

        public bool IsSkillSealed => Effector != null && Effector.IsSkillSealed;
        public bool CanShowTooltip => _isInitialized && HeroDataTableRow != null && Stat != null && !HeroDragger.AnyDragging;

        public void Initialize(HeroDataTableRow heroDataTableRow,
                               HeroStat heroStat,
                               HeroSkill heroSkill,
                               int level,
                               bool isSummoned = false,
                               Hero summoner = null)
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
            IsSummoned = isSummoned;
            Summoner = summoner;
            
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

        public float ModifyDamageToEnemy(Enemy enemy, float damage)
        {
            return Effector != null
                ? Effector.ModifyDamageToEnemy(enemy, damage)
                : damage;
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

            var sourcedDamageContext = damageContext.SourceHero == null
                ? damageContext.WithSource(this)
                : damageContext;
            enemy.Health.TakeDamage(sourcedDamageContext);

            if (triggerOnHitEffect)
            {
                NotifyHitEnemy(enemy);
                NotifyAffectedEnemy(enemy);
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
        
        public void NotifyAffectedEnemy(Enemy enemy)
        {
            if (enemy == null)
            {
                return;
            }
            
            Effector?.NotifyAffectedEnemy(enemy);
        }
        
        public void NotifyDeployed(Vector3Int cellPosition)
        {
            if (!_isInitialized || Skill == null)
            {
                return;
            }
            
            Skill.OnDeployed(this, cellPosition);
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
            
            Skill?.Release(this);
            HeroDataTableRow = null;
            Level = 0;
            IsSummoned = false;
            Summoner = null;
            Stat = null;
            Skill = null;
            _isInitialized = false;
        }

        public TooltipContent GetTooltipContent()
        {
            if (!CanShowTooltip)
            {
                return null;
            }

            var heroColor = ColorUtility.ToHtmlStringRGB(HeroDataTableRow.color);
            return new TooltipContent(
                "Hero",
                new Dictionary<string, object>
                {
                    { "heroName", $"<color=#{heroColor}>{HeroDataTableRow.displayName}</color>" },
                    { "heroLevel", Level },
                    { "heroSynergies", BuildHeroSynergies(HeroDataTableRow.synergy) },
                    { "physicalAttackIcon", "<sprite=\"Stat\" name=\"Physical ATK\">" },
                    { "magicAttackIcon", "<sprite=\"Stat\" name=\"Magic ATK\">" },
                    { "cooldownIcon", "<sprite=\"Stat\" name=\"Cooldown\">" },
                    { "rangeIcon", "<sprite=\"Stat\" name=\"Range\">" },
                    { "physicalPenetrationIcon", "<sprite=\"Stat\" name=\"Physical Penetration\">" },
                    { "magicPenetrationIcon", "<sprite=\"Stat\" name=\"Magic Penetration\">" },
                    { "finalPhysicalAttackPower", Stat.FinalPhysicalAttackPower },
                    { "finalMagicAttackPower", Stat.FinalMagicAttackPower },
                    { "finalAttackCooldown", Stat.FinalAttackCooldown },
                    { "finalAttackRange", Stat.FinalAttackRange },
                    { "finalFlatPhysicalPenetration", Stat.FinalFlatPhysicalPenetration },
                    { "finalPercentPhysicalPenetration", FormatPercent(Stat.FinalPercentPhysicalPenetration) },
                    { "finalFlatMagicPenetration", Stat.FinalFlatMagicPenetration },
                    { "finalPercentMagicPenetration", FormatPercent(Stat.FinalPercentMagicPenetration) },
                });
        }
        
        private void HandleOnSkillSealChanged(bool isSealed)
        {
            Attack?.SetSealed(isSealed);
            Model?.SetSealed(isSealed);
        }

        private static string BuildHeroSynergies(ESynergy synergy)
        {
            if (synergy == ESynergy.None)
            {
                return "None";
            }

            var synergies = EnumUtility.GetValues<ESynergy>()
                .Where(type => type != ESynergy.None && EnumUtility.HasAnyFlag(synergy, type))
                .Select(FormatSynergyName)
                .ToList();

            return synergies.Count > 0 ? string.Join(", ", synergies) : "None";
        }

        private static string FormatSynergyName(ESynergy synergyType)
        {
            if (!SynergyDataTableUtility.TryGetSynergyDataTableRow(synergyType, out var synergyDataTableRow) ||
                synergyDataTableRow == null)
            {
                return synergyType.ToString();
            }

            var synergyColor = ColorUtility.ToHtmlStringRGB(synergyDataTableRow.color);
            return $"<color=#{synergyColor}>{synergyDataTableRow.displayName}</color>";
        }

        private static string FormatPercent(float value)
        {
            return $"{Mathf.RoundToInt(value * 100f)}%";
        }

        private void OnDisable()
        {
            Release();
        }
    }
}
