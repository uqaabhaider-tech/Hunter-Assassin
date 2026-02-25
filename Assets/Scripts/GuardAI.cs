using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float wanderRadius = 10f;
    public float wanderTimer = 4f;
    public float chaseSpeed = 6f;      // Faster speed for chasing
    public float patrolSpeed = 2.5f;   // Slower speed for patrolling

    [Header("Detection Settings")]
    public float detectionRange = 12f;
    public float viewAngle = 90f;
    public float attackRate = 1.0f;
    public float chaseTime = 4.0f;     // Seconds to follow after losing sight
    public LayerMask obstructionMask;

    [Header("References")]
    public EnemyGun gunScript;
    public Animator enemyAnim;

    private NavMeshAgent agent;
    private float wanderCounter;
    private float chaseCounter;
    private bool isChasing = false;
    private AssassinController playerScript;
    private float nextAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        wanderCounter = wanderTimer;
        playerScript = GameObject.FindFirstObjectByType<AssassinController>();

        // Initial setup
        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0f;
    }

    void Update()
    {
        if (playerScript == null || !playerScript.gameObject.activeInHierarchy) return;

        // Sync Animator Speed
        if (enemyAnim != null)
            enemyAnim.SetFloat("Speed", agent.velocity.magnitude);

        float distance = Vector3.Distance(transform.position, playerScript.transform.position);
        Vector3 dirToPlayer = (playerScript.transform.position - transform.position).normalized;

        bool canSeePlayer = false;

        // Check Line of Sight
        if (distance < detectionRange)
        {
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            if (angle < viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distance, obstructionMask))
                {
                    canSeePlayer = true;
                }
            }
        }

        // Aggressive State Machine
        if (canSeePlayer)
        {
            isChasing = true;
            chaseCounter = chaseTime;
            PerformAttackChase();
        }
        else if (isChasing)
        {
            KeepChasing(); // Follow to last known position
        }
        else
        {
            Wander();
        }
    }

    void PerformAttackChase()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = 3f; // Keep this small so he follows closely
        agent.SetDestination(playerScript.transform.position);

        // Face the player
        Vector3 lookDir = playerScript.transform.position - transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 15f);

        // Attack logic
        if (Time.time >= nextAttackTime)
        {
            if (enemyAnim != null) enemyAnim.SetTrigger("shoot");
            if (gunScript != null) gunScript.Shoot(playerScript);
            nextAttackTime = Time.time + attackRate;
        }
    }

    void KeepChasing()
    {
        agent.speed = chaseSpeed;
        agent.stoppingDistance = 0f; // Walk exactly to the spot they vanished
        agent.SetDestination(playerScript.transform.position);

        chaseCounter -= Time.deltaTime;
        if (chaseCounter <= 0)
        {
            isChasing = false;
            agent.speed = patrolSpeed;
        }
    }

    void Wander()
    {
        agent.isStopped = false;
        agent.stoppingDistance = 0f;
        agent.speed = patrolSpeed;

        wanderCounter += Time.deltaTime;
        if (wanderCounter >= wanderTimer)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius + transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
                agent.SetDestination(hit.position);

            wanderCounter = 0;
        }
    }

    public void Vanish() { Destroy(gameObject); }
}