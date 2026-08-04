using JxModule;
using TMPro;
using UnityEngine;

namespace PNTD
{
    public class ShopLevelTooltipView : TooltipView
    {
        [Space(30f)]
        [BigHeader("Lines")]
        [SerializeField] private TMP_Text headerLabel;
        [SerializeField] private TMP_Text bodyLabel;

        protected override void Bind(TooltipDataTableRow tooltipDataTableRow, TooltipContent tooltipContent)
        {
            if (tooltipDataTableRow == null || tooltipContent == null)
            {
                Clear();
                return;
            }

            BindLabel(headerLabel, tooltipDataTableRow.headerText, tooltipContent);
            BindLabel(bodyLabel, tooltipDataTableRow.bodyText, tooltipContent);
        }

        private void BindLabel(TMP_Text label, string template, TooltipContent tooltipContent)
        {
            if (label == null)
            {
                return;
            }

            var text = TooltipTextFormatter.Format(template, tooltipContent.Values);
            label.text = text;
            label.gameObject.SetActive(!string.IsNullOrEmpty(text));
        }

        private void Clear()
        {
            ClearLabel(headerLabel);
            ClearLabel(bodyLabel);
        }

        private void ClearLabel(TMP_Text label)
        {
            if (label == null)
            {
                return;
            }

            label.text = string.Empty;
            label.gameObject.SetActive(false);
        }
    }
}
