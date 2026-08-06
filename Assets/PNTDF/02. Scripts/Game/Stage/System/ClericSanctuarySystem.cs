using System;
using System.Collections.Generic;
using System.Linq;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class ClericSanctuarySystem
    {
        private const float ClericTwoRadiusMultiplier = 1.30f;
        private const float ClericFourAuraMultiplier = 1.25f;

        private readonly BoardSystem _boardSystem;
        private readonly Func<SynergyContext> _synergyContextProvider;
        private readonly List<ClericSanctuaryController> _sanctuaries = new();
        private readonly List<ClericAuraContext> _auraContexts = new();

        public ClericSanctuarySystem(BoardSystem boardSystem, Func<SynergyContext> synergyContextProvider)
        {
            _boardSystem = boardSystem;
            _synergyContextProvider = synergyContextProvider;
        }

        public void Register(Hero hero)
        {
            var sanctuary = hero != null ? hero.GetComponent<ClericSanctuaryController>() : null;
            if (sanctuary == null || _sanctuaries.Contains(sanctuary))
            {
                return;
            }

            _sanctuaries.Add(sanctuary);
        }

        public void Refresh()
        {
            Cleanup();

            var heroes = _boardSystem?.Heroes;
            if (heroes == null)
            {
                return;
            }

            var clericCount = _synergyContextProvider?.Invoke()?.GetCount(ESynergy.Cleric) ?? 0;
            var radiusMultiplier = clericCount >= 2 ? ClericTwoRadiusMultiplier : 1f;
            var isClericFourActive = clericCount >= 4;

            foreach (var sanctuary in _sanctuaries)
            {
                sanctuary?.SetRadiusMultiplier(radiusMultiplier);
            }

            ClericSanctuaryRegistry.Clear();

            foreach (var hero in heroes.ToArray())
            {
                RefreshHero(hero, isClericFourActive);
            }
        }

        public void Dispose()
        {
            if (_boardSystem?.Heroes != null)
            {
                foreach (var hero in _boardSystem.Heroes)
                {
                    hero?.Stat?.RemoveModifiersFrom(this);
                    if (hero?.Skill is StarbornSkill starbornSkill)
                    {
                        starbornSkill.SetClericOrbitSpeedMultiplier(1f);
                    }
                }
            }

            ClericSanctuaryRegistry.Clear();
            _sanctuaries.Clear();
            _auraContexts.Clear();
        }

        private void RefreshHero(Hero hero, bool isClericFourActive)
        {
            if (hero?.Stat == null)
            {
                return;
            }

            hero.Stat.RemoveModifiersFrom(this);
            if (hero.Skill is StarbornSkill starbornSkill)
            {
                starbornSkill.SetClericOrbitSpeedMultiplier(1f);
            }

            BuildAuraContexts(hero);
            ClericSanctuaryRegistry.SetAlliedAuraAffected(hero, _auraContexts.Count > 0);

            if (_auraContexts.Count == 0)
            {
                return;
            }

            var effectMultiplier = GetClericAuraEffectMultiplier(_auraContexts, isClericFourActive);
            var orbitSpeedMultiplier = 1f;

            foreach (var context in _auraContexts)
            {
                var value = context.Value * effectMultiplier;
                switch (context.AuraType)
                {
                    case EClericAuraType.AdaptivePenetration:
                        ApplyAdaptivePenetration(hero, value);
                        break;

                    case EClericAuraType.AttackPower:
                        hero.Stat.AddModifier(this, EHeroStat.PhysicalAttackPower, EHeroStatModifierOperation.Additive, value);
                        hero.Stat.AddModifier(this, EHeroStat.MagicAttackPower, EHeroStatModifierOperation.Additive, value);
                        break;

                    case EClericAuraType.AttackTempo:
                        if (hero.Skill is StarbornSkill)
                        {
                            orbitSpeedMultiplier *= 1f + value;
                        }
                        else
                        {
                            hero.Stat.AddModifier(this, EHeroStat.AttackCooldown, EHeroStatModifierOperation.Multiply, 1f - value);
                        }
                        break;
                }
            }

            if (hero.Skill is StarbornSkill targetStarbornSkill)
            {
                targetStarbornSkill.SetClericOrbitSpeedMultiplier(orbitSpeedMultiplier);
            }
        }

        private void BuildAuraContexts(Hero hero)
        {
            _auraContexts.Clear();

            foreach (var sanctuary in _sanctuaries)
            {
                if (sanctuary == null || !sanctuary.IsAllyAura || !sanctuary.Contains(hero))
                {
                    continue;
                }

                _auraContexts.Add(new ClericAuraContext(sanctuary.Data.auraType,
                                                        sanctuary.Owner,
                                                        sanctuary.CurrentValue));
            }
        }

        private static float GetClericAuraEffectMultiplier(IReadOnlyCollection<ClericAuraContext> auraContexts,
                                                           bool isClericFourActive)
        {
            if (!isClericFourActive)
            {
                return 1f;
            }

            var distinctAuraCount = auraContexts
                .Select(context => context.AuraType)
                .Distinct()
                .Count();

            return distinctAuraCount >= 2 ? ClericFourAuraMultiplier : 1f;
        }

        private void ApplyAdaptivePenetration(Hero hero, float value)
        {
            if (hero.Stat.BasePhysicalAttackPower > 0f)
            {
                hero.Stat.AddModifier(this, EHeroStat.PhysicalFlatPenetration, EHeroStatModifierOperation.Flat, value);
            }

            if (hero.Stat.BaseMagicAttackPower > 0f || EnumUtility.HasAnyFlag(hero.HeroDataTableRow.synergy, ESynergy.StarBorn))
            {
                hero.Stat.AddModifier(this, EHeroStat.MagicFlatPenetration, EHeroStatModifierOperation.Flat, value);
            }
        }

        private void Cleanup()
        {
            for (var index = _sanctuaries.Count - 1; index >= 0; index--)
            {
                if (_sanctuaries[index] == null || _sanctuaries[index].Owner == null)
                {
                    _sanctuaries.RemoveAt(index);
                }
            }
        }
    }
}
