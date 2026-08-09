using System;
using System.Collections.Generic;
using System.Linq;
using JxModule;
using TMPro;
using UnityEngine;

namespace PNTD
{
    public class ShopPresenter : MonoBehaviour
    {
        [BigHeader("UI")]
        [Header("Top Group")]
        [SerializeField] private TMP_Text goldLabel;
        [SerializeField] private JxButton rerollButton;
        [SerializeField] private JxToggle lockToggle;
        [SerializeField] private ShopLevelView shopLevelView;
        
        [Header("Slot Group")]
        [SerializeField] private ShopSlotView[] shopSlotViews;

        private readonly List<ShopSlotContext> _slotData = new();
        private bool[] _soldOutSlots;
        
        public event Action<HeroDataTableRow, int> OnClickedSlot;
        public event Action OnClickedReroll;
        public event Action<bool> OnChangedLock;
        public event Action OnRequestLevelUp;

        private void Awake()
        {
            _soldOutSlots = new bool[shopSlotViews.Length];
            
            foreach (var shopSlotView in shopSlotViews)
            {
                if (shopSlotView == null)
                {
                    continue;
                }

                shopSlotView.OnClickedSlot += HandleOnClickedSlot;
            }

            shopLevelView.OnRequestLevelUp += HandleOnRequestLevelUp;
            rerollButton.AddListener(HandleOnClickedReroll);
            lockToggle.OnValueChanged += HandleOnChangedLock;
        }

        public void Initialize(ShopSystem shopSystem)
        {
            shopLevelView.Initialize(shopSystem.Level, shopSystem.Exp);
            HandleOnUpdateLock(shopSystem.IsLock);
        }
        
        public void SetSoldOut(int slotIndex)
        {
            if (_soldOutSlots == null || slotIndex < 0 ||  slotIndex >= _soldOutSlots.Length)
            {
                return;
            }
            
            _soldOutSlots[slotIndex] = true;
            shopSlotViews[slotIndex].CanvasGroup.Hide();
        }

        public void RefreshSlotsSynergies(SynergyContext synergyContext, IReadOnlyList<HeroContext> heroContexts)
        {
            var currentSynergyContext = synergyContext ?? SynergyContext.Empty;
            var count = Mathf.Min(_slotData.Count, shopSlotViews.Length);
            
            for (var index = 0; index < shopSlotViews.Length; index++)
            {
                var shopSlotView = shopSlotViews[index];
                if (shopSlotView == null)
                {
                    continue;
                }
                
                var isActive = index < count && !IsSoldOut(index);
                if (isActive)
                {
                    shopSlotView.CanvasGroup.Show();
                }
                else
                {
                    shopSlotView.CanvasGroup.Hide();
                }

                if (!isActive)
                {
                    continue;
                }
                
                var shopSlotContext = _slotData[index];
                var alreadyOwned = HasOwnedSameHero(heroContexts, shopSlotContext.HeroDataTableRow);
                var canIncreaseSynergy = !alreadyOwned && shopSlotContext.SynergyDataTableRows is { Count: > 0 };
                
                shopSlotView.Initialize(new ShopSlotContext(shopSlotContext.HeroDataTableRow,
                                                            shopSlotContext.SynergyDataTableRows, 
                                                            canIncreaseSynergy),
                                        currentSynergyContext,
                                        index);
            }
        }

        public void HandleOnUpdateGold(int gold)
        {
            goldLabel.text = $"<color=#FFD000>${gold}</color>";
        }

        public void HandleOnUpdateLevel(int level, int exp)
        {
            shopLevelView.UpdateLevel(level, exp);
        }

        public void HandleOnUpdateLock(bool isLock)
        {
            lockToggle?.SetIsOn(isLock, false);
        }

        public void HandleOnRerollShop(IReadOnlyList<ShopSlotContext> contexts)
        {
            _slotData.Clear();
            RefreshSoldOutSlots();

            if (contexts == null)
            {
                HideAllSlots();
                return;
            }
            
            var loopCount = Mathf.Min(contexts.Count, shopSlotViews.Length);
            for (var index = 0; index < loopCount; index++)
            {
                _slotData.Add(contexts[index]);
            }
        }

        private void HandleOnClickedSlot(HeroDataTableRow heroDataTableRow, int slotIndex)
        {
            OnClickedSlot?.Invoke(heroDataTableRow, slotIndex);
        }

        private void HandleOnRequestLevelUp()
        {
            OnRequestLevelUp?.Invoke();
        }

        private void HandleOnClickedReroll()
        {
            OnClickedReroll?.Invoke();
        }

        private void HandleOnChangedLock(bool isOn)
        {
            OnChangedLock?.Invoke(isOn);
        }

        private void RefreshSoldOutSlots()
        {
            if (_soldOutSlots == null || _soldOutSlots.Length != shopSlotViews.Length)
            {
                _soldOutSlots = new bool[shopSlotViews.Length];
                return;
            }

            for (var index = 0; index < _soldOutSlots.Length; index++)
            {
                _soldOutSlots[index] = false;
            }
        }

        private bool IsSoldOut(int slotIndex)
        {
            return _soldOutSlots != null && 
                   slotIndex >= 0 &&  slotIndex < _soldOutSlots.Length &&
                   _soldOutSlots[slotIndex];
        }

        private void HideAllSlots()
        {
            foreach (var shopSlotView in shopSlotViews)
            {
                if (shopSlotView == null)
                {
                    continue;
                }

                shopSlotView.CanvasGroup.Hide();
            }
        }

        private static bool HasOwnedSameHero(IReadOnlyList<HeroContext> heroContexts, HeroDataTableRow heroDataTableRow)
        {
            if (heroContexts == null || heroDataTableRow == null)
            {
                return false;
            }

            return heroContexts
                .Where(heroContext => heroContext != null && heroContext.HeroDataTableRow != null)
                .Any(heroContext => heroContext.HeroDataTableRow.rowID == heroDataTableRow.rowID);
        }

        private void OnDestroy()
        {
            foreach (var shopSlotView in shopSlotViews)
            {
                if (shopSlotView == null)
                {
                    continue;
                }
                
                shopSlotView.OnClickedSlot -= HandleOnClickedSlot;
            }
        }
    }
}

