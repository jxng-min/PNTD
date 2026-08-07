using UnityEngine;

namespace PNTD
{
    public class SummonerAbility : EnemyAbility
    {
        private readonly SummonerData _data;
        private EnemyAbilityContext _context;
        private float _coolDownRemaining;
        private int _castCount;

        public SummonerAbility(SummonerData data)
        {
            _data = data;
        }

        public override void Initialize(EnemyAbilityContext abilityContext)
        {
            Release();

            _context = abilityContext;
            _coolDownRemaining = _data != null ? _data.FirstCastDelay : 0f;
            _castCount = 0;
        }

        public override void Tick(float deltaTime, bool isCooldownPaused)
        {
            if (_context?.Owner == null || _data == null || isCooldownPaused)
            {
                return;
            }

            if (_data.MaxCasts > 0 && _castCount >= _data.MaxCasts)
            {
                return;
            }

            _coolDownRemaining -= deltaTime;
            if (_coolDownRemaining > 0f)
            {
                return;
            }

            Summon();
            _castCount++;
            _coolDownRemaining = _data.CoolDown;
        }

        public override void Release()
        {
            _context = null;
            _coolDownRemaining = 0f;
            _castCount = 0;
        }

        private void Summon()
        {
            var owner = _context.Owner;
            if (owner == null || owner.Movement == null || string.IsNullOrEmpty(_data.SpawnedEnemyID))
            {
                return;
            }

            var spawnCount = Mathf.Max(0, _data.SpawnCount);
            for (var i = 0; i < spawnCount; i++)
            {
                var position = GetSpawnPosition(owner.transform.position);
                _context.EnemySpawner?.SpawnEnemy(_data.SpawnedEnemyID,
                                                  position,
                                                  owner.Movement.CurrentPointIndex);
            }
        }

        private Vector3 GetSpawnPosition(Vector3 center)
        {
            var direction = Random.insideUnitCircle;
            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                direction = Vector2.left;
            }

            direction.Normalize();

            var minOffset = Mathf.Max(0f, _data.MinSpawnOffset);
            var maxOffset = Mathf.Max(minOffset, _data.MaxSpawnOffset);
            var distance = Random.Range(minOffset, maxOffset);
            var offset = direction * distance;

            return new Vector3(center.x + offset.x, center.y + offset.y, center.z);
        }
    }
}
