using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D col)
    {
        // Comprueba si el objeto tiene la interfaz de ICollectible implementada
        if (col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            collectible.Collect();
        }
    }

}
