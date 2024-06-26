using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Este script ser� reemplazado.")]
public class WeaponStarController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedStarProjectile = Instantiate(weaponData.Prefab);
        spawnedStarProjectile.transform.position = transform.position; // La posici�n ser� la misma asignada al objeto, el cual est� asociado a la de Player.
        spawnedStarProjectile.GetComponent<WeaponStarBehaviour>().DirectionChecker(playerMovement.lastMovedVector); // Referencia y establece la direcci�n.
    }

}
