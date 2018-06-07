using UnityEngine;

public class GameManager : MonoBehaviour {
    public Hero actor;
    public bool cameraFollows = true;
    public CameraBounds cameraBounds;

    void Start() {
        cameraBounds.SetPosition(cameraBounds.minVisibleX);
    }

    void Update() {
        if (cameraFollows) {
            cameraBounds.SetPosition(actor.transform.position.x);
        }
    }
}
