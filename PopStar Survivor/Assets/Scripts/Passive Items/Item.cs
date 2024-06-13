using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase base para los tesoros (pasivos) y las habilidades (armas).
/// Con esto podremos manejar principalmente también las evoluciones
/// de las armas y de los tesoros.
/// </summary>
public class Item : MonoBehaviour
{

    public int currentLevel = 1, maxLevel = 1;
    protected ItemData.Evolution[] evolutionData;
    protected PlayerInventory inventory;
    protected PlayerStats owner;

    public virtual void Initialise(ItemData data)
    {
        maxLevel = data.maxLevel;
        evolutionData = data.evolutionData;

        inventory = FindObjectOfType<PlayerInventory>();
        owner = FindObjectOfType<PlayerStats>();
    }

    // Comprueba todas las posibles evoluciones de una habilidad.
    public virtual ItemData.Evolution[] CanEvolve()
    {
        List<ItemData.Evolution> possibleEvolutions = new List<ItemData.Evolution>();

        // Comprueba las evoluciones listadas y si están en el inventario.
        foreach (ItemData.Evolution e in evolutionData)
        {
            if (CanEvolve(e))
            {
                possibleEvolutions.Add(e);
            }
        }
        return possibleEvolutions.ToArray();
    }

    // Comprueba si una evolución específica es posible
    public virtual bool CanEvolve(ItemData.Evolution evolution, int levelUpAmount = 1)
    {
        // El objeto debe tener el nivel para evolucionar.
        if (evolution.evolutionLevel > currentLevel + levelUpAmount)
        {
            Debug.LogWarning(string.Format("La evolución ha fallado. Nivel actual {0}. Nivel necesario {1}",
                currentLevel, evolution.evolutionLevel));
            return false;
        }

        // Comprueba que todos los ingredientes/catalizadores estén en el inventario
        foreach (ItemData.Evolution.Config c in evolution.catalysts)
        {
            Item item = inventory.Get(c.itemType);
            if (!item || item.currentLevel < c.level) 
            {
                Debug.LogWarning(string.Format("La evolución ha fallado. Falta {0}",
                    c.itemType.name));
                return false;
            }
        }
        return true;
    }

    // Genera el arma nueva evolucionada.
    // Borra el arma consumida.
    public virtual bool AttemptEvolution(ItemData.Evolution evolutionData, int levelUpAmount = 1)
    {
        if (!CanEvolve(evolutionData, levelUpAmount))
        {
            return false;
        }

        // ¿Consumir tesoros?
        // ¿Consumir habilidades?
        bool consumePassives = (evolutionData.consumes & ItemData.Evolution.Consumption.passives) > 0;
        bool consumeWeapons = (evolutionData.consumes & ItemData.Evolution.Consumption.weapons) > 0;

        foreach (ItemData.Evolution.Config c in evolutionData.catalysts)
        {
            if (c.itemType is PassiveData && consumePassives)
            {
                inventory.Remove(c.itemType, true);
            }

            if (c.itemType is WeaponData && consumeWeapons)
            {
                inventory.Remove(c.itemType, true);
            }
        }

        // ¿Consumir también la propia evolución?
        if (this is Passive && consumePassives)
        {
            inventory.Remove((this as Passive).data, true);
        }
        else if (this is Weapon && consumeWeapons)
        {
            inventory.Remove((this as Weapon).data, true);
        }

        // Agregar nueva habilidad evolucionada al inventario.
        inventory.Add(evolutionData.outcome.itemType);

        return true;
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel <= maxLevel;
    }

    // Si un objeto sube de nivel, intenta evoluxionarlo.
    public virtual bool DoLevelUp()
    {
        if (evolutionData == null)
        {
            return true;
        }

        foreach (ItemData.Evolution e in evolutionData)
        {
            if (e.condition == ItemData.Evolution.Condition.auto)
            {
                AttemptEvolution(e);
            }
        }
        return true;
    }

    // Qué efecto recibes al tener el objeto equipado.
    public virtual void OnEuip()
    {

    }

    // Efecto que pierdes al perder el objeto.
    public virtual void OnUnequip()
    {

    }

}
