using System.Collections;
using TMPro;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private string[] messages;

    [SerializeField] private float switchInterval = 5f;

    private int currentIndex = 0;

    private void Start()
    {
        if (messages.Length == 0 || tmpText == null)
        {
            Debug.LogWarning("TMPTextSwitcher: No messages or TMP_Text assigned.");
            return;
        }

        tmpText.text = messages[currentIndex];
        StartCoroutine(SwitchTextRoutine());
    }

    private IEnumerator SwitchTextRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchInterval);
            currentIndex = (currentIndex + 1) % messages.Length;
            tmpText.text = messages[currentIndex];
        }
    }
}
