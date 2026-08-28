using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMissions", menuName = "Game/Missions")]
public class Missions : ScriptableObject
{
    public enum MissionType
    {
        None,
        SatelliteCommunications,
        MeteorShower,
        RocketResupply
    }

    [System.Serializable]
    public struct MissionEntry
    {
        public MissionType mission;

        [Tooltip("1-100 progress bar fill. Used by Satellite Communications and Rocket Resupply.")]
        [Range(1, 100)] public float progressBar;

        [Tooltip("For MeteorShower: total shower duration in seconds. For None: cooldown (in seconds) before the next mission can spawn — set this yourself.")]
        public float timer;

        public int cashPerMission;
        public AudioClip IntroClip;

        [Tooltip("Played when this mission is completed successfully. On the None entry, this doubles as the one-off audio played when the game loads.")]
        public AudioClip successAudio;

        [Tooltip("Played when this mission fails. Leave empty on the None entry — it has no failure state.")]
        public AudioClip failureAudio;
    }

    [Header("Missions")]
    public List<MissionEntry> missions = new List<MissionEntry>();

    [Header("Current Mission")]
    [Tooltip("Index into the missions list. Should start pointing at the None entry.")]
    public int currentMissionIndex = -1;

    private void OnValidate()
    {
        for (int i = 0; i < missions.Count; i++)
        {
            MissionEntry entry = missions[i];
            entry.progressBar = Mathf.Clamp(entry.progressBar, 1f, 100f);
            entry.cashPerMission = Mathf.Max(entry.cashPerMission, 0);
            missions[i] = entry;
        }
    }
}