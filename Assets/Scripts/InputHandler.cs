using UnityEngine;

public class InputHandler : MonoBehaviour
{
    float horizontal;
    float vertical;
    bool jump;

    float lastJustTime;
    bool isJumping;
    public float maxJumpDuration = 0.2f;

    public float GetVerticalAxis() 
    {
        return vertical;
    }

    public float GetHorisontalAxis()
    {
        return horizontal;
    }

    public bool GetJumpButtonDown()
    {
        return jump;
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (!jump && !isJumping && Input.GetButton("Jump")) {
            jump = true;
            lastJustTime = Time.time;
            isJumping = true;
        } else if (!Input.GetButton("Jump")) {
            jump = false;
            isJumping = false;
        }

        if (jump && Time.time > lastJustTime + maxJumpDuration) {
            jump = false;
        }
    }

}
