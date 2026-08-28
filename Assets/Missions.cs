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

        [Range(1, 100)] public float progressBar;

        public float timer;

        public int cashPerMission;

        public AudioClip IntroClip;
        public AudioClip successAudio;
        public AudioClip failureAudio;

        [Tooltip("Optional: shown as on-screen text if the player has audio muted, in place of each clip above. Leave blank to fall back to a generic caption.")]
        public string introCaption;
        public string successCaption;
        public string failureCaption;
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