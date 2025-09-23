using UnityEngine;

public interface IHittable
{
    // damage: lượng sát thương
    // hitPoint: vị trí va chạm (world)
    // hitNormal: pháp tuyến bề mặt
    // instigator: đối tượng gây ra hit (người bắn)
    void Hit(float damage, Vector3 hitPoint, Vector3 hitNormal, GameObject instigator);
}
