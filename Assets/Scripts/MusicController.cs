using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    [System.Serializable]
    public class TrackInfo
    {
        [Tooltip("Audio clip for the track.")]
        public AudioClip track;

        [Tooltip("Title of the track.")]
        public string title;

        [Tooltip("Artist of the track.")]
        public string artist;
    }

    [Header("Music Settings")]
    [Tooltip("List of tracks with their titles and artists.")]
    public TrackInfo[] tracks;

    [Range(0.1f, 5f)]
    [Tooltip("Duration for fade in and fade out transitions.")]
    public float fadeDuration = 1f;

    [Header("UI Settings")]
    [Tooltip("Text object to display the current playing track's title and artist.")]
    public TMP_Text trackInfoText;

    [Tooltip("Duration to display the track info.")]
    public float trackInfoDisplayDuration = 5f;

    private AudioSource audioSource;
    private int[] trackOrder;
    private int currentTrackIndex = 0;

    private void Start()
    {
        if (tracks.Length == 0)
        {
            Debug.LogError("No tracks assigned.");
            return;
        }

        audioSource = GetComponent<AudioSource>();
        trackOrder = Enumerable.Range(0, tracks.Length).ToArray();
        ShuffleTrackOrder();

        StartCoroutine(PlayTracksWithDelay(2f));
    }

    private IEnumerator PlayTracksWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayNextTrack();
    }

    private void PlayNextTrack()
    {
        if (currentTrackIndex >= trackOrder.Length)
        {
            ShuffleTrackOrder();
            currentTrackIndex = 0;
        }

        StartCoroutine(PlayTrack(trackOrder[currentTrackIndex]));
        currentTrackIndex++;
    }

    private IEnumerator PlayTrack(int trackIndex)
    {
        TrackInfo currentTrack = tracks[trackIndex];

        // Display track info.
        StartCoroutine(DisplayTrackInfo(currentTrack.title, currentTrack.artist));

        // Play track with fade in.
        audioSource.clip = currentTrack.track;
        audioSource.volume = 0;
        audioSource.Play();

        float startVolume = 0;
        float endVolume = 1;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(currentTrack.track.length - fadeDuration * 2);

        // Fade out.
        elapsedTime = 0;
        startVolume = 1;
        endVolume = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.Stop();

        // Play the next track.
        PlayNextTrack();
    }

    private IEnumerator DisplayTrackInfo(string title, string artist)
    {
        trackInfoText.text = $"{title} - {artist}";
        trackInfoText.canvasRenderer.SetAlpha(0.01f);  // Almost transparent to start fading.
        trackInfoText.CrossFadeAlpha(1, fadeDuration, false);

        yield return new WaitForSeconds(trackInfoDisplayDuration - fadeDuration * 2);

        trackInfoText.CrossFadeAlpha(0, fadeDuration, false);
    }

    private void ShuffleTrackOrder()
    {
        trackOrder = trackOrder.OrderBy(x => Random.value).ToArray();
    }
}
