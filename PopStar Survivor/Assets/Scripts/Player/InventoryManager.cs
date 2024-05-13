using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>();
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
    public int[] passiveItemLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>();

    [System.Serializable]
    public class WeaponUpgrade
    {
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
    }

    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public GameObject initialPassiveItem;
        public PassiveItemScriptableObject passiveItemData;
    }

    [System.Serializable]
    public class UpgradeUI
    {
        public Text upgradeNameDisplay;
        public Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>(); // Lista de opciones de mejora de habilidades (armas).
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>(); // Lista de opciones de mejora de tesoros (pasivos).
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>(); // Lista de mejoras que se muestran en la pantalla de selección de opción de mejora.

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    // Añade un arma a un slot
    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    // Añade un objeto pasivo a un slot
    public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)
    {
        passiveItemSlots[slotIndex] = passiveItem;
        passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level;
        passiveItemUISlots[slotIndex].enabled = true;
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.Icon;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpWeapon(int slotIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];

            if (!weapon.weaponData.NextLevelPrefab)
            {
                Debug.LogError("Este arma ya no tiene un nivel superior: " + weapon.name);
                return;
            }

            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform);
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level;

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    public void LevelUpPassiveItem(int slotIndex)
    {
        if (passiveItemSlots.Count > slotIndex)
        {
            PassiveItem passiveItem = passiveItemSlots[slotIndex];

            if (!passiveItem.passiveItemData.NextLevelPrefab)
            {
                Debug.LogError("Este tesoro ya no tiene un nivel superior: " + passiveItem.name);
                return;
            }

            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform);
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>());
            Destroy(passiveItem.gameObject);
            passiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level;

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    void ApplyUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUIOptions)
        {
            int upgradeType = Random.Range(1, 3); // Elije entre Habilidad y Tesoro.

            if (upgradeType == 1)
            {
                WeaponUpgrade chosenWeaponUpgrade = weaponUpgradeOptions[Random.Range(0, weaponUpgradeOptions.Count)];
                
                if (chosenWeaponUpgrade != null)
                {
                    bool newWeapon = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    { 
                        if (weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData)
                        {
                            newWeapon = false;

                            if (!newWeapon)
                            {
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i)); // Le aplica la funcionalidad al botón.
                                
                                // Descripción y nombre del siguiente nivel de la habilidad / arma.
                                upgradeOption.upgradeDescriptionDisplay.text
                                    = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description;
                                upgradeOption.upgradeNameDisplay.text
                                    = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    } if (newWeapon){
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenWeaponUpgrade.initialWeapon)); // Le aplica la funcionalidad al botón.

                        // Descripción y nombre iniciales de la habilidad / arma.
                        upgradeOption.upgradeDescriptionDisplay.text =
                            chosenWeaponUpgrade.weaponData.Description;
                        upgradeOption.upgradeNameDisplay.text =
                            chosenWeaponUpgrade.weaponData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon;
                }
            }
            else if (upgradeType == 2)
            {
                PassiveItemUpgrade chosenPassiveItemUpgrade = passiveItemUpgradeOptions[Random.Range(0, passiveItemUpgradeOptions.Count)];

                if (chosenPassiveItemUpgrade != null)
                {

                    bool newPassiveItem = false;
                    for (int i = 0;i < passiveItemSlots.Count; i++)
                    {
                        if (passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenPassiveItemUpgrade.passiveItemData)
                        {
                            newPassiveItem = false;

                            if (!newPassiveItem)
                            {
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i)); // Le aplica la funcionalidad al botón.

                                // Descripción y nombre del siguiente nivel de tesoro / pasivo.
                                upgradeOption.upgradeDescriptionDisplay.text
                                    = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description;
                                upgradeOption.upgradeNameDisplay.text
                                    = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newPassiveItem = true;
                        }
                    }

                    if (newPassiveItem)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosenPassiveItemUpgrade.initialPassiveItem)); // Le aplica la funcionalidad al botón.

                        // Descripción y nombre iniciales del tesoro / pasivo.
                        upgradeOption.upgradeDescriptionDisplay.text =
                            chosenPassiveItemUpgrade.passiveItemData.Description;
                        upgradeOption.upgradeNameDisplay.text =
                            chosenPassiveItemUpgrade.passiveItemData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.Icon;
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }
}
