using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_DarkNepenthes : EnemyController
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;

    public override void AttackBehavior(List<GameObject> castedTargets)
    {
        GameObject projectileGO = Instantiate(projectile, firePoint.position,Quaternion.identity);
        bool isCritical;
        int damage = enemy.CalcDamage(out isCritical);
        projectileGO.GetComponent<EnemyProjectile>().Setup(direction, damage, isCritical);
    }
}
