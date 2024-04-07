using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script base para el comportamiento de todas las armas que lanzan proyectiles [se colocan en los prefabs de las armas proyectiles].
/// </summary>

public class ProjectileWeaponBehaviour : MonoBehaviour
{

    protected Vector3 direction;
    public float destroyAfterSeconds;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }
    
    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;
    }
}
