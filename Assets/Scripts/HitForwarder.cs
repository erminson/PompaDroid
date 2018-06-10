using UnityEngine;

public class HitForwarder : MonoBehaviour
{
    public Actor actor;
    public Collider triggerCollider;

    void OnTriggerEnter(Collider hitCollider)
    {
        Debug.Log("OnTriggerEnter");
        Vector3 direction = new Vector3(hitCollider.transform.position.x - actor.transform.position.x, 0, 0);
        direction.Normalize();

        BoxCollider collider = triggerCollider as BoxCollider;
        Vector3 centralPoint = this.transform.position;
        if (collider) {
            centralPoint = transform.TransformPoint(collider.center);
        }
        Vector3 startPoint = hitCollider.ClosestPointOnBounds(centralPoint);
        actor.DidHitObject(hitCollider, startPoint, direction);
    }
}
