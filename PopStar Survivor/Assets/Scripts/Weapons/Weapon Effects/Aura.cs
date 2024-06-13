using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Area de efecto de daño continuo por intervalos de tiempo.
/// </summary>
public class Aura : WeaponEffect
{

    Dictionary<EnemyStats, float> affectedTargets = new Dictionary<EnemyStats, float>();
    List<EnemyStats> targetsToUnaffect = new List<EnemyStats>();

    // Update is called once per frame
    void Update()
    {
        Dictionary<EnemyStats, float> affectedTargetsCopy = new Dictionary<EnemyStats, float>(affectedTargets);

        // Cicla entre todos los enemigos afectados por el aura, y reduce el cooldown.
        // Si el cooldown llega a cero, vuelve a recibir daño.
        foreach (KeyValuePair<EnemyStats, float> pair in affectedTargetsCopy)
        {
            affectedTargets[pair.Key] -= Time.deltaTime;

            if (pair.Value <= 0)
            {
                if (targetsToUnaffect.Contains(pair.Key))
                {
                    affectedTargets.Remove(pair.Key);
                    targetsToUnaffect.Remove(pair.Key);
                }
                else
                {
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTargets[pair.Key] = stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyStats enemyStats))
        {
            // Si el enemigo no es afectado por el aura, es agregado
            // a la lista de afectados.

            if (!affectedTargets.ContainsKey(enemyStats))
            {
                affectedTargets.Add(enemyStats, 0);
            }
            else
            {
                if (targetsToUnaffect.Contains(enemyStats))
                {
                    targetsToUnaffect.Remove(enemyStats);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyStats enemyStats))
        {
            // El enemigo objetivo no debe dejar de ser trackeado
            // aunque abandone el aura, pues necesitamos seguir
            // calculando su cooldown.

            if (affectedTargets.ContainsKey(enemyStats))
            {
                targetsToUnaffect.Add(enemyStats);
            }
        }
    }

}
