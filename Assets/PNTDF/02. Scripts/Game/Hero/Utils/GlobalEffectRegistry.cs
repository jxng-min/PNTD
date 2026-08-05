using System.Collections.Generic;

namespace PNTD
{
    public static class GlobalEffectRegistry
    {
        private static readonly List<GlobalEffect> Effects = new();

        public static void Register(GlobalEffect effect)
        {
            if (effect == null || Effects.Contains(effect))
            {
                return;
            }
            
            Effects.Add(effect);
        }

        public static void Unregister(GlobalEffect effect)
        {
            if (effect == null)
            {
                return;
            }

            Effects.Remove(effect);
        }

        public static void OnStageBegin(REffectContext context)
        {
            foreach (var effect in Effects)
            {
                effect.OnStageBegin(context);
            }
        }

        public static void OnEnemyKilled(Hero hero, Enemy enemy)
        {
            foreach (var effect in Effects)
            {
                effect.OnEnemyKilled(hero, enemy);
            }
        }
    }
}