using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sustituto de PassiveItemScriptableObject.
/// La idea es guardar todos los datos de nivel
/// del objeto pasivo o tesoro en un solo objeto,
/// en vez de en múltiples como anteriormente.
/// </summary>
[CreateAssetMenu(fileName = "Passive Data", menuName = "PSS2D Rogue-like/Passive Data")]

public class PassiveData : ItemData
{
    public Passive.Modifier baseStats;
    public Passive.Modifier[] growth;

    public Passive.Modifier GetLevelData(int level)
    {
        if (level - 2 < growth.Length)
        {
            return growth[level - 2];
        }

        Debug.LogWarning(string.Format("El tesoro no tiene sus stats configuradas para el nivel {0}.", level));
        return new Passive.Modifier();
    }
}
