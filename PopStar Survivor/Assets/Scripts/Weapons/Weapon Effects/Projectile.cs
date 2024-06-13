using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Se agrega a todos los prefabs de proyectiles.
/// Todos los proyectiles vuelan en al dirección en la que miran
/// y hacen daño al golpear un objeto.
/// </summary>
public class Projectile : WeaponEffect
{
    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    protected Rigidbody2D rb;
    protected int piercing;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();

        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed;
        }

        // Prevenimos que el área sea 0
        float area = stats.area == 0 ? 1 : stats.area;

        transform.localScale = new Vector3(
            area * Mathf.Sign(transform.localScale.x),
            area * Mathf.Sign(transform.localScale.y), 1
        );

        // Indicamos el piercing del objeto
        piercing = stats.piercing;

        // Destruimos el proyectil tras acabar su lifespan
        if (stats.lifespan > 0)
        {
            Destroy(gameObject, stats.lifespan);
        }

        // Si tiene autoapuntado, le otorgamos la capacidad de buscar enemigos
        if (hasAutoAim)
        {
            AcquireAutoAimFacing();
        }
    }

    public virtual void AcquireAutoAimFacing()
    {
        // El ángulo de apuntado. A dónde disparará.
        float aimAngle;

        // Encontrar a los enemigos en pantalla
        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();

        // Selecciona enemigos aleatorios. Si no hay, dispara aleatoriamente.
        if (targets.Length > 0)
        {
            EnemyStats selectedTarget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        // Transforma el ángulo del proyectil para coincidir con la dirección de disparo.
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }

    protected virtual void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            stats.speed += owner.Speed;
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats enemyStats = other.GetComponent<EnemyStats>();
        BreakableProps prop = other.GetComponent<BreakableProps>();

        // Interactua solo con enemigos o props que se rompan.
        if (enemyStats)
        {
            // Si hay owner del proyectil y la fuente de daño es dicho owner,
            // el knockback se calcula usando el owner en vez del proyectil.
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
            
            // Hace daño y destruye el proyectil.
            enemyStats.TakeDamage(GetDamage(), source);

            Weapon.Stats stats = weapon.GetStats();
            piercing--;

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (prop)
        {
            prop.TakeDamage(GetDamage());
            piercing--;

            Weapon.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }

        if (piercing <= 0)
        {
            Destroy(gameObject);
        }
    }
}
