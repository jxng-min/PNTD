using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PNTD
{
    public class HexerAbility : EnemyAbility
    {
        private readonly HexerData _data;
        private readonly List<EnemyHexEffect> _activeHexEffects = new();
        private EnemyAbilityContext _context;
        private float _coolDownRemaining;

        public HexerAbility(HexerData data)
        {
            _data = data;
        }

        public override void Initialize(EnemyAbilityContext abilityContext)
        {
            Release();

            _context = abilityContext;
            _coolDownRemaining = _data != null ? _data.FirstCastDelay : 0f;
        }

        public override void Tick(float deltaTime, bool isCooldownPaused)
        {
            if (_context?.Owner == null || _data == null || isCooldownPaused)
            {
                return;
            }

            _coolDownRemaining -= deltaTime;
            if (_coolDownRemaining > 0f)
            {
                return;
            }

            TryCastHex();
            _coolDownRemaining = _data.CoolDown;
        }

        public override void Release()
        {
            _activeHexEffects.Clear();
            _context = null;
            _coolDownRemaining = 0f;
        }

        private void TryCastHex()
        {
            var target = FindTarget();
            if (target == null)
            {
                return;
            }

            PruneActiveEffects();
            var maxActiveHex = Mathf.Max(1, _data.MaxActiveHexPerHexer);
            if (_activeHexEffects.Count >= maxActiveHex)
            {
                _activeHexEffects[0].Refresh();
                return;
            }

            var existingHex = target.Effector?.Effects
                .OfType<EnemyHexEffect>()
                .FirstOrDefault(effect => effect != null && !effect.IsFinished);

            if (existingHex != null)
            {
                existingHex.Refresh();
                return;
            }

            var newHex = new EnemyHexEffect(_context.Owner,
                                            _data.Duration,
                                            _data.AttackDamagePenalty,
                                            _data.AttackSpeedPenalty);
            if (target.TryAddEffect(newHex))
            {
                _activeHexEffects.Add(newHex);
            }
        }

        private void PruneActiveEffects()
        {
            for (var i = _activeHexEffects.Count - 1; i >= 0; i--)
            {
                if (_activeHexEffects[i] == null || _activeHexEffects[i].IsFinished)
                {
                    _activeHexEffects.RemoveAt(i);
                }
            }
        }

        private Hero FindTarget()
        {
            var owner = _context.Owner;
            var heroes = _context.HeroProvider?.Heroes;
            if (owner == null || heroes == null)
            {
                return null;
            }

            var castRangeSqr = _data.CastRange * _data.CastRange;
            Hero bestTarget = null;
            var bestScore = float.MinValue;

            foreach (var hero in heroes)
            {
                if (hero == null || hero.Stat == null)
                {
                    continue;
                }

                var distanceSqr = (hero.transform.position - owner.transform.position).sqrMagnitude;
                if (distanceSqr > castRangeSqr)
                {
                    continue;
                }

                var cooldown = Mathf.Max(0.1f, hero.Stat.FinalAttackCooldown);
                var score = (hero.Stat.FinalPhysicalAttackPower + hero.Stat.FinalMagicAttackPower) / cooldown;
                if (score <= bestScore)
                {
                    continue;
                }

                bestScore = score;
                bestTarget = hero;
            }

            return bestTarget;
        }
    }
}
