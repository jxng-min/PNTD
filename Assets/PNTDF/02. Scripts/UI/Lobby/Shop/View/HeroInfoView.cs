using JxModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    public class HeroInfoView : MonoBehaviour, ITooltipProvider
    {
        [BigHeader("UI")]
        [SerializeField] private Image colorImage;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text costLabel;
        
        private HeroDataTableRow _heroDataTableRow;
        
        public bool CanShowTooltip => _heroDataTableRow != null;

        public void Initialize(HeroDataTableRow heroDataTableRow)
        {
            _heroDataTableRow = heroDataTableRow;
            
            colorImage.color = _heroDataTableRow.color;
            nameLabel.text = _heroDataTableRow.displayName;
            costLabel.text = $"{_heroDataTableRow.cost}";
        }

        public TooltipContent GetTooltipContent()
        {
            return HeroContextTooltipUtility.CreateShopSlot(_heroDataTableRow);
        }
    }
}