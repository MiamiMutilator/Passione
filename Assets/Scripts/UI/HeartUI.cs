using UnityEngine;

public class HeartUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;  // PlayerHealth Drag
    [SerializeField] private Animator animator;          // Animator on HeartImage 

    private static readonly int StateHash = Animator.StringToHash("State");
    private int _lastState = -1;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!playerHealth) playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    private void OnEnable()
    {
        Apply(true);
    }

    private void Update()
    {
        Apply(false);
    }

    private void Apply(bool force)

    // Check player health
    // 
    {
        if (!playerHealth || !animator) return;

        float ratio = playerHealth.maxHealth > 0
            ? playerHealth.Health / (float)playerHealth.maxHealth
            : 0f;

        // 0 = Full, 4 = Empty
        int state =
            ratio >= 0.99f ? 0 :
            ratio >= 0.75f ? 1 :
            ratio >= 0.50f ? 2 :
            ratio >= 0.25f ? 3 :
                              4;

        if (force || state != _lastState)
        {
            _lastState = state;
            animator.SetInteger(StateHash, state);
            Debug.Log($"[HeartUI] HP {playerHealth.Health}/{playerHealth.maxHealth} (ratio {ratio:F2}) -> State {state}");
        }
    }
}
