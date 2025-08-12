using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
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
    private NavMeshAgent agent;

    private enum State { Patrol, Chase }
    private State currentState = State.Patrol;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        agent.autoBraking = true;
        agent.stoppingDistance = 0.1f;
        GoToNextPatrolPoint();
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

        Debug.DrawRay(transform.position + Vector3.up, Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward * viewDistance, Color.yellow);
        Debug.DrawRay(transform.position + Vector3.up, Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward * viewDistance, Color.yellow);
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
            GoToNextPatrolPoint();
        }

        animator.SetBool("IsWalking", true);
        animator.SetBool("IsChasing", false);
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.speed = patrolSpeed;
        agent.SetDestination(patrolPoints[currentPoint].position);
    }

    void Chase()
    {
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsChasing", true);

        if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath();
            animator.SetBool("IsWalking", false);
        }

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
        agent.ResetPath();
        animator.SetTrigger("IsAttacking");

        pc.rb.linearVelocity = Vector3.zero;
        pc.rb.isKinematic = true;

        yield return new WaitForSeconds(spiderAttackDelay);

        pc.TakeDamage(1);
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isAttacking)
        {
            StartCoroutine(AttackPlayer(other.GetComponent<PlayerController>()));
        }
    }
}