using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Este script ser� reemplazado.")]
public class WeaponStarBehaviour : ProjectileWeaponBehaviour
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * currentSpeed * Time.deltaTime; // Movimiento del proyectil estrella
    }
}
