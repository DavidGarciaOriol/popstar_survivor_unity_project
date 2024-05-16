using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, ICollectible
{
    protected bool hasBeenCollected = false;

    public virtual void Collect()
    {
        hasBeenCollected = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) // Si el objeto se acerca al jugador con el magnet, se destruye.
        {
            Destroy(gameObject);
        }
    }
}
