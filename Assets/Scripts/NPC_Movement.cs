using UnityEngine;

public class NPCPatrol : MonoBehaviour
{
    [Header("Movement")]
    public float distance = 5f;
    public float speed = 2f;
    public float rotationSpeed = 5f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;

    [Header("Time Loop")]
    [SerializeField] private float timeTolerance = 0.05f;

    private Vector3 pointA;
    private Vector3 pointB;
    private bool goingToB = true;

    private bool hasBeenBlasted = false;

    private const string BlastTimeKey = "NPCBlastTime";
    private const string HasRecordedKey = "HasRecordedBlast";

    private bool replayMode = false;
    private float recordedBlastTime;

    void Start()
    {
        pointA = transform.position;
        pointB = pointA + transform.forward * distance;

        // Check if this is a replay
        if (PlayerPrefs.GetInt(HasRecordedKey, 0) == 1)
        {
            replayMode = true;
            recordedBlastTime = PlayerPrefs.GetFloat(BlastTimeKey);

            Debug.Log("Replay mode. NPC will blast at: "
                      + recordedBlastTime + " seconds");
        }
    }

    void Update()
    {
        if (hasBeenBlasted)
            return;

        // ==========================================
        // REPLAY MODE
        // ==========================================

        if (replayMode)
        {
            if (Time.timeSinceLevelLoad >= recordedBlastTime)
            {
                BlastNPC();
                return;
            }
        }

        // ==========================================
        // NPC MOVEMENT
        // ==========================================

        Vector3 target = goingToB ? pointB : pointA;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.magnitude > 0.05f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            goingToB = !goingToB;
        }
    }

    // ==========================================
    // CALLED BY PLAYER
    // ==========================================

    public void BlastNPC()
    {
        if (hasBeenBlasted)
            return;

        hasBeenBlasted = true;

        // Only record time during FIRST playthrough
        if (!replayMode)
        {
            float blastTime = Time.timeSinceLevelLoad;

            PlayerPrefs.SetFloat(
                BlastTimeKey,
                blastTime
            );

            PlayerPrefs.SetInt(
                HasRecordedKey,
                1
            );

            PlayerPrefs.Save();

            Debug.Log(
                "FIRST BLAST TIME RECORDED: "
                + blastTime
            );
        }

        // Spawn explosion
        if (explosionPrefab != null)
        {
            Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        // Ask GameManager to reload after 2 seconds
        if (GameReset.Instance != null)
        {
            GameReset.Instance.ResetGameAfter(2f);
        }
        else
        {
            Debug.LogError("GameReset GameManager not found!");
        }

        // Destroy NPC
        Destroy(gameObject);
    }
}