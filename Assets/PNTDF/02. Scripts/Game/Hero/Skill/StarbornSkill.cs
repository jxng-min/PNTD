using System.Collections;
using UnityEngine;

namespace PNTD
{
    public abstract class StarbornSkill : HeroSkill
    {
        private StarbornOrbitController _controller;

        public override bool IsContinuous => true;

        protected abstract StarbornAttackData AttackData { get; }

        public override void Attach(Hero hero)
        {
            if (hero == null)
            {
                return;
            }

            _controller = hero.gameObject.GetComponent<StarbornOrbitController>();
            if (_controller == null)
            {
                _controller = hero.gameObject.AddComponent<StarbornOrbitController>();
            }

            _controller.Initialize(hero, AttackData);
        }

        public void SetSynergyOrbBonus(int bonusOrbCount)
        {
            _controller?.SetSynergyOrbBonus(bonusOrbCount);
        }

        public override void Release(Hero hero)
        {
            _controller?.Release();
            _controller = null;
        }

        public override IEnumerator Execute(Hero hero)
        {
            yield break;
        }
    }
}
