using UnityEngine;

public class DrawSpawnpoint : MonoBehaviour
{
    public Vector3 size = new Vector3(2f, 5f, 2f);
    public Color gizmoColor = new Color(1f, 0f, 0f);
    [Range(0f, 1f)]
    public float opacity = 0.5f;

    private void OnDrawGizmos()
    {
        // Set the color with custom alpha
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, opacity);

        // Draw the cube
        Gizmos.DrawCube(transform.position, size);

        // Draw wire cube outline
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
