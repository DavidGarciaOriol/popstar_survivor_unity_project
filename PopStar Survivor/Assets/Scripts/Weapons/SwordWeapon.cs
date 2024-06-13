using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : ProjectileWeapon
{

    // La cantidad de veces que atacará la espada en una instancia.
    int currentSpawnCount;

    protected override bool Attack(int attackCount = 1)
    {
        // Si no hay prefab asignado al proyectil.
        if (!currentStats.projectilePrefab) 
        {
            Debug.LogWarning(string.Format("No existe el prefab del proyectil {0}", name));
            currentCooldown = data.baseStats.cooldown;
            return false; 
        }

        // Si no existe el proyectil asignado.
        if (!CanAttack())
        {
            return false;
        }

        // La primera vez que atacamos.
        if (currentCooldown <= 0)
        {
            currentSpawnCount = 0;
        }

        // Calculamos el ángulo. Si el ataque es impar, cambiamos la dirección.

        float spawnDir = Mathf.Sign(movement.lastMovedVector.x) * (currentSpawnCount % 2 != 0 ? -1 : 1);
        Vector2 spawnOffset = new Vector2(
            spawnDir * Random.Range(currentStats.spawnVariance.xMin,
            currentStats.spawnVariance.xMax), 0
        );

        // Generamos la copia del proyectil.

        currentStats.speed = 0;
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)spawnOffset,
            Quaternion.identity
        );
        
        prefab.owner = owner;

        // Invierte el sprite del proyectil.
        if (spawnDir < 0)
        {
            prefab.transform.localScale = new Vector3(
                -Mathf.Abs(prefab.transform.localScale.x),
                prefab.transform.localScale.y,
                prefab.transform.localScale.z
            );
            Debug.Log(spawnDir + " | " + prefab.transform.localScale);
        }

        // Asigna las stats.
        prefab.weapon = this;
        currentCooldown = data.baseStats.cooldown;
        attackCount--;
        currentSpawnCount++;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }

}
