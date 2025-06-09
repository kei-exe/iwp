using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public float timeToLosePlayer = 3f;

    public float attackDistance = 1.5f;
    public float spiderAttackDelay = 1.0f;

    private Transform player;
    private int currentPoint = 0;
    private float loseTimer = 0f;
    private bool isAttacking = false;

    [SerializeField] private Animator animator;

    private enum State { Patrol, Chase }
    private State currentState = State.Patrol;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isAttacking) return;

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                CheckForPlayer();
                break;

            case State.Chase:
                Chase();
                CheckLosePlayer();
                break;
        }

        // Debug view cone lines
        Debug.DrawRay(transform.position + Vector3.up, Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward * viewDistance, Color.yellow);
        Debug.DrawRay(transform.position + Vector3.up, Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward * viewDistance, Color.yellow);
    }

    void Patrol()
    {
        Transform target = patrolPoints[currentPoint];
        MoveTowards(target.position, patrolSpeed);

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }

        animator.SetBool("IsWalking", true);
        animator.SetBool("IsChasing", false);
    }

    void Chase()
    {
        if (Vector3.Distance(transform.position, player.position) > 1.5f)
        {
            MoveTowards(player.position, chaseSpeed);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        animator.SetBool("IsChasing", true);

        if (gameObject.CompareTag("Spider"))
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsChasing", false);
        }
    }

    IEnumerator AttackPlayer(PlayerController pc)
    {
        if (pc == null) yield break;

        isAttacking = true;

        // Stop movement and play animation
        animator.SetTrigger("IsAttacking");

        // Freeze movement only
        pc.rb.linearVelocity = Vector3.zero;
        pc.rb.isKinematic = true;

        animator.SetTrigger("IsAttacking");

        yield return new WaitForSeconds(1f); // match attack animation length

        pc.TakeDamage(1);

        // Unfreeze movement
        pc.rb.isKinematic = false;

        isAttacking = false;
    }

    void CheckForPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (directionToPlayer.magnitude <= viewDistance && angle <= viewAngle / 2f)
        {
            if (HasLineOfSight())
            {
                currentState = State.Chase;
                loseTimer = 0f;
            }
        }
    }

    void CheckLosePlayer()
    {
        if (HasLineOfSight())
        {
            loseTimer = 0f;
        }
        else
        {
            loseTimer += Time.deltaTime;
            if (loseTimer >= timeToLosePlayer)
            {
                currentState = State.Patrol;
            }
        }
    }

    bool HasLineOfSight()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Ray ray = new Ray(transform.position + Vector3.up, direction);

        Debug.DrawRay(ray.origin, ray.direction * viewDistance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, viewDistance))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isAttacking)
        {
            StartCoroutine(AttackPlayer(other.GetComponent<PlayerController>()));
        }
    }
}