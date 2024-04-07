using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStarBehaviour : ProjectileWeaponBehaviour
{

    WeaponStarController swController;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        swController = FindObjectOfType<WeaponStarController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * swController.speed * Time.deltaTime; // Movimiento del proyectil estrella
    }
}
