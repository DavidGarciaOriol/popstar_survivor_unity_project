using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script base para todos los controladores de armas
/// </summary>

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public GameObject prefab;
    public float damage;
    public float speed;
    public float cooldownDuration;
    float currentCooldown;
    public int pierce;

    protected PlayerMovement playerMovement;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        currentCooldown = cooldownDuration; // Al inicio el cooldown actual será la duración base.
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
        currentCooldown = cooldownDuration;
    }
}
