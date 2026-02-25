using UnityEngine;
using System.Collections;

public class EnemyGun : MonoBehaviour
{
    public Transform muzzlePoint;
    public LineRenderer bulletTracer;
    public float bulletRange = 20f;
    public float tracerDuration = 0.05f;
    public LayerMask targetLayer;

    public void Shoot(AssassinController target)
    {
        // Aiming at the chest (1 unit up)
        Vector3 targetCenter = target.transform.position + Vector3.up;
        Vector3 shootDir = (targetCenter - muzzlePoint.position).normalized;

        RaycastHit hit;
        Vector3 targetPoint;

        // VISUAL DEBUG: This draws a red line in the SCENE tab for 2 seconds
        Debug.DrawRay(muzzlePoint.position, shootDir * bulletRange, Color.red, 2f);

        // Check if we hit the LayerMask
        if (Physics.Raycast(muzzlePoint.position, shootDir, out hit, bulletRange, targetLayer))
        {
            targetPoint = hit.point;
            Debug.Log("PHYSICS HIT: " + hit.collider.name + " | Tag: " + hit.collider.tag);

            if (hit.collider.CompareTag("Player"))
            {
                target.TakeDamage(1);
            }
        }
        else
        {
            // If this runs, the ray missed everything on that layer
            Debug.Log("RAY MISSED: Check your LayerMask and Collider!");
            targetPoint = muzzlePoint.position + (shootDir * bulletRange);
        }

        StartCoroutine(DrawTracer(targetPoint));
    }

    private IEnumerator DrawTracer(Vector3 endPoint)
    {
        if (bulletTracer != null)
        {
            LineRenderer tracer = Instantiate(bulletTracer);
            tracer.SetPosition(0, muzzlePoint.position);
            tracer.SetPosition(1, endPoint);
            yield return new WaitForSeconds(tracerDuration);
            if (tracer != null) Destroy(tracer.gameObject);
        }
    }
}