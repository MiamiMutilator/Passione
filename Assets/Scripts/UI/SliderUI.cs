using UnityEngine;
using UnityEngine.UI;

public class SliderUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Slider slider;      // Passion Slider
    [SerializeField] private Animator animator;  // Background 

    [Header("Animator state names (full → empty)")]
    [SerializeField] private string[] stateNames = { "SliderFull", "SliderMid", "SliderLow" };

    [Header("Tuning")]
    [SerializeField] private float fadeDuration = 0.10f;
    [Tooltip("Bucket thresholds as ratio of slider.maxValue")]
    [SerializeField] private float fullMin = 0.67f; // adjust as need
    [SerializeField] private float midMin = 0.34f; // adjust as needed

    private int _lastIdx = -1;

    private void Awake()
    {
        if (!slider) slider = GetComponentInParent<Slider>();
        if (!animator) animator = GetComponent<Animator>();
    }

    private void OnEnable() => Apply(true);
    private void Update() => Apply(false);

    private void Apply(bool force)
    {
        if (!slider || !animator || stateNames.Length != 3) return;

        float max = (slider.maxValue > 0f) ? slider.maxValue : 100f;
        float ratio = Mathf.Clamp01(slider.value / max);

        int idx = (ratio >= fullMin) ? 0
                : (ratio >= midMin) ? 1
                : 2;

        if (force || idx != _lastIdx)
        {
            _lastIdx = idx;
            animator.CrossFade(stateNames[idx], fadeDuration, 0, 0f);
        }
    }
}
