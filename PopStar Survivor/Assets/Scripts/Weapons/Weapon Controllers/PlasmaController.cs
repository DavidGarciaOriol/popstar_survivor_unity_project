using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedPlasmaArea = Instantiate(weaponData.Prefab);
        spawnedPlasmaArea.transform.position = transform.position;
        spawnedPlasmaArea.transform.parent = transform;
    }
}
