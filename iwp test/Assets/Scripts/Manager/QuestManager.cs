using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private string questName = "";
    [SerializeField] private string description = "";
    public QuestState state = QuestState.NotStarted;

    public enum QuestState { NotStarted, InProgress, Completed }

    public UIManager uiManager;

    [SerializeField] private GameObject trigger;
    [SerializeField] private GameObject trigger2;

    [Header("Anim")]
    public PlayerController playerController;
    public Transform playerTransform;

    public Transform spiderTransform;
    public EnemyController enemyController;
    public Animator spiderAnimator;
    public GameObject fireEffectPrefab;

    public Transform bedroomTransform;

    public float rotateDuration = 1f;

    private void Start()
    {
        StartQuest();
    }

    public void StartQuest()
    {
        if (state == QuestState.NotStarted)
        {
            state = QuestState.InProgress;
            uiManager.UpdateQuest(questName, description);
        }
    }
    public void CompleteQuest()
    {
        if (state == QuestState.InProgress)
        {
            state = QuestState.Completed;
            uiManager.UpdateQuest(questName, "Completed! Nice job. Now go back to sleep...");

            if (trigger != null)
                trigger.SetActive(true);

            if (trigger2 != null)
                trigger2.SetActive(false);

            if (SceneManager.GetActiveScene().name == "L2")
                StartCoroutine(QuestCompleteSequence());
        }
    }

    private IEnumerator QuestCompleteSequence()
    {
        float takeDamageDuration = GetAnimationClipLength(spiderAnimator, "TakeDamage");
        float deathAnimDuration = GetAnimationClipLength(spiderAnimator, "Death");

        playerController.EnableControl(false);
        if (enemyController != null)
        {
            enemyController.enabled = false;
        }

        yield return RotatePlayerTowards(spiderTransform.position, rotateDuration);

        if (spiderAnimator != null)
        {
            spiderAnimator.SetTrigger("TakeDamage");
            yield return new WaitForSeconds(takeDamageDuration);
        }

        if (fireEffectPrefab != null && spiderTransform != null)
        {
            GameObject fireEffect = Instantiate(fireEffectPrefab, spiderTransform.position, Quaternion.identity, spiderTransform);
        }

        if (spiderAnimator != null)
        {
            spiderAnimator.SetTrigger("Death");
            yield return new WaitForSeconds(deathAnimDuration);
        }

        yield return RotatePlayerTowards(bedroomTransform.position, rotateDuration);

        playerController.EnableControl(true);
    }

    private IEnumerator RotatePlayerTowards(Vector3 targetPosition, float duration)
    {
        Transform cameraTransform = playerController.cameraTransform;

        // rotate
        Quaternion startPlayerRot = playerTransform.rotation;
        Quaternion startCameraRot = cameraTransform.localRotation;

        // direction from camera pos to spider
        Vector3 direction = targetPosition - cameraTransform.position;
        if (direction == Vector3.zero)
            yield break;

        // rotation for player
        Vector3 flatDirection = direction;
        flatDirection.y = 0;
        if (flatDirection == Vector3.zero)
            yield break;

        Quaternion targetPlayerRot = Quaternion.LookRotation(flatDirection);

        // camera pitch
        Vector3 directionNormalized = direction.normalized;
        float targetPitch = -Mathf.Asin(directionNormalized.y) * Mathf.Rad2Deg;

        Quaternion targetCameraRot = Quaternion.Euler(targetPitch, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            playerTransform.rotation = Quaternion.Slerp(startPlayerRot, targetPlayerRot, t);
            cameraTransform.localRotation = Quaternion.Slerp(startCameraRot, targetCameraRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerTransform.rotation = targetPlayerRot;
        cameraTransform.localRotation = targetCameraRot;
    }

    float GetAnimationClipLength(Animator animator, string clipName)
    {
        if (animator == null) return 0f;

        RuntimeAnimatorController rac = animator.runtimeAnimatorController;
        foreach (var clip in rac.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 0f;
    }

}
