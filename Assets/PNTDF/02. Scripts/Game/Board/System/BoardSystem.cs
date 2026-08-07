using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PNTD
{
    public class BoardSystem
    {
        private readonly Dictionary<Vector3Int, Hero> _heroByCell = new();
        private readonly Dictionary<Hero, Vector3Int> _cellByHero = new();

        public IReadOnlyCollection<Hero> Heroes => _cellByHero.Keys.ToArray();

        public void Initialize()
        {
            _heroByCell.Clear();
            _cellByHero.Clear();
        }

        public bool IsOccupied(Vector3Int cellPosition)
        {
            return _heroByCell.ContainsKey(cellPosition);
        }

        public bool TryGetHero(Vector3Int cellPosition, out Hero hero)
        {
            return _heroByCell.TryGetValue(cellPosition, out hero);
        }

        public bool TryGetCell(Hero hero, out Vector3Int cellPosition)
        {
            return _cellByHero.TryGetValue(hero, out cellPosition);
        }

        public bool TryOccupy(Vector3Int cellPosition, Hero hero)
        {
            if (hero == null || IsOccupied(cellPosition))
            {
                return false;
            }
            
            _heroByCell[cellPosition] = hero;
            _cellByHero[hero] = cellPosition;
            return true;
        }

        public bool CanOccupy(Vector3Int cellPosition)
        {
            return !IsOccupied(cellPosition);
        }

        public bool ReleaseByCell(Vector3Int cellPosition)
        {
            if (!TryGetHero(cellPosition, out var hero))
            {
                return false;
            }

            _heroByCell.Remove(cellPosition);
            _cellByHero.Remove(hero);
            return true;
        }

        public bool ReleaseByHero(Hero hero)
        {
            if (!TryGetCell(hero, out var cellPosition))
            {
                return false;
            }

            _heroByCell.Remove(cellPosition);
            _cellByHero.Remove(hero);
            return true;
        }
    }
}
