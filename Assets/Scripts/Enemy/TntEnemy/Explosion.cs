using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float damage = 5f;
    public float lifetime = 0.5f; // Hiệu ứng tồn tại 0.5s

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Gọi hàm gây sát thương ở đây
            Debug.Log("Player bị trúng bom!");
        }
    }
}
