using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaAreaBehaviour : MeleeWeaponBehaviour
{

    List<GameObject> markedEnemies;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !markedEnemies.Contains(col.gameObject))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(currentDamage, transform.position);
            markedEnemies.Add(col.gameObject); // Marca al enemigo apra que no reciba daño de este ataque de nuevo.
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable)
                && !markedEnemies.Contains(col.gameObject))
            {
                breakable.TakeDamage(currentDamage);
                markedEnemies.Add(col.gameObject);
            }

        }

    }
}
