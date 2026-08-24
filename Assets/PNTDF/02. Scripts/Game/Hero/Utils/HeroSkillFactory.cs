using System;
using System.Collections.Generic;
using System.Reflection;

namespace PNTD
{
    public static class HeroSkillFactory
    {
        private static readonly Dictionary<EHeroType, Type> SkillTypes = new();

        static HeroSkillFactory()
        {
            var skillBaseType = typeof(HeroSkill);

            foreach (var type in skillBaseType.Assembly.GetTypes())
            {
                if (type.IsAbstract || !skillBaseType.IsAssignableFrom(type))
                {
                    continue;
                }
                
                var attribute = type.GetCustomAttribute<HeroSkillAttribute>();
                if (attribute == null)
                {
                    continue;
                }
                
                SkillTypes.TryAdd(attribute.HeroType, type);
            }
        }

        public static HeroSkill Create(EHeroType heroType)
        {
            if (!SkillTypes.TryGetValue(heroType, out var skillType))
            {
                return new EmptyHeroSkill();
            }

            return (HeroSkill)Activator.CreateInstance(skillType);
        }
    }
}