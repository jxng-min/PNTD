using System;
using System.Collections.Generic;
using System.Linq;
using JxModule;

namespace PNTD
{
    public static class SynergyCalculator
    {
        public static Dictionary<ESynergy, int> Calculate(IEnumerable<HeroContext> heroContexts)
        {
            var result = new Dictionary<ESynergy, int>();

            foreach (ESynergy eSynergy in Enum.GetValues(typeof(ESynergy)))
            {
                if (eSynergy == ESynergy.None)
                {
                    continue;
                }
                
                var count = heroContexts.Count(hero => EnumUtility.HasAnyFlag(hero.HeroDataTableRow.synergy, eSynergy));
                result[eSynergy] = count;
            }

            return result;
        }
    }
}