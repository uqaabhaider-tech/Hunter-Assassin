using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float wanderRadius = 10f;
    public float wanderTimer = 4f;
    public float chaseSpeed = 4f;
    public float patrolSpeed = 2f;

    [Header("Detection Settings")]
    public float detectionRange = 12f;
    public float viewAngle = 120f;
    public float attackRate = 1.5f;
    public float chaseTime = 4.0f;
    public LayerMask obstructionMask;

    [Header("Vision Visuals")]
    public Light visionSpotlight;
    public Color patrolColor = new Color(0.3f, 0, 0);
    public Color alertColor = Color.red;

    [Header("Hunter Assassin FX")]
    public GameObject gemPrefab; // Drag your Diamond prefab here!

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

        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0f;

        if (visionSpotlight != null)
        {
            visionSpotlight.color = patrolColor;
            visionSpotlight.range = detectionRange;
        }
    }

    void Update()
    {
        if (playerScript == null || !playerScript.gameObject.activeInHierarchy) return;

        if (enemyAnim != null)
            enemyAnim.SetFloat("Speed", agent.velocity.magnitude);

        float distance = Vector3.Distance(transform.position, playerScript.transform.position);
        Vector3 dirToPlayer = (playerScript.transform.position - transform.position).normalized;

        bool canSeePlayer = false;

        if (distance < detectionRange)
        {
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            if (angle < viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position + Vector3.up, dirToPlayer, out RaycastHit hit, distance, obstructionMask))
                {
                    canSeePlayer = true;
                }
            }
        }

        if (canSeePlayer)
        {
            isChasing = true;
            chaseCounter = chaseTime;
            if (visionSpotlight != null) visionSpotlight.color = alertColor;
            PerformAttackChase();
        }
        else if (isChasing)
        {
            if (visionSpotlight != null) visionSpotlight.color = alertColor;
            KeepChasing();
        }
        else
        {
            if (visionSpotlight != null) visionSpotlight.color = patrolColor;
            Wander();
        }
    }

    void PerformAttackChase()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = 4f;
        agent.SetDestination(playerScript.transform.position);

        Vector3 lookDir = playerScript.transform.position - transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);

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
        agent.stoppingDistance = 0f;
        agent.SetDestination(playerScript.transform.position);

        chaseCounter -= Time.deltaTime;
        if (chaseCounter <= 0) isChasing = false;
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

    // --- UPDATED VANISH WITH DEBUGGING ---
    public void Vanish()
    {
        // Update UI first
        if (UIManager.instance != null) UIManager.instance.AddKill();

        // Spawn 10 to 20 random gems
        if (gemPrefab != null)
        {
            int amount = Random.Range(10, 21); // Random range 10-20
            for (int i = 0; i < amount; i++)
            {
                Vector3 scatter = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
                Instantiate(gemPrefab, transform.position + scatter, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}