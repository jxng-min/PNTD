using System.Collections;
using UnityEngine;

namespace PNTD
{
    public abstract class StarbornSkill : HeroSkill
    {
        private StarbornOrbitController _controller;
        private float _clericOrbitSpeedMultiplier = 1f;
        private float _hexOrbitSpeedMultiplier = 1f;

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

            _clericOrbitSpeedMultiplier = 1f;
            _hexOrbitSpeedMultiplier = 1f;
            _controller.Initialize(hero, AttackData);
        }

        public void SetSynergyOrbBonus(int bonusOrbCount)
        {
            _controller?.SetSynergyOrbBonus(bonusOrbCount);
        }

        public void SetClericOrbitSpeedMultiplier(float multiplier)
        {
            _clericOrbitSpeedMultiplier = Mathf.Max(0f, multiplier);
            RefreshOrbitSpeedMultiplier();
        }

        public void SetHexOrbitSpeedMultiplier(float multiplier)
        {
            _hexOrbitSpeedMultiplier = Mathf.Max(0f, multiplier);
            RefreshOrbitSpeedMultiplier();
        }

        public override void Release(Hero hero)
        {
            _controller?.Release();
            _controller = null;
            _clericOrbitSpeedMultiplier = 1f;
            _hexOrbitSpeedMultiplier = 1f;
        }

        public override IEnumerator Execute(Hero hero)
        {
            yield break;
        }

        private void RefreshOrbitSpeedMultiplier()
        {
            _controller?.SetOrbitSpeedMultiplier(_clericOrbitSpeedMultiplier * _hexOrbitSpeedMultiplier);
        }
    }
}
