using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GameObject spawnedStarProjectile = Instantiate(prefab);
        spawnedStarProjectile.transform.position = transform.position; // La posición será la misma asignada al objeto, el cual está asociado a la de Player.
        spawnedStarProjectile.GetComponent<WeaponStarBehaviour>().DirectionChecker(playerMovement.lastMovedVector); // Referencia y establece la dirección.
    }

}
