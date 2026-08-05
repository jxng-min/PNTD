using System.Collections;

namespace PNTD
{
    public class EmptyHeroSkill : HeroSkill
    {
        public override IEnumerator Execute(Hero hero)
        {
            yield break;
        }
    }
}
