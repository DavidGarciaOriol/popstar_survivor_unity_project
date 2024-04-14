using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceStar : MonoBehaviour, ICollectible
{

    public int experienceGranted;

    public void Collect()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.IncreasedExperience(experienceGranted);
        Destroy(gameObject);
    }
}
