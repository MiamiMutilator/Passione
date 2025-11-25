using UnityEngine;

public class TriggerDoorControllerMonoClose : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;

    [SerializeField] private bool openTrigger = false;
    [SerializeField] private bool closeTrigger = false;

    private void OnTriggerEnterClose(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(openTrigger)
            {
                myDoor.Play("RDoorOpen", 0, 0.0f);
                gameObject.SetActive(false);
            }

            else if(closeTrigger)
            {
                myDoor.Play("RDoorClose", 0, 0.0f);
                gameObject.SetActive(false);
            }
        }
    }
}
