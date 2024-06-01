using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    CharacterScriptableObject characterData;

    // Estadísticas actuales
    
    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    float currentMagnet;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        { 
            // Comprueba si el valor ha cambiado.
            if (currentHealth != value)
            {
                currentHealth = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = "Vitalidad: " + currentHealth;
                }

            }
        }
    }

    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            if (currentRecovery != value)
            {
                currentRecovery = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Regeneraci�n: " + currentRecovery;
                }
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Movimiento: " + currentMoveSpeed;
                }
            }
        }
    }

    public float CurrentMight
    {
        get { return currentMight; }
        set
        {
            if (currentMight != value)
            {
                currentMight = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Poder: " + currentMight;
                }
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            if (currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Vel. Proyectil: " + currentProjectileSpeed;
                }

            }
        }
    }

    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            if (currentMagnet != value)
            {
                currentMagnet = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnetismo: " + currentMagnet;
                }

            }
        }
    }
    #endregion

    public ParticleSystem damageEffect;

    // Experiencia y nivel

    [Header("Experience/level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    // Clase para definir un rango de niveles y su experiencia correspondiente necesaria.
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    // I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    public List<LevelRange> levelRanges;

    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TMP_Text levelText;

    void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();

        CurrentHealth = characterData.MaxHealth;
        CurrentRecovery = characterData.Recovery;
        CurrentMoveSpeed = characterData.MoveSpeed;
        CurrentMight = characterData.Might;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentMagnet = characterData.Magnet;

        SpawnWeapon(characterData.StartingWeapon);
    }


    private void Start()
    {
        // Inicializa el primer incremento del tope de experiencia inicial.
        experienceCap = levelRanges[0].experienceCapIncrease;

        // Display de stats inicial.
        GameManager.instance.currentHealthDisplay.text = "Vitalidad: " + currentHealth;
        GameManager.instance.currentRecoveryDisplay.text = "Regeneraci�n: " + currentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "Movimiento: " + currentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Poder: " + CurrentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Vel. Proyectil: " + currentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnetismo: " + CurrentMagnet;
    
        GameManager.instance.AssignChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }

        Recover();
    }

    public void IncreasedExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
        UpdateExpBar();

    }

    void LevelUpChecker()
    {
        if (experience >= experienceCap) 
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                }
            }
            experienceCap += experienceCapIncrease;

            UpdateLevelText();

            GameManager.instance.StartLevelUp();
        }
    }

    void UpdateExpBar()
    {
        expBar.fillAmount = (float) experience / experienceCap;
    }

    void UpdateLevelText()
    {
        levelText.text = level.ToString();
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            CurrentHealth -= dmg;

            if (damageEffect)
            {
                Instantiate(damageEffect, transform.position, Quaternion.identity);
            }

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if (CurrentHealth <= 0)
            {
                Kill();
            }

            UpdateHealthBar();
        }
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth / characterData.MaxHealth;
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReached(level);
            GameManager.instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponUISlots, inventory.passiveItemUISlots);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += amount;

            if (CurrentHealth > characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
        }

        UpdateHealthBar();
    }

    void Recover()
    {
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
            if (CurrentHealth > characterData.MaxHealth) 
            {
                CurrentHealth = characterData.MaxHealth;
            }
        }

        UpdateHealthBar();
    }

    public void SpawnWeapon(GameObject weapon)
    {
        if (weaponIndex >= inventory.weaponSlots.Count - 1)
        {
            Debug.LogError("Slots de inventario llenos.");
            return;
        }

        // Genera el arma inicial
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); // Agrega el arma a su slot de inventario.

        weaponIndex++;
    
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogError("Slots de inventario llenos.");
            return;
        }

        // Genera el tesoro/objeto pasivo inicial
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>()); // Agrega el objeto pasivo a su slot de inventario.

        passiveItemIndex++;

    }
}
