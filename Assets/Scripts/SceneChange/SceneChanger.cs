using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    public Animator fadeAnim;
    public float fadeTime = .5f;
    public Vector2 newPlayerPosition;
    private Transform player;

    void Start()
    {
        // Tìm FadeCanvas trong scene
        GameObject fadeCanvas = GameObject.Find("FadeCanvas");
        if (fadeCanvas != null)
        {
            fadeAnim = fadeCanvas.GetComponentInChildren<Animator>();
            if (fadeAnim == null)
            {
                Debug.LogWarning("Animator not found on FadeCanvas for teleporter at position: " + transform.position);
            }
        }
        else
        {
            Debug.LogWarning("FadeCanvas not found for teleporter at position: " + transform.position);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = collision.transform;
            fadeAnim.Play("FadeToWhite");
            StartCoroutine(DelayFade());
        }
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeTime);
        player.position = newPlayerPosition;
        SceneManager.LoadScene(sceneToLoad);
    }
}
