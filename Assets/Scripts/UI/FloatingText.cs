using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 2f;      // Yazının yukarı çıkma hızı
    public float destroyTime = 1f;    // Kaç saniye sonra yok olacağı
    
    private TextMeshPro textMesh;
    private Color textColor;

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null) textColor = textMesh.color;
        
        // Doğduğu andan itibaren destroyTime saniye sonra kendini imha et
        Destroy(gameObject, destroyTime); 
    }

    void Update()
    {
        // Yazıyı her karede yukarı doğru hareket ettir
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        
        // Yazının şeffaflığını (Alpha) yavaşça sıfıra indir (Sönerek yok olma efekti)
        if (textMesh != null)
        {
            textColor.a -= (Time.deltaTime / destroyTime);
            textMesh.color = textColor;
        }
    }
}