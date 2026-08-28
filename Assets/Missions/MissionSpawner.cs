using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using static Missions;

public class MissionSpawner : MonoBehaviour
{
    [Header("Data")]
    public Missions missionsData;
    public ShipResourceData shipResourceData;
    public ShipUpgradeData shipData;

    [Header("Spawn Area")]
    public Transform planet;
    public float spawnRadius = 20f;
    public Vector2 spawnHeightRange = new Vector2(-2f, 2f);

    [Header("Mission Prefabs")]
    public GameObject satellitePrefab;
    public GameObject rocketPrefab;
    public GameObject[] meteorPrefabs;

    [Header("Meteor Shower Settings")]
    public float meteorSpawnInterval = 2f;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Captions")]
    [Tooltip("Shown when audioSource is muted or global volume is 0, in place of playing a clip.")]
    public TMP_Text captionText;
    public float captionDuration = 3f;
    private float captionTimer;

    [Header("Tutorial")]
    public AudioClip tutorialClip;
    public string tutorialCaption;

    [Header("UI")]
    [Tooltip("Reflects the active mission's progressBar (1-100) while a mission is running. Hidden/ignored during None.")]
    public Slider missionProgressSlider;

    private int noneIndex = -1;
    private bool hasCompletedFirstOrbit;
    private int lastKnownOrbitCount;
    private bool noneTimerRunning;
    private float noneTimer;

    private bool missionTimerRunning;
    private float missionTimer;
    private float meteorSpawnCountdown;

    private readonly List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        if (missionsData == null || shipResourceData == null)
        {
            Debug.LogWarning("MissionSpawner: Missing Missions data or ShipResourceData.");
            return;
        }

        noneIndex = FindIndexOfType(MissionType.None);
        if (noneIndex == -1)
        {
            Debug.LogWarning("MissionSpawner: No entry with MissionType.None found.");
            return;
        }

        missionsData.currentMissionIndex = noneIndex;

        if (missionProgressSlider != null)
        {
            missionProgressSlider.minValue = 1f;
            missionProgressSlider.maxValue = 100f;
            missionProgressSlider.gameObject.SetActive(false); // nothing to show during None
        }

        // Startup audio
        PlayClip(missionsData.missions[noneIndex].successAudio, missionsData.missions[noneIndex].successCaption);

        // Tutorial audio
        PlayClip(tutorialClip, tutorialCaption);

        lastKnownOrbitCount = shipResourceData.CurrentOrbits;
        hasCompletedFirstOrbit = lastKnownOrbitCount >= 1;

        if (hasCompletedFirstOrbit)
            BeginNoneCountdown();
    }

    private void Update()
    {
        if (missionsData == null || shipResourceData == null || noneIndex == -1)
            return;

        if (shipResourceData.CurrentOrbits > lastKnownOrbitCount)
        {
            lastKnownOrbitCount = shipResourceData.CurrentOrbits;

            if (!hasCompletedFirstOrbit)
            {
                hasCompletedFirstOrbit = true;
                if (missionsData.currentMissionIndex == noneIndex)
                    BeginNoneCountdown();
            }
        }

        if (noneTimerRunning)
        {
            noneTimer -= Time.deltaTime;
            if (noneTimer <= 0f)
            {
                noneTimerRunning = false;
                SelectAndSpawnMission();
            }
        }

        if (missionTimerRunning)
            UpdateActiveMission();

        UpdateProgressSlider();
        UpdateCaptionTimer();
    }

    private void BeginNoneCountdown()
    {
        if (!hasCompletedFirstOrbit)
            return;

        noneTimer = missionsData.missions[noneIndex].timer;
        noneTimerRunning = true;
    }

    private void SelectAndSpawnMission()
    {
        // ALWAYS pick a random mission between 1 and 3
        int chosenIndex = Random.Range(1, 4); // 1–3 inclusive

        missionsData.currentMissionIndex = chosenIndex;

        missionTimer = missionsData.missions[chosenIndex].timer;
        missionTimerRunning = true;
        meteorSpawnCountdown = 0f;

        if (missionProgressSlider != null)
        {
            missionProgressSlider.gameObject.SetActive(true);
            missionProgressSlider.SetValueWithoutNotify(missionsData.missions[chosenIndex].progressBar);
        }

        // Play mission intro
        PlayClip(missionsData.missions[chosenIndex].IntroClip, missionsData.missions[chosenIndex].introCaption);

        SpawnMissionItem(missionsData.missions[chosenIndex].mission);
    }

    private void SpawnMissionItem(MissionType type)
    {
        ClearSpawnedObjects();

        switch (type)
        {
            case MissionType.SatelliteCommunications:
                SpawnSingle(satellitePrefab);
                break;

            case MissionType.RocketResupply:
                SpawnSingle(rocketPrefab);
                break;

            case MissionType.MeteorShower:
                break;
        }
    }

    private void UpdateActiveMission()
    {
        MissionEntry active = missionsData.missions[missionsData.currentMissionIndex];

        if (active.mission == MissionType.MeteorShower && shipResourceData.HullHealth <= 0f)
        {
            missionTimerRunning = false;
            CompleteMission(false);
            return;
        }

        if (active.mission == MissionType.MeteorShower)
        {
            meteorSpawnCountdown -= Time.deltaTime;
            if (meteorSpawnCountdown <= 0f)
            {
                SpawnRandomMeteor();
                meteorSpawnCountdown = meteorSpawnInterval;
            }
        }

        missionTimer -= Time.deltaTime;
        if (missionTimer <= 0f)
        {
            missionTimerRunning = false;

            if (active.mission == MissionType.MeteorShower)
            {
                CompleteMission(true);
            }
            else
            {
                bool success = Mathf.Approximately(active.progressBar, 100f);
                CompleteMission(success);
            }
        }
    }

    private void UpdateProgressSlider()
    {
        if (missionProgressSlider == null || missionsData.currentMissionIndex == noneIndex)
            return;

        missionProgressSlider.SetValueWithoutNotify(missionsData.missions[missionsData.currentMissionIndex].progressBar);
    }

    private void SpawnRandomMeteor()
    {
        if (meteorPrefabs == null || meteorPrefabs.Length == 0)
        {
            Debug.LogWarning("MissionSpawner: No meteor prefabs assigned.");
            return;
        }

        GameObject prefab = meteorPrefabs[Random.Range(0, meteorPrefabs.Length)];
        SpawnSingle(prefab);
    }

    private void SpawnSingle(GameObject prefab)
    {
        if (prefab == null || planet == null)
            return;

        Vector3 spawnPos = GetRandomPointInRadius();
        GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity);
        spawnedObjects.Add(instance);
    }

    private Vector3 GetRandomPointInRadius()
    {
        Vector2 circlePoint = Random.insideUnitCircle * spawnRadius;
        float height = Random.Range(spawnHeightRange.x, spawnHeightRange.y);
        return planet.position + new Vector3(circlePoint.x, height, circlePoint.y);
    }

    private void ClearSpawnedObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedObjects.Clear();
    }

    public void CompleteMission(bool success)
    {
        if (noneIndex == -1 || missionsData.currentMissionIndex == noneIndex)
            return;

        MissionEntry completed = missionsData.missions[missionsData.currentMissionIndex];

        if (success)
        {
            if (shipData != null)
                shipData.Money += completed.cashPerMission;
            PlayClip(completed.successAudio, completed.successCaption);
        }
        else
        {
            PlayClip(completed.failureAudio, completed.failureCaption);
        }

        missionTimerRunning = false;
        ClearSpawnedObjects();

        if (missionProgressSlider != null)
            missionProgressSlider.gameObject.SetActive(false);

        // ALWAYS return to None (0)
        missionsData.currentMissionIndex = noneIndex;
        BeginNoneCountdown();
    }

    // Plays the clip normally, or shows a caption instead if audio is muted.
    private void PlayClip(AudioClip clip, string caption)
    {
        bool isMuted = (audioSource != null && audioSource.mute) || AudioListener.volume <= 0f;

        if (isMuted)
        {
            ShowCaption(string.IsNullOrEmpty(caption) ? "(audio muted)" : caption);
            return;
        }

        if (clip == null || audioSource == null)
            return;

        audioSource.PlayOneShot(clip);
    }

    private void ShowCaption(string text)
    {
        if (captionText == null)
            return;

        captionText.text = text;
        captionTimer = captionDuration;
    }

    private void UpdateCaptionTimer()
    {
        if (captionTimer <= 0f || captionText == null)
            return;

        captionTimer -= Time.deltaTime;
        if (captionTimer <= 0f)
            captionText.text = "";
    }

    private int FindIndexOfType(MissionType type)
    {
        for (int i = 0; i < missionsData.missions.Count; i++)
        {
            if (missionsData.missions[i].mission == type)
                return i;
        }
        return -1;
    }
}