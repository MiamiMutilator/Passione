using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public GameObject Spawn(GameObject obj)
    {
        GameObject newObject = Instantiate(obj, transform.parent);

        return newObject;
    }
}
