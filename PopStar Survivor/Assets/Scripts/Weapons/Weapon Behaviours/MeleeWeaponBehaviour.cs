using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script base para el comportamiento de las armas melee [colocar en el prefab de armas melee].
/// </summary>

public class MeleeWeaponBehaviour : MonoBehaviour
{

    public float destroyAfterSeconds;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

}
