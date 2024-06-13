using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else
            {
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }
            Debug.Log(string.Format("Asignado {0} al jugador.", item.name));
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty()
        {
            return item == null;
        }
    }

    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();
    public List<PassiveData> availablePassives = new List<PassiveData>();
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();
    
    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    // Comprueba si el inventario tiene un objeto de tipo específico.
    public bool Has(ItemData type)
    {
        return Get(type);
    }

    public Item Get(ItemData type)
    {
        if (type is WeaponData)
        {
            return Get(type as WeaponData);
        }
        else if (type is PassiveData)
        {
            return Get(type as PassiveData);
        }
        return null;
    }

    // Encuentra un tesoro de un tipo concreto.
    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p.data == type)
            {
                return p;
            }
        }
        return null;
    }

    // Encuentra una habilidad de un tipo concreto.
    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w.data == type)
            {
                return w;
            }
        }
        return null;
    }

    public bool Remove(WeaponData data, bool removeUpgradeAvailability = false)
    {
        // Borra este arma o habilidad de la pool de upgrades.
        if (removeUpgradeAvailability)
        {
            availableWeapons.Remove(data);
        }

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;

            if (w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }

        return false;
    }

    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        // Borra esta pasiva o tesoro de la pool de upgrades.
        if (removeUpgradeAvailability) 
        { 
            availablePassives.Remove(data); 
        }

        for (int i = 0; i < passiveSlots.Count; i++)
        {
            Passive p = passiveSlots[i].item as Passive;
            if (p.data == data)
            {
                passiveSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }

        return false;
    }

    // Si le pasas un ItemData, determina el tipo y llama al Method Overload correspondiente.
    // También tenemos un bool opcional para borrar el objeto de la lista de mejoras.

    public bool Remove(ItemData data, bool removeUpgradeAvailability = false)
    {
        if (data is PassiveData)
        {
            return Remove(data as PassiveData, removeUpgradeAvailability);
        }
        else if (data is WeaponData)
        {
            return Remove(data as WeaponData, removeUpgradeAvailability);    
        }
        return false;
    }

    // Encuentra slot vacío y agrega una habilidad de tipo específico.
    // Devuelve el número del slot utilizado.

    public int Add(WeaponData data)
    {
        int slotNumber = -1;

        // Busca un slot vacío.
        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNumber = i;
                break;
            }
        }

        // Si no hay slot libre, sale.

        if (slotNumber < 0)
        {
            return slotNumber;
        }

        // Si lo encuentra, crea la habilidad en el slot.
        // Obtiene el tipo del habilidad que queremos generar.
        Type weaponType = Type.GetType(data.behaviour);

        if (weaponType != null)
        {
            // Genera el GameObject de la habilidad.
            GameObject gameObject = new GameObject(data.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon) gameObject.AddComponent(weaponType);
            spawnedWeapon.Initialise(data);
            spawnedWeapon.transform.SetParent(transform);
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.OnEuip();

            // Asigna la habilidad al slot.
            weaponSlots[slotNumber].Assign(spawnedWeapon);

            // Cierra la pantalla de level up si sigue mostrándose.
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }

            return slotNumber;
        }
        else
        {
            Debug.LogWarning(string.Format("Tipo de arma especificado inválido para {0}", data.name));
        }

        return -1;
    }

    // Encuentra un slot vacío de tesoro de cierto tipo.
    // Devuelve el slot en el que lo coloca.

    public int Add(PassiveData data)
    {
        int slotNumber = -1;

        // Busca un slot vacío.
        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNumber = i;
                break;
            }
        }

        // Si no hay slot vacío, sale.
        if (slotNumber < 0)
        {
            return slotNumber;
        }

        // Si existe, crea el tesoro en el slot.
        // Obtiene el tipo de tesoro que queremos generar.
        GameObject gameObject = new GameObject(data.baseStats.name + " Passive");
        Passive p = gameObject.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform);
        p.transform.localPosition = Vector2.zero;

        // Asigna el tesoro al slot.
        passiveSlots[slotNumber].Assign(p);

        // Cierra la pantalla de level up si sigue mostrándose.
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();

        return slotNumber;
    }

    // Esta función determina el tipo de objeto que se está agregando.
    public int Add(ItemData data)
    {
        if (data is WeaponData)
        {
            return Add(data as WeaponData);
        }
        else if (data is PassiveData)
        {
            return Add(data as PassiveData);
        }
        return -1;
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex].item as Weapon;

            // Si la habilidad está a máximo nivel, no levearla.

            if (!weapon.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Fallo al subir nivel a {0}", weapon.name));
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveSlots.Count > slotIndex)
        {
            Passive passive = passiveSlots[slotIndex].item as Passive;

            // Si el tesoro está a máximo nivel, no levearlo.

            if (!passive.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Fallo al subir nivel a {0}", passive.name));
                return;
            }
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
    }

    // Determina qué opciones a mejorar deberían aparecer.

    void ApplyUpgradeOptions()
    {
        // Crea un duplicado de las opciones disponibles para
        // iterar a través de ellas en la función.

        List<WeaponData> availableWeaponUpgrades = new List<WeaponData>(availableWeapons);
        List<PassiveData> availablePassiveItemUpgrades = new List<PassiveData>(availablePassives);

        // Itera por cada slot en la UI de mejoras.
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            // Si no hay más mejoras disponibles, aborta.
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                return;
            }

            // Determina si es una mejora para tesoros o habilidades.
            int upgradeType;
            if (availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                upgradeType = UnityEngine.Random.Range(1, 3);
            }

            // Genera una mejora de habilidad
            if (upgradeType == 1)
            {
                WeaponData chosenWeaponUpgrade = availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);

                // Se asegura de que los datos de habilidad son válidos.
                if (chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    // Cicla entre nuestras habilidades.
                    // Si encuentra un match, la sube de nivel si
                    // la opción es seleccionada.
                    bool isLevelUp = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        Weapon w = weaponSlots[i].item as Weapon;
                        if (w != null && w.data == chosenWeaponUpgrade)
                        {
                            // Si está a nivel máximo, no se permite la mejora.
                            if (chosenWeaponUpgrade.maxLevel <= w.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            // Configura el Event Listener, item y nivel para
                            // corresponder a los del siguiente nivel.
                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i));
                            Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    // Si llega a este punto, será porque estamos agregando
                    // una habilidad nueva, y no mejorando una existente.

                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade));
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name;
                        upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                    }
                }
            }
            else if (upgradeType == 2)
            {
                // Este sistema será recodeado en un futuro, pues actualmente
                // genera un problema eliminando opciones de selección cuando una
                // habilidad llega al nivel máximo.

                // Este código hace lo mismo que el if de arriba en aspectos generales.
                // Por ello, me ahorraré comentarlo.

                PassiveData chosenPassiveUpgrade = availablePassiveItemUpgrades[UnityEngine.Random.Range(0, availablePassiveItemUpgrades.Count)];
                availablePassiveItemUpgrades.Remove(chosenPassiveUpgrade);

                if (chosenPassiveUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool isLevelUp = false;
                    for (int i = 0; i < passiveSlots.Count; i++)
                    {
                        Passive p = passiveSlots[i].item as Passive;
                        if (p != null && p.data == chosenPassiveUpgrade)
                        {
                            if (chosenPassiveUpgrade.maxLevel <= p.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i));
                            Passive.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade));
                        //Passive.Modifier nextLevel = chosenPassiveUpgrade.baseStats;
                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveUpgrade.baseStats.description;
                        upgradeOption.upgradeNameDisplay.text = chosenPassiveUpgrade.baseStats.name;
                        upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                    }
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}