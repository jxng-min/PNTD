using JxModule;
using TMPro;
using UnityEngine;

namespace PNTD
{
    public class PaletteTooltipView : TooltipView
    {
        [Space(30f)]
        [BigHeader("Lines")]
        [SerializeField] private TMP_Text headerLabel;
        [SerializeField] private TMP_Text tagLabel;

        protected override void Bind(TooltipDataTableRow tooltipDataTableRow, TooltipContent tooltipContent)
        {
            if (tooltipDataTableRow == null || tooltipContent == null)
            {
                Clear();
                return;
            }

            BindLabel(headerLabel, tooltipDataTableRow.headerText, tooltipContent);
            BindLabel(tagLabel, tooltipDataTableRow.tagText, tooltipContent);
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
            ClearLabel(tagLabel);
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
