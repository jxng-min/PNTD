using System.Collections;
using System.Collections.Generic;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class ArtificerSkill : HeroSkill
    {
        private const string RobotHeroId = "Hero_MageRobo";

        private HeroSkillContext _context;
        private readonly List<Hero> _robots = new();

        public override void Initialize(HeroSkillContext context)
        {
            _context = context;
        }
        
        public override void Attach(Hero hero)
        {
            if (hero?.Dragger != null)
            {
                hero.Dragger.OnBeginDragRequested += HandleOnBeginDragRequested;
            }
        }

        public override IEnumerator Execute(Hero hero)
        {
            if (hero == null || _context?.BuildMap == null || _context.BoardSystem == null || _context.HeroFactory == null)
            {
                yield break;
            }

            if (!_context.BoardSystem.TryGetCell(hero, out var cellPosition))
            {
                yield break;
            }

            if (!TryFindRobotCell(hero, cellPosition, out var robotCell))
            {
                yield break;
            }

            var position = _context.BuildMap.GetCellCenterWorld(robotCell);
            var robot = _context.HeroFactory.CreateSummoned(RobotHeroId, hero.Level, hero, position, _context.Root);
            if (robot == null)
            {
                yield break;
            }

            if (!_context.BoardSystem.TryOccupy(robotCell, robot))
            {
                _context.HeroFactory.Release(robot);
                yield break;
            }

            _robots.Add(robot);
            robot.NotifyDeployed(robotCell);
        }

        private bool TryFindRobotCell(Hero hero, Vector3Int originCell, out Vector3Int robotCell)
        {
            var candidates = new List<Vector3Int>
            {
                originCell + Vector3Int.left,
                originCell + Vector3Int.right,
                originCell + Vector3Int.up,
                originCell + Vector3Int.down
            };
            
            candidates.RemoveAll(cell => !CanUseCell(cell));

            if (candidates.Count <= 0)
            {
                robotCell = default;
                return false;
            }

            robotCell = ChooseCloserCellToTarget(hero, candidates);
            return true;
        }

        private bool CanUseCell(Vector3Int cellPosition)
        {
            return _context.BuildMap.HasTile(cellPosition) &&
                   _context.BoardSystem.CanOccupy(cellPosition);
        }

        private Vector3Int ChooseCloserCellToTarget(Hero hero, IReadOnlyList<Vector3Int> cells)
        {
            var target = hero.Caster?.FindNearestTarget();
            if (target == null)
            {
                return cells[0];
            }

            var targetPosition = target.transform.position;
            var nearestCell = cells[0];
            var nearestDistance = float.MaxValue;

            foreach (var cell in cells)
            {
                var position = _context.BuildMap.GetCellCenterWorld(cell);
                var distance = (targetPosition - position).sqrMagnitude;
                if (distance >= nearestDistance)
                {
                    continue;
                }

                nearestCell = cell;
                nearestDistance = distance;
            }

            return nearestCell;
        }

        public override void Release(Hero hero)
        {
            if (hero?.Dragger != null)
            {
                hero.Dragger.OnBeginDragRequested -= HandleOnBeginDragRequested;
            }
            
            ReleaseRobots();
        }
        
        private void HandleOnBeginDragRequested(Hero hero, UnityEngine.EventSystems.PointerEventData eventData)
        {
            ReleaseRobots();
        }

        private void ReleaseRobots()
        {
            if (_robots.Count <= 0)
            {
                return;
            }

            foreach (var robot in _robots)
            {
                if (robot == null)
                {
                    continue;
                }

                _context?.BoardSystem?.ReleaseByHero(robot);
                _context?.HeroFactory?.Release(robot);
            }
            
            _robots.Clear();
        }
    }
}
