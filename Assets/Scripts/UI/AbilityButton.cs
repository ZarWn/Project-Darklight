using UnityEngine;

public class AbilityButton : MonoBehaviour
{
    [Header("Yetenek Numarası (0, 1, 2, 3)")]
    public int abilitySlot; 

    public void OnButtonClick()
    {
        // 1. Önce hafızadaki hazır menajeri bulmayı dener
        ActiveAbilityManager manager = ActiveAbilityManager.Instance;
        
        // 2. Eğer bulamazsa, sahnedeki tüm objeleri tarayıp menajeri zorla bulur
        if (manager == null) 
        {
            manager = FindFirstObjectByType<ActiveAbilityManager>();
        }

        // 3. Bulduysa yeteneği çalıştırır
        if (manager != null)
        {
            manager.CastAbility(abilitySlot);
        }
        else
        {
            Debug.LogError("ActiveAbilityManager sahnede bulunamadı! Lütfen sahneye eklediğinden emin ol.");
        }
    }
}