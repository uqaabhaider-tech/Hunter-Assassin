using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class AssassinController : MonoBehaviour
{
    private NavMeshAgent agent;
    public Animator anim;
    public int health = 3;
    public float attackRange = 2.0f;
    private Transform targetEnemy;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
            HandleInput();

        if (targetEnemy != null)
        {
            if (Vector3.Distance(transform.position, targetEnemy.position) <= attackRange)
                ExecuteKill();
        }

        if (anim != null)
            anim.SetFloat("speed", agent.velocity.magnitude);
    }

    void HandleInput()
    {
        Vector2 screenPos = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Enemy"))
                targetEnemy = hit.collider.transform;
            else
                targetEnemy = null;

            agent.SetDestination(hit.point);
        }
    }

    void ExecuteKill()
    {
        if (targetEnemy == null) return;
        GuardAI enemy = targetEnemy.GetComponent<GuardAI>();
        if (enemy != null)
        {
            transform.LookAt(targetEnemy);
            if (anim != null) anim.SetTrigger("Attack");
            enemy.Vanish();
        }
        targetEnemy = null;
        agent.ResetPath();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Player Hit! Remaining Health: " + health); // Check your console for this message!

        if (health <= 0)
        {
            // This is what makes the player "Vanish"
            gameObject.SetActive(false);
            Debug.Log("PLAYER VANISHED");
        }
    }
}