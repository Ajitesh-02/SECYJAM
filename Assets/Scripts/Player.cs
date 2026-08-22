using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3f;

    private Animator animator;
    private NPCPatrol nearbyNPC;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        bool movementPressed = Input.GetKey(KeyCode.W) ||
                               Input.GetKey(KeyCode.A) ||
                               Input.GetKey(KeyCode.S) ||
                               Input.GetKey(KeyCode.D);

        animator.SetBool("isWalking", movementPressed);

        Vector2 inputVector = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            inputVector.y = 1;

        if (Input.GetKey(KeyCode.S))
            inputVector.y = -1;

        if (Input.GetKey(KeyCode.A))
            inputVector.x = -1;

        if (Input.GetKey(KeyCode.D))
            inputVector.x = 1;

        inputVector = inputVector.normalized;

        Vector3 moveDir = new Vector3(
            inputVector.x,
            0f,
            inputVector.y
        );

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float currentSpeed = moveDir.magnitude * moveSpeed;
        animator.SetFloat("SpeedMultiplier", currentSpeed / 5f);

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void HandleInteraction()
    {
        nearbyNPC = FindClosestNPC();

        if (nearbyNPC != null && Input.GetKeyDown(KeyCode.E))
        {
            nearbyNPC.BlastNPC();
        }
    }

    private NPCPatrol FindClosestNPC()
    {
        NPCPatrol[] npcs = FindObjectsOfType<NPCPatrol>();

        NPCPatrol closestNPC = null;
        float closestDistance = interactionRange;

        foreach (NPCPatrol npc in npcs)
        {
            float distance = Vector3.Distance(
                transform.position,
                npc.transform.position
            );

            if (distance <= closestDistance)
            {
                closestDistance = distance;
                closestNPC = npc;
            }
        }

        return closestNPC;
    }
}