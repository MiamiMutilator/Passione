using UnityEngine;

public class HeartUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;  
    [SerializeField] private Animator animator;           

    private static readonly int StateHash = Animator.StringToHash("State");
    private int _lastState = -1;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        
        ApplyState(force: true);
    }

    private void Update()
    {
        ApplyState(force: false);
    }

    private void ApplyState(bool force)
    {
        if (playerHealth == null || animator == null) return;

        
        float ratio = playerHealth.maxHealth > 0
            ? playerHealth.Health / (float)playerHealth.maxHealth
            : 0f;

        // Convert health ratio into one of 5 bands (0 = full, 4 = empty)
        int state =
            ratio >= 0.99f ? 0 :   // Full health
            ratio >= 0.75f ? 1 :   // 3/4
            ratio >= 0.50f ? 2 :   // 2/4
            ratio >= 0.25f ? 3 :   // 1/4
                               4;   // Empty (0 health)

        // Only update Animator if state changes or on forced refresh
        if (force || state != _lastState)
        {
            _lastState = state;
            animator.SetInteger(StateHash, state);

            // Debug line (optional):
            // Debug.Log($"HeartUI → Health: {playerHealth.Health}/{playerHealth.maxHealth}, Ratio: {ratio:F2}, State: {state}");
        }
    }
}
