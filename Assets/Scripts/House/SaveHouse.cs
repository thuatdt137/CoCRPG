using UnityEngine;

public class SaveHouse : MonoBehaviour
{
    public Animator anim;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSaveHandler saveHandler = other.GetComponent<PlayerSaveHandler>();
            if (saveHandler != null)
            {
                saveHandler.canSaveLoad = true;
                // Có thể thêm hiệu ứng, như hiển thị thông báo "Có thể lưu tại đây"
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSaveHandler saveHandler = other.GetComponent<PlayerSaveHandler>();
            if (saveHandler != null)
            {
                saveHandler.canSaveLoad = false;
                // Ẩn thông báo nếu có
            }
        }
    }
}