using UnityEngine;
using TMPro;

public class IdleVoicelineManager : MonoBehaviour
{
    [Header("Idle Settings")]
    public float idleTimeThreshold = 120f;
    private float idleTimer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] idleClips;
    [TextArea]
    public string[] subtitles;

    [Header("UI")]
    public TMP_Text subtitleText;
    public float subtitleDuration = 4f;

    private void Start()
    {
        idleTimer = 0f;
        if (subtitleText != null) subtitleText.text = "";
    }

    private void Update()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTimeThreshold && !audioSource.isPlaying)
        {
            PlayRandomVoiceline();
            idleTimer = 0f;
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