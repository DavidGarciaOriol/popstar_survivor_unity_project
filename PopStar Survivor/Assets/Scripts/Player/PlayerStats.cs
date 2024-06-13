using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    float health;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return health; }
        set
        { 
            // Comprueba si el valor ha cambiado.
            if (health != value)
            {
                health = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = string.Format(
                        "Salud: {0} / {1}",
                        health, actualStats.maxHealth
                    );
                }
            }
        }
    }

    public float MaxHealth
    {
        get { return actualStats.maxHealth; }
        set
        {
            if (actualStats.maxHealth != value)
            {
                actualStats.maxHealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = string.Format(
                        "Salud: {0} / {1}",
                        health, actualStats.maxHealth
                    );
                }
            }
        }
    }

    public float CurrentRecovery
    {
        get { return Recovery; }
        set { CurrentRecovery = value; }
    }

    public float Recovery { 
        get { return actualStats.recovery; }
        set
        {
            if (actualStats.recovery != value)
            {
                actualStats.recovery = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Regeneración: " + actualStats.recovery;
                }
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set { MoveSpeed = value; }
    }

    public float MoveSpeed
    {
        get { return actualStats.moveSpeed; }
        set
        {
            if (actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Movimiento: " + actualStats.moveSpeed;
                }
            }
        }
    }

    public float CurrentMight
    {
        get { return Might; }
        set { CurrentMight = value; }
    }

    public float Might
    {
        get { return actualStats.might; }
        set {
            if (actualStats.might != value)
            {
                actualStats.might = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Poder: " + actualStats.might;
                }
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return Speed; }
        set { MoveSpeed = value; }
    }

    public float Speed
    {
        get { return actualStats.speed; }
        set
        {
            if (actualStats.speed != value)
            {
                actualStats.speed = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Vel. Proyectil: " + actualStats.speed;
                }

            }
        }
    }

    public float CurrentMagnet
    {
        get { return Magnet; }
        set { CurrentMagnet = value; }
    }

    public float Magnet
    {
        get { return actualStats.magnet; }
        set
        {
            if (actualStats.magnet != value)
            {
                actualStats.magnet = value;

                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnetismo: " + actualStats.magnet;
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

    PlayerInventory inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TMP_Text levelText;

    void Awake()
    {
        characterData = CharacterSelector.GetData();

        if (CharacterSelector.instance)
        {
            CharacterSelector.instance.DestroySingleton();
        }
        

        inventory = GetComponent<PlayerInventory>();

        baseStats = actualStats = characterData.stats;
        health = actualStats.maxHealth;
    }


    private void Start()
    {
        // Genera la habilidad inicial
        inventory.Add(characterData.StartingWeapon);

        // Inicializa el primer incremento del tope de experiencia inicial.
        experienceCap = levelRanges[0].experienceCapIncrease;

        // Display de stats inicial.
        GameManager.instance.currentHealthDisplay.text = "Vitalidad: " + CurrentHealth;
        GameManager.instance.currentRecoveryDisplay.text = "Regeneración: " + CurrentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "Movimiento: " + CurrentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Poder: " + CurrentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Vel. Proyectil: " + CurrentProjectileSpeed;
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

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;

            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
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
                Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);
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
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponSlots, inventory.passiveSlots);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;

            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }

        UpdateHealthBar();
    }

    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
            CurrentHealth += Recovery * Time.deltaTime;

            if (CurrentHealth > actualStats.maxHealth) 
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }

        UpdateHealthBar();
    }

    [System.Obsolete("Función mantenida por temas de compatibilidad. Será eliminada pronto")]
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
        //inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); // Agrega el arma a su slot de inventario.

        weaponIndex++;
    }

    [System.Obsolete("Ya no hace falta generar los objetos pasivos directamente.")]
    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveItemIndex >= inventory.passiveSlots.Count - 1)
        {
            Debug.LogError("Slots de inventario llenos.");
            return;
        }

        // Genera el tesoro/objeto pasivo inicial
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);
       // inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>()); // Agrega el objeto pasivo a su slot de inventario.

        passiveItemIndex++;

    }
}
