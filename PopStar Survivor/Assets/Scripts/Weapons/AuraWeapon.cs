using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraWeapon : Weapon
{

    protected Aura currentAura;

    protected override void Update() { }

    public override void OnEuip()
    {
        // Intenta reemplazar el aura del arma con una nueva.

        if (currentStats.auraPrefab)
        {
            if (currentAura)
            {
                Destroy(currentAura);
            }

            currentAura = Instantiate(currentStats.auraPrefab, transform);
            currentAura.weapon = this;
            currentAura.owner = owner;
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
    }

    public override void OnUnequip()
    {
        if (currentAura)
        {
            Destroy(currentAura);
        }
    }

    public override bool DoLevelUp()
    {
        if (!base.DoLevelUp())
        {
            return false; 
        }

        // Se actualiza el aura de haber una ya en el arma.

        if (currentAura)
        {
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
        return true;
    }
}
