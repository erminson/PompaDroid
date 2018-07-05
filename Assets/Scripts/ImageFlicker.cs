﻿using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageFlicker : MonoBehaviour
{
    private bool isShown = true;
    public float flickerDelay = 0.3f;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        InvokeRepeating("ToggleImage", flickerDelay, flickerDelay);
    }

    void ToggleImage()
    {
        image.enabled = !isShown;
        isShown = !isShown;
    }
}
