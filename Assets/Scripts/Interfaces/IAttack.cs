using UnityEngine;

public interface IAttack
{
    int Damage { get; set;  }
    GameObject Originator { get; set; }

    void OnSuccessfulHit();
}
