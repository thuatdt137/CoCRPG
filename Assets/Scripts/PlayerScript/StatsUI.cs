using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlot;
    public CanvasGroup statsCanvas;

    private bool statsOpen = false;

    void Start()
    {
        UpdateAllStats();
    }

    void Update()
    {
        if (Input.GetButtonDown("ToggleStats"))
            if (statsOpen)
            {
                Time.timeScale = 1;
                UpdateAllStats();
                statsCanvas.alpha = 0;
                statsCanvas.blocksRaycasts = false;
                statsOpen = false;
            }
            else
            {
                Time.timeScale = 0;
                UpdateAllStats();
                statsCanvas.alpha = 1;
                statsCanvas.blocksRaycasts = true;
                statsOpen = true;
            }
    }
    public void UpdateDamage()
    {
        statsSlot[0].GetComponentInChildren<TMP_Text>().text = "Damage: " + StatsManager.Instance.damage;
    }
    public void UpdateSpeed()
    {
        statsSlot[1].GetComponentInChildren<TMP_Text>().text = "Speed: " + StatsManager.Instance.speed;
    }

    public void UpdateAllStats()
    {
        UpdateDamage();
        UpdateSpeed();
    }
}
