using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script base para el comportamiento de las armas melee [colocar en el prefab de armas melee].
/// </summary>

public class MeleeWeaponBehaviour : MonoBehaviour
{

    public WeaponScriptableObject weaponData;

    public float destroyAfterSeconds;

    // Estadísticas actuales
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;

    private void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.Pierce;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(currentDamage);
        }
    }
}
