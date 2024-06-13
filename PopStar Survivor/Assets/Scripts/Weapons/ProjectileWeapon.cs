using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    // Intervalo entre ataques.
    protected float currentAttackInterval;

    // Número de veces que ocurre el ataque entre intervalos.
    protected int currentAttackCount;

    protected override void Update()
    {
        base.Update();

        if (currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;

            if (currentAttackInterval <= 0)
            {
                Attack(currentAttackCount);
            }
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0)
        {
            return true;
        }

        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("No hay un prefab de proyectil para {0}", name));
            currentCooldown = data.baseStats.cooldown;
            return false;
        }

        // Comprobamos que podamos atacar.
        if ((!CanAttack()))
        {
            return false;
        }

        // Obtenemos el ángulo de disparo.
        float spawnAngle = GetSpawnAngle();

        //Generamos la copia del proyectil.
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
            Quaternion.Euler(0, 0, spawnAngle)
        );
        
        prefab.weapon = this;
        prefab.owner = owner;

        // Reiniciamos el cooldown solo si el ataque fue activado por el cooldown.
        if (currentCooldown <= 0)
        {
            currentCooldown += currentStats.cooldown;
        }

        attackCount--;

        // Si quedan ataques por el número, seguimos atacando.
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }

    protected virtual float GetSpawnAngle() 
    {
        return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
    }

    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }

    /*protected virtual float GetProjectileSpeed()
    {
        return currentStats.speed * owner.Speed;
    }*/
}
