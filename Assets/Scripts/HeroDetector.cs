﻿using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HeroDetector : MonoBehaviour
{
    public bool heroIsNearby;

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Hero") {
            heroIsNearby = true;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.name == "Hero") {
            heroIsNearby = false;
        }
    }
}