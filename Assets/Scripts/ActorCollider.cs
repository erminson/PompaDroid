using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ActorCollider : MonoBehaviour
{
    public Vector3 standingColliderCenter;
    public Vector3 standingCollederSize;

    public Vector3 downColliderCenter;
    public Vector3 downColliderSize;

    private BoxCollider actorCollider;

    void Awake()
    {
        actorCollider = GetComponent<BoxCollider>();
    }

    public void SetColliderStance(bool isStanding)
    {
        if (isStanding)
        {
            actorCollider.center = standingColliderCenter;
            actorCollider.size = standingCollederSize;
        }
        else
        {
            actorCollider.center = downColliderCenter;
            actorCollider.size = downColliderSize;
        }
    }
}
