using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System;

public class Timeline : MonoBehaviour
{
    [Header("Setup")]
    public PlayableDirector playableDirector;
    public bool autoPlayOnStart = true;

    [Header("On Timeline End")]
    public bool loadSceneOnEnd = false;
    public string sceneToLoad;

    public UnityEngine.Events.UnityEvent onTimelineEnd; // player renable

    void Start()
    {
        if (playableDirector == null)
            playableDirector = GetComponent<PlayableDirector>();

        if (playableDirector != null)
        {
            playableDirector.stopped += OnTimelineStopped;

            if (autoPlayOnStart)
                playableDirector.Play();
        }
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        if (loadSceneOnEnd && !string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }

        onTimelineEnd?.Invoke(); // You can hook any custom logic to this via Inspector
    }

    public void PlayTimelineManually()
    {
        if (playableDirector != null)
            playableDirector.Play();
    }
}
