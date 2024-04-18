using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{

    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;

    private void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        playerCollector.radius = player.currentMagnet;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Comprueba si el objeto tiene la interfaz de ICollectible implementada
        if (col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            // Animación de atracción de objetos
            Rigidbody2D rigidbody2D = col.gameObject.GetComponent<Rigidbody2D>();
            Vector2 forceDirection = (transform.position - col.transform.position).normalized;
            rigidbody2D.AddForce(forceDirection * pullSpeed);

            collectible.Collect();
        }
    }

}
