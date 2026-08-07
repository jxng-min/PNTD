using System.Collections;
using UnityEngine;

namespace PNTD
{
    public abstract class ClericSkill : HeroSkill
    {
        private ClericSanctuaryController _controller;

        public override bool IsContinuous => true;

        protected abstract ClericSanctuaryData SanctuaryData { get; }

        public override void Attach(Hero hero)
        {
            if (hero == null)
            {
                return;
            }

            _controller = hero.GetComponent<ClericSanctuaryController>();
            if (_controller == null)
            {
                _controller = hero.gameObject.AddComponent<ClericSanctuaryController>();
            }

            _controller.Initialize(hero, SanctuaryData);
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
