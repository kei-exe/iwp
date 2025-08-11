using UnityEngine;
using UnityEngine.Playables;

public class PlayTimelineOnce : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline;

    private static bool hasPlayedThisSession = false;

    private void Start()
    {
        // Check memory first
        if (!hasPlayedThisSession && PlayerPrefs.GetInt("HasPlayedIntro", 0) == 0)
        {
            timeline.Play();
            hasPlayedThisSession = true;
            PlayerPrefs.SetInt("HasPlayedIntro", 1);
            PlayerPrefs.Save();
        }
        else
        {
            timeline.Stop();
        }
    }
}