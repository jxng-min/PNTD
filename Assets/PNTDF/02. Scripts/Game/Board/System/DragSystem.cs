using System;
using UnityEngine;

namespace PNTD
{
    public class DragSystem
    {
        public event Action<Hero, Vector3> OnBeginDrag;
        public event Action OnEndDrag;
        
        public Hero DraggingHero { get; private set; }
        public Vector3Int OriginCellPosition { get; private set; }
        public bool IsDragging => DraggingHero != null;

        public bool HandleOnBeginDrag(Hero hero, Vector3Int originCellPosition)
        {
            if (hero == null || IsDragging)
            {
                return false;
            }
            
            DraggingHero = hero;
            OriginCellPosition = originCellPosition;
            
            OnBeginDrag?.Invoke(hero, originCellPosition);
            return true;
        }

        public void HandleOnEndDrag()
        {
            if (!IsDragging)
            {
                return;
            }

            DraggingHero = null;
            OriginCellPosition = default;
            
            OnEndDrag?.Invoke();
        }
    }
}