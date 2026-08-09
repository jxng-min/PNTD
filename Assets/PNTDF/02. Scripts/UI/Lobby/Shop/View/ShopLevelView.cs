using System;
using System.Collections.Generic;
using JxModule;
using JxModule.DataTable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    public class ShopLevelView : ViewBase, ITooltipProvider
    {
        private const string TooltipId = "ShopLevel";
        private const int MaxLevel = 5;

        [BigHeader("UI")]
        [SerializeField] private JxButton levelUpButton;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private Image[] expImages;
        
        [Space(30f)]
        [BigHeader("Settings")]
        [SerializeField] private Color enabledColor;
        [SerializeField] private Color disabledColor;

        public event Action OnRequestLevelUp;
        public bool CanShowTooltip => GetShopRateDataTableRow() != null;

        private TooltipUI _tooltipUI;
        private int _level;

        private void Awake()
        {
            _tooltipUI = GetComponent<TooltipUI>();
            levelUpButton.AddListener(RequestLevelUp);   
        }

        public void Initialize(int level, int exp)
        {
            UpdateLevel(level, exp);
        }

        public void UpdateLevel(int level, int exp)
        {
            _level = level;
            levelLabel.text = level.ToString();
            UpdateExp(exp);
            _tooltipUI?.Refresh();
        }

        public TooltipContent GetTooltipContent()
        {
            var shopRateDataTableRow = GetShopRateDataTableRow();
            if (shopRateDataTableRow == null)
            {
                return null;
            }

            return new TooltipContent(
                TooltipId,
                new Dictionary<string, object>
                {
                    { "level", shopRateDataTableRow.level },
                    { "cost", _level + 1 },
                    { "levelUpCostText", GetLevelUpCostText() },
                    { "tier1", shopRateDataTableRow.tier1 },
                    { "tier2", shopRateDataTableRow.tier2 },
                    { "tier3", shopRateDataTableRow.tier3 },
                });
        }

        private void UpdateExp(int exp)
        {
            for (var i = 0; i < expImages.Length; i++)
            {
                expImages[i].color = i < exp ? enabledColor : disabledColor;
            }
        }

        private void RequestLevelUp()
        {
            OnRequestLevelUp?.Invoke();
        }

        private ShopRateDataTableRow GetShopRateDataTableRow()
        {
            return DataTableManager.FindRow<ShopRateDataTableRow>(row => row.level == _level);
        }

        private string GetLevelUpCostText()
        {
            if (_level >= MaxLevel)
            {
                return "<color=green>Already at max level.</color>";
            }

            return $"Gold Required to Level Up: <color=#FFCE1B>{_level + 1}</color>";
        }

        private void OnDestroy()
        {
            levelUpButton.RemoveAllListeners();
        }
    }
}
