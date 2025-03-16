using UnityEngine;

public class Player_ChangeEquipment : MonoBehaviour
{
    public Player_Combat combat;
    public Player_Bow bow;

    void Update()
    {
        if (Input.GetButtonDown("ChangeEquipment"))
        {
            combat.enabled = !combat.enabled;
            bow.enabled = !bow.enabled;
            if (bow.enabled)
            {
                transform.localScale = new Vector3(1f, 1f, 0);
            }
            else
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 0);

            }
        }
    }
}
