using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PNTD
{
    public class HeroDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Hero _hero;
        
        public event Action<Hero, PointerEventData> OnBeginDragRequested;
        public event Action<Hero, PointerEventData> OnDragRequested;
        public event Action<Hero, PointerEventData> OnEndDragRequested;

        public void Initialize(Hero hero)
        {
            _hero = hero;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragRequested?.Invoke(_hero, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragRequested?.Invoke(_hero, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragRequested?.Invoke(_hero, eventData);
        }
    }
}