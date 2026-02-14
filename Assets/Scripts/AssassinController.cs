using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem; // Necessary for New Input System

public class AssassinController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            MoveToPointer();
        }

        // NEW LOGIC: Check if we are close enough to stop
        float speed = 0;

        // If the agent is still calculating a path or moving
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            speed = agent.velocity.magnitude;
        }
        else
        {
            // Force the speed to 0 if we are at the destination
            speed = 0;
            agent.velocity = Vector3.zero; // Stop any micro-drifting
        }

        anim.SetFloat("speed", speed);
    }

    void MoveToPointer()
    {
        // Get the position of the pointer (Touch or Mouse)
        Vector2 pointerPosition = Pointer.current.position.ReadValue();

        // Convert the 2D screen point to a 3D Ray
        Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
        RaycastHit hit;

        // Perform the Raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Move the agent to the hit point
            agent.SetDestination(hit.point);
        }
    }
}