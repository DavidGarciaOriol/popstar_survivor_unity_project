using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFood : Pickup
{
    public int healthToRestore;


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

        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.RestoreHealth(player.MaxHealth * healthToRestore / 100);
    }
}
