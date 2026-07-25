using System;
using JxModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    public class ShopLevelView : ViewBase
    {
        [BigHeader("UI")]
        [SerializeField] private JxButton levelUpButton;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private Image[] expImages;
        
        [Space(30f)]
        [BigHeader("Settings")]
        [SerializeField] private Color enabledColor;
        [SerializeField] private Color disabledColor;

        public event Action OnRequestLevelUp;

        private void Awake()
        {
            levelUpButton.AddListener(RequestLevelUp);   
        }

        public void Initialize(int level, int exp)
        {
            UpdateLevel(level, exp);
        }

        public void UpdateLevel(int level, int exp)
        {
            levelLabel.text = level.ToString();
            UpdateExp(exp);
        }

        private void UpdateExp(int exp)
        {
            for (var i = 0; i < expImages.Length; i++)
            {
                expImages[i].color = i < exp ? disabledColor : enabledColor;
            }
        }

        private void RequestLevelUp()
        {
            OnRequestLevelUp?.Invoke();
        }

        private void OnDestroy()
        {
            levelUpButton.RemoveAllListeners();
        }
    }
}