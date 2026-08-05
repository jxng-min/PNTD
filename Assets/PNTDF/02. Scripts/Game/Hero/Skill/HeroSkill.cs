using System.Collections;
using UnityEngine;

namespace PNTD
{
    public abstract class HeroSkill
    {
        public virtual void Attach(Hero hero) {}
        public virtual void Initialize(HeroSkillContext context) {}
        public virtual void Release(Hero hero) {}
        public virtual void OnDeployed(Hero hero, Vector3Int cellPosition) {}
        public virtual bool IsContinuous => false;

        public abstract IEnumerator Execute(Hero hero);
    }
}
