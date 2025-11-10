using UnityEngine;

public class HeartUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerHealth playerHealth;   // Player
    [SerializeField] private Animator animator;           // HeartImage

    [Header("Animator state names (full → empty)")]
    [SerializeField]
    private string[] stateNames =
        { "HeartFull", "Heart1", "Heart2", "Heart3", "Heart4" };

    [Header("Tuning")]
    [SerializeField] private float fadeDuration = 0.10f;  // crossfade

    private int _lastIndex = -1;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!playerHealth) playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    void OnEnable() => Apply(force: true);

    void Update() => Apply(force: false);

    private void Apply(bool force)
    {
        if (!playerHealth || !animator || stateNames.Length != 5) return;

        
        float ratio = playerHealth.maxHealth > 0 //player health
            ? Mathf.Clamp01(playerHealth.Health / (float)playerHealth.maxHealth)
            : 0f;

        // Map to 5 buckets (Full, 75%, 50%, 25%, Empty)
        // 10 - Full
        // 8–9 - Heart1
        // 6–7 - Heart2
        // 3–5 - Heart3
        // 0–2 - Heart4 empty
        int idx =
            (ratio >= 0.99f) ? 0 :
            (ratio >= 0.75f) ? 1 :
            (ratio >= 0.50f) ? 2 :
            (ratio >= 0.25f) ? 3 : 4;

        if (force || idx != _lastIndex)
        {
            _lastIndex = idx;
            animator.CrossFade(stateNames[idx], fadeDuration, 0, 0f);
        }
    }
}
