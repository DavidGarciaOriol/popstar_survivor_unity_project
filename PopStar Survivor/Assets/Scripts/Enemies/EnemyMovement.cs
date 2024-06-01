using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;

    Vector2 knockbackVelocity;
    float knockbackDuration;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (knockbackDuration > 0)
        {
            // Se aplica el movimiento del knockback si el enemigo está bajo el efecto de éste.
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            // De no estar en knockback, el enemigo se mueve de forma constante hacia el jugador.
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position,
                enemy.currentMoveSpeed * Time.deltaTime);

        }
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0)
        {
            return;
        }

        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}
