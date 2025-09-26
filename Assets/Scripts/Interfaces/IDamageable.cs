using UnityEngine;

public interface IDamageable
{
    int Health { set; get; }
    bool Targetable { set; get; }
    void OnHit(GameObject source, int damage);
}
