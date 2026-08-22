using UnityEngine;
using UnityEngine.UI;

public class SpaceshipHandler : MonoBehaviour
{
    [Header("Upgrades")]
    public ShipUpgradeData spaceship;
    public GameObject BackupBattery;
    public GameObject SolUpgrade;
    public GameObject Shields;

    [Header("UI")]
    public Slider HullHealthSlider;
    public Slider PowerSlider;
    public Slider ShieldHealthSlider;
    public Slider HeatSlider;
    public GameObject PlayerUI;
    public GameObject DeathUI;

    [Header("Public Data")]
    public float ShieldRechargePowerDrain;
    public float heatCooldownRate = 5f;


    //Private
    private bool Usedl;
    private float backupPower;
    private float DefaultShield;
    private float DefaultPower;
    private bool isPaused = false;

    void Start()
    {
        PlayerUI.SetActive(true);
        DeathUI.SetActive(false);
        Usedl = false;
        spaceship.Power = 100;
        spaceship.ShieldHealth = 100;
        spaceship.Heat = 0;
        spaceship.HullHealth = 100;
        if (spaceship.BackupBat)
        {
            BackupBattery.SetActive(true);
        }
        else BackupBattery.SetActive(false);

        if (spaceship.UpgradeSol)
        {
            SolUpgrade.SetActive(true);
        }
        else SolUpgrade.SetActive(false);


        //Battery Level Upgrades
        if (spaceship.BattLevel == 1)
        {
            spaceship.Power = (float)(spaceship.Power * 1.06);
        }
        else if (spaceship.BattLevel == 2)
        {
            spaceship.Power = (float)(spaceship.Power * 1.08);
        }
        else if (spaceship.BattLevel == 3)
        {
            spaceship.Power = (float)(spaceship.Power * 1.1);
        }
        else
            spaceship.Power = 100;


        //Shields Level Upgrades
        if (spaceship.ShieldLevel == 1)
        {
            spaceship.ShieldHealth = (float)(spaceship.ShieldHealth * 1.1);
        }
        else if (spaceship.ShieldLevel == 2)
        {
            spaceship.ShieldHealth = (float)(spaceship.ShieldHealth * 1.15);
        }
        else if (spaceship.ShieldLevel == 3)
        {
            spaceship.ShieldHealth = (float)(spaceship.ShieldHealth * 1.2);
        }
        else
            spaceship.ShieldHealth = 100;

        backupPower = (float)(spaceship.Power * 0.25);
        DefaultShield = spaceship.ShieldHealth;
        DefaultPower = spaceship.Power;

        // Sliders scale with each stat's own max, since Power/Shield max varies with upgrades.
        PowerSlider.minValue = 0;
        PowerSlider.maxValue = DefaultPower;
        ShieldHealthSlider.minValue = 0;
        ShieldHealthSlider.maxValue = DefaultShield;
        HeatSlider.minValue = 0;
        HeatSlider.maxValue = 100;
        HullHealthSlider.minValue = 0;
        HullHealthSlider.maxValue = 100;
    }

    // Update is called once per frame
    void Update()
    {
        CoolHeat();
        //Validations
        if (spaceship.Power > DefaultPower)
            spaceship.Power = DefaultPower;
        if (spaceship.ShieldHealth > DefaultShield) spaceship.ShieldHealth = DefaultShield;
        if (spaceship.Heat > 100) spaceship.Heat = 100;

        //==========================================
        if (spaceship.Power <= 0)
        {
            if (spaceship.BackupBat && !Usedl)
            {
                Usedl = true;
                spaceship.Power = backupPower;
            }
            else
            {
                PlayerUI.SetActive(false);
                DeathUI.SetActive(true);
                isPaused = !isPaused;
                Time.timeScale = isPaused ? 0f : 1f;
            }

        }
        if (spaceship.Heat == 100)
        {
            PlayerUI.SetActive(false);
            DeathUI.SetActive(true);
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
        }

        if (spaceship.ShieldHealth == 0)
            Shields.SetActive(false);
        else Shields.SetActive(true);

        UpdateStatSliders();


    }

    private void UpdateStatSliders()
    {
        PowerSlider.SetValueWithoutNotify(spaceship.Power);
        ShieldHealthSlider.SetValueWithoutNotify(spaceship.ShieldHealth);
        HeatSlider.SetValueWithoutNotify(spaceship.Heat);
        HullHealthSlider.SetValueWithoutNotify(spaceship.HullHealth);
    }




    private void CoolHeat()
    {
        if (!spaceship.ishit)
        {
            spaceship.Heat = Mathf.Max(0f, spaceship.Heat - heatCooldownRate * Time.deltaTime);
        }
        // if IsHit is true, heat is left untouched — no cooling happens
    }


    //Get/Set methods

    private void RemovePower(float Value)
    {
        spaceship.Power = Mathf.Max(0f, spaceship.Power - Value);
    }

    private void AddPower(float Value)
    {
        if (spaceship.SolPanLevel == 1)
            Value = (float)(Value * 1.06);
        else if (spaceship.SolPanLevel == 2)
            Value = (float)(Value * 1.08);
        if (spaceship.UpgradeSol)
            Value = (float)(Value * 2);

        spaceship.Power = Mathf.Min(DefaultPower, spaceship.Power + Value);
    }

    private void RemoveShieldHealth(float Value)
    {
        spaceship.ShieldHealth = Mathf.Max(0f, spaceship.ShieldHealth - Value);
    }

    public void ResetShieldHealth()
    {
        spaceship.ShieldHealth = DefaultShield;
    }

    private void AddHeat(float Value)
    {
        spaceship.Heat = Mathf.Min(100f, spaceship.Heat + Value); // pick your real heat cap
    }

    private void RemoveHeat(float Value)
    {
        spaceship.Heat = Mathf.Max(0f, spaceship.Heat - Value);
    }

    private void RemoveHullHealth(float Value)
    {
        spaceship.HullHealth = Mathf.Max(0f, spaceship.HullHealth - Value);
    }
    private void ResetHullHealth()
    {
        spaceship.HullHealth = 100;
    }
}