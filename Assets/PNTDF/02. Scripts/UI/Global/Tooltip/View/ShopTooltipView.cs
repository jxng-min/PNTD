using JxModule;
using TMPro;
using UnityEngine;

namespace PNTD
{
    public class ShopTooltipView : TooltipView
    {
        [Space(30f)]
        [BigHeader("Lines")]
        [SerializeField] private TMP_Text headerLabel;
        [SerializeField] private TMP_Text tagLabel;
        [SerializeField] private TMP_Text bodyLabel;
        
        [Space(30f)]
        [BigHeader("Separate Line")]
        [SerializeField] private TMP_Text separateLabel;
        
        [Space(30f)]
        [BigHeader("Additional Lines")]
        [SerializeField] private TMP_Text additionalHeaderLabel;
        [SerializeField] private TMP_Text additionalTagLabel;
        [SerializeField] private TMP_Text additionalBodyLabel;
        
        protected override void Bind(TooltipDataTableRow tooltipDataTableRow, TooltipContent tooltipContent)
        {
            if (tooltipDataTableRow == null || tooltipContent == null)
            {
                Clear();
                return;
            }
            
            BindLabel(headerLabel, tooltipDataTableRow.headerText, tooltipContent);
            BindLabel(tagLabel, tooltipDataTableRow.tagText, tooltipContent);
            BindLabel(bodyLabel, tooltipDataTableRow.bodyText, tooltipContent);
            
            BindLabel(additionalHeaderLabel, tooltipDataTableRow.additionalHeaderText, tooltipContent);
            BindLabel(additionalTagLabel, tooltipDataTableRow.additionalTagText, tooltipContent);
            BindLabel(additionalBodyLabel, tooltipDataTableRow.additionalBodyText, tooltipContent);
            
            Canvas.ForceUpdateCanvases();
            BuildSeparateLine();
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

        private void BuildSeparateLine()
        {
            if (separateLabel == null)
            {
                return;
            }

            var maxWidth = GetMaxLabelWidth();

            if (maxWidth <= 0f)
            {
                ClearLabel(separateLabel);
                return;
            }

            var dashWidth = separateLabel.GetPreferredValues("-", Mathf.Infinity, Mathf.Infinity).x;
            if (dashWidth <= 0f)
            {
                ClearLabel(separateLabel);
                return;
            }
            
            var dashCount = Mathf.FloorToInt(maxWidth / dashWidth) + 1;

            separateLabel.text = new string('-', dashCount);
            separateLabel.gameObject.SetActive(true);
        }
        
        private float GetMaxLabelWidth()
        {
            var maxWidth = 0f;

            maxWidth = Mathf.Max(maxWidth, GetLabelWidth(headerLabel));
            maxWidth = Mathf.Max(maxWidth, GetLabelWidth(tagLabel));
            maxWidth = Mathf.Max(maxWidth, GetLabelWidth(bodyLabel));

            maxWidth = Mathf.Max(maxWidth, GetLabelWidth(additionalHeaderLabel));
            maxWidth = Mathf.Max(maxWidth, GetLabelWidth(additionalTagLabel));
            maxWidth = Mathf.Max(maxWidth, GetLabelWidth(additionalBodyLabel));

            return maxWidth;
        }

        private static float GetLabelWidth(TMP_Text label)
        {
            if (label == null || !label.gameObject.activeSelf)
            {
                return 0f;
            }

            return label.preferredWidth;
        }

        private void Clear()
        {
            ClearLabel(headerLabel);
            ClearLabel(tagLabel);
            ClearLabel(bodyLabel);

            ClearLabel(additionalHeaderLabel);
            ClearLabel(additionalTagLabel);
            ClearLabel(additionalBodyLabel);
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