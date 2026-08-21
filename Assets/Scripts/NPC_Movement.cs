using UnityEngine;

public class NPCPatrol : MonoBehaviour
{
    [Header("Movement")]
    public float distance = 5f;
    public float speed = 2f;
    public float rotationSpeed = 5f;

    private Vector3 pointA;
    private Vector3 pointB;
    private bool goingToB = true;

    void Start()
    {
        // Wherever you place the NPC becomes Point A
        pointA = transform.position;

        // Point B is 'distance' units in front of the NPC
        pointB = pointA + transform.forward * distance;
    }

    void Update()
    {
        Vector3 target = goingToB ? pointB : pointA;

        // Move toward target
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        // Rotate toward target
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.magnitude > 0.05f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Reached target
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            // Switch direction
            goingToB = !goingToB;
        }
    }

    // Call this function later when you want to blast the NPC
    public void BlastNPC()
    {
        // Explosion will be added later
    }
}