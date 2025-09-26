using UnityEngine;

public interface IDamageable
{
    int Health { set; get; }
    bool Targetable { set; get; }
    void OnHit(IAttack source, int damage);
}
