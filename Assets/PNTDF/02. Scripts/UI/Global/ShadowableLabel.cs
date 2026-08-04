using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace PNTD
{
    public class ShadowableLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text visualLabel;
        [SerializeField] private TMP_Text shadowLabel;
        
        private static readonly Regex ColorTagRegex = new(@"<\/?color(?:=[^>]+)?>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public void SetText(string text)
        {
            visualLabel.text = text;
            
            var removedColorTagText = RemoveColorTag(text);
            shadowLabel.text = removedColorTagText;
        }

        public IEnumerator TypeRoutine(float charPerSecond)
        {
            var targetText = visualLabel.text;
            visualLabel.text = string.Empty;
            shadowLabel.text = string.Empty;

            if (charPerSecond <= 0f)
            {
                visualLabel.text = targetText;
                shadowLabel.text = targetText;
                yield break;
            }

            var delay = 1f / charPerSecond;
            var currentText = string.Empty;

            foreach (var ch in targetText)
            {
                currentText += ch;
                visualLabel.text = currentText;
                shadowLabel.text = currentText;
                yield return new WaitForSeconds(delay);
            }
        }

        private static string RemoveColorTag(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            
            return ColorTagRegex.Replace(text, string.Empty);
        }
    }
}