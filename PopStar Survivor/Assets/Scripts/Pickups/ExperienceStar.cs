using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceStar : Pickup
{

    public int experienceGranted;

    public override void Collect()
    {
        if (hasBeenCollected)
        {
            return;
        }
        else
        {
            base.Collect();
        }

        Debug.Log("Called -> Item Collected");
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.IncreasedExperience(experienceGranted);
    }
}
