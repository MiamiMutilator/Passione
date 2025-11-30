using UnityEngine;
using UnityEngine.AI;

public class RagdollEnabler : MonoBehaviour
{
    [SerializeField]
    public Animator Animator;
    [SerializeField]
    public Transform RagdollRoot;
    [SerializeField]
    public bool StartRagdoll = false;
    // Only public for Ragdoll Runtime GUI for explosive force
    public Rigidbody[] Rigidbodies;
    private CharacterJoint[] Joints;
    private Collider[] Colliders;

    public void Awake()
    {
        Animator = GetComponent<Animator>();

        Rigidbodies = RagdollRoot.GetComponentsInChildren<Rigidbody>();
        Joints = RagdollRoot.GetComponentsInChildren<CharacterJoint>();
        Colliders = RagdollRoot.GetComponentsInChildren<Collider>();
    }

    public void Update()
    {
        if (StartRagdoll)
        {
            EnableRagdoll();
            foreach(Rigidbody rb in Rigidbodies)
            {
                rb.isKinematic = false;
            }
            StartRagdoll = false;
        }
    }

    public void EnableRagdoll()
    {
        this.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        this.gameObject.GetComponent<EnemyPathing>().enabled = false;
        this.gameObject.GetComponent<Blocking>().enabled = false;
        this.gameObject.GetComponent<EnemyHealth>().enabled = false;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;

        Animator.enabled = false;

        foreach (CharacterJoint joint in Joints)
        {
            joint.enableCollision = true;
        }
        foreach (Collider collider in Colliders)
        {
            collider.enabled = true;
        }
        foreach (Rigidbody rigidbody in Rigidbodies)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.detectCollisions = true;
            rigidbody.useGravity = true;
        }
    }

    public void EnableAnimator()
    {
        Animator.enabled = true;
        foreach (CharacterJoint joint in Joints)
        {
            joint.enableCollision = false;
        }
        foreach (Collider collider in Colliders)
        {
            collider.enabled = false;
        }
        foreach (Rigidbody rigidbody in Rigidbodies)
        {
            rigidbody.detectCollisions = false;
            rigidbody.useGravity = false;
        }
    }
}