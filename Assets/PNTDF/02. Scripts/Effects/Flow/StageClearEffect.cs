using System.Collections;
using JxModule;
using UnityEngine;
using UnityEngine.UI;

namespace PNTD
{
    [ManagedEffect("Flow", "Stage Clear", 2)]
    public class StageClearEffect : MonoBehaviour
    {
        [BigHeader("Settings")]
        [Header("Gold")]
        [SerializeField] private float goldTypingSpeed = 10f;
        [SerializeField] private float goldDelay = 0.25f;
        
        [Header("Interest")]
        [SerializeField] private float interestSpeed = 10f;
        [SerializeField] private float interestDelay = 0.25f;
        
        [Header("Total")]
        [SerializeField]  private float totalTypingSpeed = 10f;
        [SerializeField] private float totalDelay = 0.25f;

        public IEnumerator PlayStageClearEffect(int gold, 
                                                int bonus, 
                                                int interest, 
                                                CanvasGroup clearGroup,
                                                ShadowableLabel goldLabel,
                                                ShadowableLabel interestLabel,
                                                ShadowableLabel totalLabel)
        {
            clearGroup.Show();
            
            goldLabel.SetText(string.Empty);
            interestLabel.SetText(string.Empty);
            totalLabel.SetText(string.Empty);
            
            goldLabel.SetText(bonus > 0 ? $"gold earned: {gold}+{bonus}" : $"gold earned: {gold}");
            yield return goldLabel.TypeRoutine(goldTypingSpeed);
            yield return new WaitForSeconds(goldDelay);
            
            interestLabel.SetText($"interest: {interest}");
            yield return interestLabel.TypeRoutine(interestSpeed);
            yield return new WaitForSeconds(interestDelay);
            
            totalLabel.SetText($"total: {gold + bonus + interest}");
            yield return totalLabel.TypeRoutine(totalTypingSpeed);
            yield return new WaitForSeconds(totalDelay);
            
            clearGroup.Hide();
        }
    }
}