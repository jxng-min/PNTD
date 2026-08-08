using System;
using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class WaveSystem : IEnemyProvider, IEnemySpawner
    {
#region TurnContext
        private sealed class TurnStateContext
        {
            public string EnemyID;
            public int RemainingSpawnCount;
            public float SpawnInterval;
            public float TimeUntilNextSpawn;
            public bool Completed;
        }
#endregion

        private readonly HashSet<Enemy> _aliveEnemies = new();
        private readonly List<TurnStateContext> _turnContexts = new();

        private EnemyFactory _enemyFactory;

        private bool _isWaveRunning;
        private bool _isCompleted;

        public event Action<Enemy> OnEnemyDied;
        public event Action<Enemy> OnDestinationReached;
        public event Action OnWaveEnd;
        
        public IReadOnlyCollection<Enemy> AliveEnemies => _aliveEnemies;

        public void Initialize(EnemyFactory enemyFactory)
        {
            if (enemyFactory == null)
            {
                DebugExtension.LogColor($"Wave System: Enemy Factory is null.", Color.red);
                return;
            }

            _enemyFactory = enemyFactory;
            StopWave();
        }
        
        public void Dispose()
        {
            StopWave();
            _enemyFactory = null;
        }

        public void Tick(float deltaTime)
        {
            if (!_isWaveRunning || _isCompleted)
            {
                return;
            }

            foreach (var turnStateContext in _turnContexts)
            {
                if (turnStateContext.Completed)
                {
                    continue;
                }
                
                turnStateContext.TimeUntilNextSpawn -= deltaTime;
                while (!turnStateContext.Completed && turnStateContext.TimeUntilNextSpawn <= 0f)
                {
                    SpawnEnemy(turnStateContext.EnemyID);
                    turnStateContext.RemainingSpawnCount--;

                    if (turnStateContext.RemainingSpawnCount <= 0)
                    {
                        turnStateContext.Completed = true;
                        break;
                    }

                    if (turnStateContext.SpawnInterval <= 0f)
                    {
                        turnStateContext.TimeUntilNextSpawn = 0f;
                        continue;
                    }
                    
                    turnStateContext.TimeUntilNextSpawn += turnStateContext.SpawnInterval;
                }
            }
            
            TryCompleteWave();
        }

        public void StartWave(int waveIndex, WaveContext waveContext)
        {
            if (_enemyFactory == null)
            {
                DebugExtension.LogColor($"Wave System: StartWave was called before initialization.", Color.red);
                return;
            }

            StopWave();

            if (waveContext?.Turns == null || waveContext.Turns.Count == 0)
            {
                _isWaveRunning = true;
                TryCompleteWave();
                return;
            }

            foreach (var turnContext in waveContext.Turns)
            {
                _turnContexts.Add(new TurnStateContext
                {
                    EnemyID = turnContext.EnemyId,
                    RemainingSpawnCount = turnContext.SpawnCount,
                    SpawnInterval = turnContext.SpawnInterval,
                    TimeUntilNextSpawn = turnContext.StartTime,
                    Completed = false
                });
            }
            
            _isWaveRunning = true;
            TryCompleteWave();
        }

        public void StopWave()
        {
            ClearAliveEnemies();
            _turnContexts.Clear();
            _isWaveRunning = false;
            _isCompleted = false;
        }

        public void RegisterEnemy(Enemy enemy)
        {
            if (enemy == null || !_aliveEnemies.Add(enemy))
            {
                return;
            }

            enemy.OnEnemyDied += HandleOnEnemyDied;
            enemy.OnDestinationReached += HandleOnDestinationReached;
        }

        private void SpawnEnemy(string enemyId)
        {
            if (_enemyFactory == null)
            {
                DebugExtension.LogColor($"Wave System: Enemy Factory is null.", Color.red);
                return;
            }

            var enemy = _enemyFactory.Create(enemyId);
            RegisterEnemy(enemy);
        }

        public Enemy SpawnEnemy(string enemyId, Vector3 position, int pathPointIndex)
        {
            if (_enemyFactory == null)
            {
                DebugExtension.LogColor($"Wave System: Enemy Factory is null.", Color.red);
                return null;
            }

            var enemy = _enemyFactory.Create(enemyId, position, pathPointIndex);
            RegisterEnemy(enemy);
            TryCompleteWave();
            return enemy;
        }

        private void ReleaseEnemy(Enemy enemy)
        {
            if (enemy == null)
            {
                return;
            }

            _aliveEnemies.Remove(enemy);
            enemy.OnEnemyDied -= HandleOnEnemyDied;
            enemy.OnDestinationReached -= HandleOnDestinationReached;
        }

        private void ClearAliveEnemies()
        {
            var enemies = new List<Enemy>(_aliveEnemies);
            foreach (var enemy in enemies)
            {
                ReleaseEnemy(enemy);
            }
        }

        private void HandleOnEnemyDied(Enemy enemy)
        {
            ReleaseEnemy(enemy);
            OnEnemyDied?.Invoke(enemy);
            TryCompleteWave();
        }

        private void HandleOnDestinationReached(Enemy enemy)
        {
            ReleaseEnemy(enemy);
            OnDestinationReached?.Invoke(enemy);
            TryCompleteWave();
        }

        private void TryCompleteWave()
        {
            if (_isCompleted || !_isWaveRunning)
            {
                return;
            }

            if (_turnContexts.Exists(turn => !turn.Completed) || _aliveEnemies.Count > 0)
            {
                return;
            }

            _isCompleted = true;
            OnWaveEnd?.Invoke();
        }
    }
}
