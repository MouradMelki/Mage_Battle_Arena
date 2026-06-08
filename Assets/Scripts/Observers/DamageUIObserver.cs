using System;
using UnityEngine;
using UnityEngine.UI;

public class DamageUIObserver : Observer
{
    private PlayerController playerController;
    private GameObject DamageImage;
    private Image damageImage;
    private readonly float FlashSpeed = 5f;
    private Color flashColour = new Color(10f, 0f, 0f, 0.3f);

    public DamageUIObserver(PlayerController playerController)
    {
        DamageImage = GameObject.FindGameObjectWithTag("DamageImage");
        if (DamageImage)
        {
            damageImage = DamageImage.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("DamageUIObserver requires a DamageImage tagged Image object in the scene.");
        }

        this.playerController = playerController;
        this.playerController.Attach(this);
    }

    public override void UpdateObserver()
    {
        FlashDamage(playerController.Damaged);
    }

    private void FlashDamage(bool damaged)
    {
        if (!damageImage)
        {
            return;
        }

        if (damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, FlashSpeed * Time.deltaTime);
        }
    }
}
