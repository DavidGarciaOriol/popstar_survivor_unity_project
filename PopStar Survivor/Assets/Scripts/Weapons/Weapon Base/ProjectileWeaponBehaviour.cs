using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script base para el comportamiento de todas las armas que lanzan proyectiles [se colocan en los prefabs de las armas proyectiles].
/// </summary>

public class ProjectileWeaponBehaviour : MonoBehaviour
{

    public WeaponScriptableObject weaponData;

    protected Vector3 direction;
    public float destroyAfterSeconds;

    // Estad�sticas actuales
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;

    void Awake()
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
    
    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;
    }

    protected void OnTriggerEnter2D(Collider2D col)
    {
        // Referencia al objeto del collider colisionado, y le hace da�o usando su funci�n TakeDamage()
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(currentDamage);
            ReducePierce();
        }
    }

    void ReducePierce() // Destruir proyectil al llegar el Piercing a 0
    {
        currentPierce--;

        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    } 
}
