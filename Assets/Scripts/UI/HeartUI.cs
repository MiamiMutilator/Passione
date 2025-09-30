using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth; 
    [SerializeField] private Image heartImage; 

    [Header("Sprites (0 = empty ... 4 = full)")]
    [Tooltip("Exactly 5 sprites. 0 = empty, 4 = full.")]
    [SerializeField] private Sprite[] heartSprites = new Sprite[5];

    private int _lastIndex = -1;

    private void Reset()
    {
        heartImage = GetComponent<Image>();
    }

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        ForceRefresh();
    }

    private void Update()
    {
        if (playerHealth == null) return;

        // Scale health (0..maxHealth) into 0..4
        int index = Mathf.RoundToInt(
            (playerHealth.Health / (float)playerHealth.maxHealth) * (heartSprites.Length - 1)
        );

        index = Mathf.Clamp(index, 0, heartSprites.Length - 1);

        if (index != _lastIndex)
            ForceRefresh();
    }

    private void ForceRefresh()
    {
        int index = Mathf.RoundToInt(
            (playerHealth.Health / (float)playerHealth.maxHealth) * (heartSprites.Length - 1)
        );

        index = Mathf.Clamp(index, 0, heartSprites.Length - 1);

        _lastIndex = index;
        if (heartSprites != null && heartSprites.Length > 0)
            heartImage.sprite = heartSprites[index];
    }
}