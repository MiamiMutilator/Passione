using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpawnpointFactory : MonoBehaviour
{
    public GameObject spawnerPrefab;

    public GameObject CreateSpawner()
    {
        GameObject newSpawner = Instantiate(spawnerPrefab, transform);
        newSpawner.name = "Spawnpoint";

        return newSpawner;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpawnpointFactory)), CanEditMultipleObjects]
public class SpawnpointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        /* Serialized Field */
        serializedObject.Update();

        var spawner = serializedObject.FindProperty("spawnerPrefab");

        EditorGUILayout.PropertyField(spawner);

        serializedObject.ApplyModifiedProperties();

        if (spawner == null)
        {
            EditorGUILayout.HelpBox("Set spawner prefab before pressing the button!", MessageType.Warning);
        }

        /* Buttons */
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Select all spawners"))
            {
                var allSpawnComponents = FindObjectsByType<Spawnpoint>(FindObjectsSortMode.None);
                var allSpawners = allSpawnComponents
                    .Select(spawner => spawner.gameObject)
                    .ToArray();
                Selection.objects = allSpawners;
            }

            if (GUILayout.Button("Clear selection"))
            {
                Selection.objects = null;
            }
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Create Spawnpoint", GUILayout.Height(40)))
        {
            if(spawner != null)
            {
                SpawnpointFactory factory = (SpawnpointFactory) target; // Get reference to target class
                GameObject newSpawner = factory.CreateSpawner();
                Selection.objects = new GameObject[]{newSpawner}; // Select the newly created object
            }
        }
    }
}
#endif
