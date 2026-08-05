using System;
using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class HeroStat
    {
        private readonly List<HeroStatModifier> _modifiers = new();

        public event Action OnChangedStat;
        
        public float BasePhysicalAttackPower { get; private set; }
        public float BaseMagicAttackPower { get; private set; }
        public float BaseAttackCooldown { get; private set; }
        public float BaseAttackRange { get; private set; }
        public float BaseFlatPhysicalPenetration { get; private set; }
        public float BasePercentPhysicalPenetration { get; private set; }
        public float BaseFlatMagicPenetration { get; private set; }
        public float BasePercentMagicPenetration { get; private set; }
        
        public float FinalPhysicalAttackPower { get; private set; }
        public float FinalMagicAttackPower { get; private set; }
        public float FinalAttackCooldown { get; private set; }
        public float FinalAttackRange { get; private set; }
        public float FinalFlatPhysicalPenetration { get; private set; }
        public float FinalPercentPhysicalPenetration { get; private set; }
        public float FinalFlatMagicPenetration { get; private set; }
        public float FinalPercentMagicPenetration { get; private set; }
        
        public IReadOnlyList<HeroStatModifier> Modifiers => _modifiers;

        public HeroStat(HeroAttackDataTableRow heroAttackDataTableRow)
        {
            if (heroAttackDataTableRow == null)
            {
                DebugExtension.LogColor($"Hero Stat: HeroAttackDataTableRow is null.", Color.red);

                BasePhysicalAttackPower = 0f;
                BaseMagicAttackPower = 0f;
                BaseAttackCooldown = 0f;
                BaseAttackRange = 0f;
                BaseFlatPhysicalPenetration = 0f;
                BasePercentPhysicalPenetration = 0f;
                BaseFlatMagicPenetration = 0f;
                BasePercentMagicPenetration = 0f;
                
                return;
            }

            BasePhysicalAttackPower = heroAttackDataTableRow.physicalAttackPower;
            BaseMagicAttackPower = heroAttackDataTableRow.magicAttackPower;
            BaseAttackCooldown = heroAttackDataTableRow.attackCooldown;
            BaseAttackRange = heroAttackDataTableRow.attackRange;
            BaseFlatPhysicalPenetration = heroAttackDataTableRow.physicalFlatPenetration;
            BasePercentPhysicalPenetration = heroAttackDataTableRow.physicalPercentPenetration;
            BaseFlatMagicPenetration = heroAttackDataTableRow.magicFlatPenetration;
            BasePercentMagicPenetration = heroAttackDataTableRow.magicPercentPenetration;

            RecalculateStats(false);
        }

        public HeroStatModifier AddModifier(object source, 
                                            EHeroStat stat, 
                                            EHeroStatModifierOperation operation, 
                                            float value)
        {
            var modifier = new HeroStatModifier(source, stat, operation, value);
            AddModifier(modifier);
            return modifier;
        }

        public void AddModifier(HeroStatModifier modifier)
        {
            if (modifier == null)
            {
                return;
            }
            
            _modifiers.Add(modifier);
            RecalculateStats();
        }

        public bool RemoveModifier(HeroStatModifier modifier)
        {
            if (modifier == null)
            {
                return false;
            }

            if (!_modifiers.Remove(modifier))
            {
                return false;
            }

            RecalculateStats();
            return true;
        }

        public int RemoveModifiersFrom(object source)
        {
            if (source == null)
            {
                return 0;
            }

            var removedCount = _modifiers.RemoveAll(modifier => modifier != null && modifier.IsSource(source));
            if (removedCount > 0)
            {
                RecalculateStats();
            }
            
            return removedCount;
        }

        public void ClearModifiers()
        {
            if (_modifiers.Count == 0)
            {
                return;
            }
            
            _modifiers.Clear();
            RecalculateStats();
        }

        public bool HasModifierFrom(object source)
        {
            if (source == null)
            {
                return false;
            }

            foreach (var modifier in _modifiers)
            {
                if (modifier != null && modifier.IsSource(source))
                {
                    return true;
                }
            }

            return false;
        }

        public DamageContext CreateDamageContext(EAttack attack, float damageMultiplier)
        {
            return attack switch
            {
                EAttack.Physical => new DamageContext(FinalPhysicalAttackPower * damageMultiplier,
                                                      EAttack.Physical,
                                                      FinalFlatPhysicalPenetration,
                                                      FinalPercentPhysicalPenetration),

                EAttack.Magic => new DamageContext(FinalMagicAttackPower * damageMultiplier,
                                                   EAttack.Magic,
                                                   FinalFlatMagicPenetration,
                                                   FinalPercentMagicPenetration),

                EAttack.True => new DamageContext(Mathf.Max(FinalPhysicalAttackPower, FinalMagicAttackPower) * damageMultiplier, EAttack.True),

                _ => default
            };
        }

        private void RecalculateStats(bool notify = true)
        {
            FinalPhysicalAttackPower = Mathf.Max(0f, CalculateStat(EHeroStat.PhysicalAttackPower, BasePhysicalAttackPower));
            FinalMagicAttackPower = Mathf.Max(0f, CalculateStat(EHeroStat.MagicAttackPower, BaseMagicAttackPower));
            FinalAttackCooldown = Mathf.Max(0f, CalculateStat(EHeroStat.AttackCooldown, BaseAttackCooldown));
            FinalAttackRange = Mathf.Max(0f, CalculateStat(EHeroStat.AttackRange, BaseAttackRange));
            FinalFlatPhysicalPenetration = Mathf.Max(0f, CalculateStat(EHeroStat.PhysicalFlatPenetration, BaseFlatPhysicalPenetration));
            FinalPercentPhysicalPenetration = Mathf.Clamp01(CalculateStat(EHeroStat.PhysicalPercentPenetration, BasePercentPhysicalPenetration));
            FinalFlatMagicPenetration = Mathf.Max(0f, CalculateStat(EHeroStat.MagicFlatPenetration, BaseFlatMagicPenetration));
            FinalPercentMagicPenetration = Mathf.Clamp01(CalculateStat(EHeroStat.MagicPercentPenetration, BasePercentMagicPenetration));

            if (notify)
            {
                OnChangedStat?.Invoke();
            }
        }

        private float CalculateStat(EHeroStat stat, float baseValue)
        {
            var flatValue = 0f;
            var additiveValue = 0f;
            var multiplyValue = 1f;

            foreach (var modifier in _modifiers)
            {
                if (modifier == null || modifier.Stat != stat)
                {
                    continue;
                }

                switch (modifier.Operation)
                {
                    case EHeroStatModifierOperation.Flat:
                        flatValue += modifier.Value;
                        break;
                    
                    case EHeroStatModifierOperation.Additive:
                        additiveValue += modifier.Value;
                        break;
                    
                    case EHeroStatModifierOperation.Multiply:
                        multiplyValue *= modifier.Value;
                        break;
                    
                    default:
                        break;
                }
            }

            return (baseValue + flatValue) * (1f + additiveValue) * multiplyValue;
        }
    }
}