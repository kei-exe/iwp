using UnityEngine;
using TMPro; // If you use TextMeshPro

public class IdleVoicelineManager : MonoBehaviour
{
    [Header("Idle Settings")]
    public float idleTimeThreshold = 120f; // 2 minutes
    private float idleTimer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] idleClips;
    [TextArea]
    public string[] subtitles; // Match order to idleClips

    [Header("UI")]
    public TMP_Text subtitleText; // Assign your subtitle UI element
    public float subtitleDuration = 4f;

    private void Start()
    {
        idleTimer = 0f;
        if (subtitleText != null) subtitleText.text = "";
    }

    private void Update()
    {
        // Detect player input (keyboard or mouse)
        if (Input.anyKey || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            idleTimer = 0f;
        }
        else
        {
            idleTimer += Time.deltaTime;
        }

        // If idle long enough, trigger a voiceline
        if (idleTimer >= idleTimeThreshold && !audioSource.isPlaying)
        {
            PlayRandomVoiceline();
            idleTimer = 0f; // Reset so it can happen again after another idle period
        }
    }

    void PlayRandomVoiceline()
    {
        if (idleClips.Length == 0) return;

        int index = Random.Range(0, idleClips.Length);
        audioSource.clip = idleClips[index];
        audioSource.Play();

        if (subtitleText != null)
        {
            subtitleText.text = subtitles[index];
            CancelInvoke(nameof(ClearSubtitle));
            Invoke(nameof(ClearSubtitle), subtitleDuration);
        }
    }

    void ClearSubtitle()
    {
        subtitleText.text = "";
    }
}