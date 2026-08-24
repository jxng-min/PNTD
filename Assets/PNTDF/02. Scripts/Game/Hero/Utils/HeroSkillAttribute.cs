using System;

namespace PNTD
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HeroSkillAttribute : Attribute
    {
        public EHeroType HeroType { get; }

        public HeroSkillAttribute(EHeroType heroType)
        {
            HeroType = heroType;
        }
    }
}