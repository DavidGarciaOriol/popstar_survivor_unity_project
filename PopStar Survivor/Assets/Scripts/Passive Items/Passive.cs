using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : Item
{
    public PassiveData data;
    [SerializeField] CharacterData.Stats currentBoosts;

    [System.Serializable]

    public struct Modifier
    {
        public string name, description;
        public CharacterData.Stats boosts;
    }

    // Inicializa los tesoros creados dinámicamente.
    public virtual void Initialise(PassiveData data)
    {
        base.Initialise(data);
        this.data = data;
        currentBoosts = data.baseStats.boosts;
    }

    public virtual CharacterData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("No puede subir de nivel{0} a nivel {1}, nivel máximo de {2} alcanzado",
                name, currentLevel, data.maxLevel));
            return false;
        }

        currentBoosts += data.GetLevelData(++currentLevel).boosts;
        return true;
    }

}
