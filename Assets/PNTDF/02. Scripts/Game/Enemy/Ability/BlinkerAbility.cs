namespace PNTD
{
    public class BlinkerAbility : EnemyAbility
    {
        private readonly BlinkerData _data;
        private EnemyAbilityContext _context;
        private float _coolDownRemaining;

        public BlinkerAbility(BlinkerData data)
        {
            _data = data;
        }

        public override void Initialize(EnemyAbilityContext abilityContext)
        {
            Release();

            _context = abilityContext;
            _coolDownRemaining = _data != null ? _data.FirstCastDelay : 0f;
        }

        public override void Tick(float deltaTime, bool isCooldownPaused)
        {
            if (_context?.Owner == null || _data == null || isCooldownPaused)
            {
                return;
            }

            _coolDownRemaining -= deltaTime;
            if (_coolDownRemaining > 0f)
            {
                return;
            }

            Blink();
            _coolDownRemaining = _data.CoolDown;
        }

        public override void Release()
        {
            _context = null;
            _coolDownRemaining = 0f;
        }

        private void Blink()
        {
            var owner = _context.Owner;
            if (owner.Movement == null || owner.Movement.IsReached)
            {
                return;
            }

            owner.Status?.AddInvincibleEffect(_data.InvincibleDuration);
            owner.Movement.MoveForwardOnPath(_data.BlinkDistance);
        }
    }
}
