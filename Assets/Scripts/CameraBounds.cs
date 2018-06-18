using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBounds : MonoBehaviour {
    public float minVisibleX;
    public float maxVisibleX;

    private float minValue;
    private float maxValue;

    public float cameraHalfWidth;

    public Camera activeCamera;

    public Transform cameraRoot;

    public Transform leftBounds;
    public Transform rightBounds;

    void Start() {
        activeCamera = Camera.main;

        float delta = activeCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x -
                                  activeCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        cameraHalfWidth = Mathf.Abs(delta) * 0.5f;

        minValue = minVisibleX + cameraHalfWidth;
        maxValue = maxVisibleX - cameraHalfWidth;

        Vector3 position;
        position = leftBounds.transform.localPosition;
        position.x = transform.localPosition.x - cameraHalfWidth;
        leftBounds.transform.localPosition = position;

        position = rightBounds.transform.localPosition;
        position.x = transform.localPosition.x + cameraHalfWidth;
        rightBounds.transform.localPosition = position;
    }

    public void SetXPosition(float x) {
        Vector3 trans = cameraRoot.position;
        trans.x = Mathf.Clamp(x, minValue, maxValue);
        cameraRoot.position = trans;
    }
}
