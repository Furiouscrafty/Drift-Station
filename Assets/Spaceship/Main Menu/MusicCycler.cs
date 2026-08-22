using UnityEngine;
public class MusicCycler : MonoBehaviour
{
    [Header("Assign your AudioSource")]
    public AudioSource audioSource;
    [Header("Add music tracks here")]
    public AudioClip[] musicTracks;
    private int currentIndex = 0;
    private void Start()
    {
        if (audioSource == null || musicTracks.Length == 0)
        {
            Debug.LogWarning("MusicCycler: Missing AudioSource or music tracks.");
            return;
        }
        currentIndex = Random.Range(0, musicTracks.Length);
        PlayTrack(currentIndex);
    }
    private void Update()
    {
        // If the track finished, move to the next one
        if (!audioSource.isPlaying)
        {
            NextTrack();
        }
    }
    private void PlayTrack(int index)
    {
        audioSource.clip = musicTracks[index];
        audioSource.Play();
    }
    private void NextTrack()
    {
        if (musicTracks.Length == 1)
        {
            PlayTrack(0);
            return;
        }

        int newIndex;
        do
        {
            newIndex = Random.Range(0, musicTracks.Length);
        } while (newIndex == currentIndex); // avoid repeating the same track twice in a row

        currentIndex = newIndex;
        PlayTrack(currentIndex);
    }
}