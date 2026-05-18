using UnityEngine;

public class PlayerAutoAttack : MonoBehaviour
{
    private Animator animator;

    [Header("Saldırı Zamanlama Ayarları")]
    public float attackCooldown = 1.2f; // Karakter kaç saniyede bir otomatik vuracak?
    private float attackTimer = 0f;

    void Start()
    {
        // Karakterin üzerindeki Animator bileşenini otomatik buluyoruz
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Zamanlayıcıyı her karede saniye cinsinden ilerletiyoruz
        attackTimer += Time.deltaTime;

        // Saldırı süresi geldiğinde bu blok çalışır
        if (attackTimer >= attackCooldown)
        {
            OtomatikRastgeleSaldiri();
            attackTimer = 0f; // Süreyi sıfırla, yeniden saysın
        }
    }

    void OtomatikRastgeleSaldiri()
{
    // 0, 1 veya 2 tam sayılarından birini rastgele seçiyoruz (Int uyumlu)
    // Not: Random.Range(int, int) üst sınırı dahil etmez, bu yüzden 0 ile 3 yazıyoruz ki 0, 1, 2 seçilsin.
    int rastgeleVurus = Random.Range(0, 3); 

    // Unity'ye zorla "Ben tam sayı gönderiyorum" diyoruz (SetInteger yaptık)
    animator.SetInteger("AttackIndex", rastgeleVurus);

    // Ve saldırı animasyonunu ateşliyoruz!
    animator.SetTrigger("Attack");
}
}