using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script base para todos los controladores de armas
/// </summary>
[System.Obsolete("Este script será reemplazado.")]
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData;
    float currentCooldown;

    protected PlayerMovement playerMovement;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        currentCooldown = weaponData.CooldownDuration; // Al inicio el cooldown actual será la duración base.
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f) // Cuando el cooldown sea 0
        {
            Attack();
        } 
    }

    protected virtual void Attack()
    {
        currentCooldown = weaponData.CooldownDuration;
    }
}
