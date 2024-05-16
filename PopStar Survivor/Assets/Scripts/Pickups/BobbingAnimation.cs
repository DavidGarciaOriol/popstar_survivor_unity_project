using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float frequency; // Velocidad del movimiento.
    public float magnitude; // Rango del movimiento.
    public Vector3 direction; // Direcci�n del movimiento.

    Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // Funci�n para el efecto de tembleque/bobbing
        transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequency) * magnitude;

    }
}
