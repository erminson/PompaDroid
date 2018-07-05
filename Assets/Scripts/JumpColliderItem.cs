using UnityEngine;
using System.Collections;

public class JumpColliderItem : MonoBehaviour
{
    public int isTriggeredCount = 0;

    void OnTriggerEnter(Collider other)
    {
        isTriggeredCount++;    
    }

    void OnTriggerExit(Collider other)
    {
        isTriggeredCount--;
    }
}
