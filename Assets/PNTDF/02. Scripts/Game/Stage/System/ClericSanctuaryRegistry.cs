using System.Collections.Generic;

namespace PNTD
{
    public static class ClericSanctuaryRegistry
    {
        private static readonly HashSet<Hero> AlliedAuraAffectedHeroes = new();

        public static bool HasAlliedAura(Hero hero)
        {
            return hero != null && AlliedAuraAffectedHeroes.Contains(hero);
        }

        public static void SetAlliedAuraAffected(Hero hero, bool isAffected)
        {
            if (hero == null)
            {
                return;
            }

            if (isAffected)
            {
                AlliedAuraAffectedHeroes.Add(hero);
            }
            else
            {
                AlliedAuraAffectedHeroes.Remove(hero);
            }
        }

        public static void Clear()
        {
            AlliedAuraAffectedHeroes.Clear();
        }
    }
}
