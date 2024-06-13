using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        PlayerInventory playerInventory = collision.GetComponent<PlayerInventory>();

        if (playerInventory)
        {
            // bool randomBool = Random.Range(0, 2) == 0;

            OpenTreasureChest(playerInventory, true);
            Destroy(gameObject);
        }
    }

    public void OpenTreasureChest(PlayerInventory inventory, bool isHigherTier)
    {
        
        // Cicla entre las armas para comprobar si pueden evolucionar.

        foreach (PlayerInventory.Slot slot in inventory.weaponSlots)
        {
            Weapon weapon = slot.item as Weapon;
            if (weapon.data.evolutionData == null)
            {
                // Ignora el arma si no puede evolucionar.
                continue;
            }

            // Cicla entre todas las posibles evoluciones del arma.
            foreach (ItemData.Evolution evolution in weapon.data.evolutionData)
            {
                // Solo intenta evolucionar armas cuya condición sea usar un cofre.
                if (evolution.condition == ItemData.Evolution.Condition.treasureChest)
                {
                    bool attempt = weapon.AttemptEvolution(evolution, 0);
                    if (attempt)
                    {
                        return;
                    }
                }
            }
        }
    }
}
