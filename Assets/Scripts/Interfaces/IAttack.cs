using UnityEngine;

public interface IAttack
{
    GameObject Originator { get; set; }

    void OnSuccessfulHit();
}
