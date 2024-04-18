using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceStar : Pickup, ICollectible
{

    public int experienceGranted;

    public void Collect()
    {
        Debug.Log("Called -> Item Collected");
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.IncreasedExperience(experienceGranted);
    }
}
