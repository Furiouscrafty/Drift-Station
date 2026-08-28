using UnityEngine;
using UnityEngine.UI;

public class SpaceshipHandler : MonoBehaviour
{
    [Header("Upgrades")]
    public ShipUpgradeData spaceship;
    public ShipResourceData resources;
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
        resources.CurrentOrbits = 0;
        PlayerUI.SetActive(true);
        DeathUI.SetActive(false);
        Usedl = false;
        resources.Power = 100;
        resources.ShieldHealth = 100;
        resources.Heat = 0;
        resources.HullHealth = 100;
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
            resources.Power = (float)(resources.Power * 1.06);
        }
        else if (spaceship.BattLevel == 2)
        {
            resources.Power = (float)(resources.Power * 1.08);
        }
        else if (spaceship.BattLevel == 3)
        {
            resources.Power = (float)(resources.Power * 1.1);
        }
        else
            resources.Power = 100;


        //Shields Level Upgrades
        if (spaceship.ShieldLevel == 1)
        {
            resources.ShieldHealth = (float)(resources.ShieldHealth * 1.1);
        }
        else if (spaceship.ShieldLevel == 2)
        {
            resources.ShieldHealth = (float)(resources.ShieldHealth * 1.15);
        }
        else if (spaceship.ShieldLevel == 3)
        {
            resources.ShieldHealth = (float)(resources.ShieldHealth * 1.2);
        }
        else
            resources.ShieldHealth = 100;

        backupPower = (float)(resources.Power * 0.25);
        DefaultShield = resources.ShieldHealth;
        DefaultPower = resources.Power;

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
        if (resources.Power > DefaultPower)
            resources.Power = DefaultPower;
        if (resources.ShieldHealth > DefaultShield) resources.ShieldHealth = DefaultShield;
        if (resources.Heat > 100) resources.Heat = 100;

        //==========================================
        if (resources.Power <= 0)
        {
            if (spaceship.BackupBat && !Usedl)
            {
                Usedl = true;
                resources.Power = backupPower;
            }
            else
            {
                PlayerUI.SetActive(false);
                DeathUI.SetActive(true);
                isPaused = !isPaused;
                Time.timeScale = isPaused ? 0f : 1f;
            }

        }
        if (resources.Heat == 100)
        {
            PlayerUI.SetActive(false);
            DeathUI.SetActive(true);
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
        }

        if (resources.ShieldHealth == 0)
            Shields.SetActive(false);
        else Shields.SetActive(true);

        UpdateStatSliders();


    }

    private void UpdateStatSliders()
    {
        PowerSlider.SetValueWithoutNotify(resources.Power);
        ShieldHealthSlider.SetValueWithoutNotify(resources.ShieldHealth);
        HeatSlider.SetValueWithoutNotify(resources.Heat);
        HullHealthSlider.SetValueWithoutNotify(resources.HullHealth);
    }




    private void CoolHeat()
    {
        if (!resources.ishit)
        {
            resources.Heat = Mathf.Max(0f, resources.Heat - heatCooldownRate * Time.deltaTime);
        }
        // if IsHit is true, heat is left untouched — no cooling happens
    }


    //Get/Set methods

    private void RemovePower(float Value)
    {
        resources.Power = Mathf.Max(0f, resources.Power - Value);
    }

    private void AddPower(float Value)
    {
        if (spaceship.SolPanLevel == 1)
            Value = (float)(Value * 1.06);
        else if (spaceship.SolPanLevel == 2)
            Value = (float)(Value * 1.08);
        if (spaceship.UpgradeSol)
            Value = (float)(Value * 2);

        resources.Power = Mathf.Min(DefaultPower, resources.Power + Value);
    }

    private void RemoveShieldHealth(float Value)
    {
        resources.ShieldHealth = Mathf.Max(0f, resources.ShieldHealth - Value);
    }

    public void ResetShieldHealth()
    {
        resources.ShieldHealth = DefaultShield;
    }

    private void AddHeat(float Value)
    {
        resources.Heat = Mathf.Min(100f, resources.Heat + Value); // pick your real heat cap
    }

    private void RemoveHeat(float Value)
    {
        resources.Heat = Mathf.Max(0f, resources.Heat - Value);
    }

    private void RemoveHullHealth(float Value)
    {
        resources.HullHealth = Mathf.Max(0f, resources.HullHealth - Value);
    }
    private void ResetHullHealth()
    {
        resources.HullHealth = 100;
    }
}