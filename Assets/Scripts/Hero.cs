using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Actor {
    
    public float walkSpeed = 2;
    public float runSpeed = 5;

    bool isRunning;
    bool isMoving;
    float lastWalk;
    public bool canRun = true;
    float tapAgainToRunTime = 0.2f;
    Vector3 lastWalkVector;

    Vector3 currentDir;
    bool isFacingLeft;


    bool isJumpLandAnim;
    bool isJumpingAnim;

    public InputHandler input;

    public float jumpForce = 1750;
    private float jumpDuration = 0.2f;
    private float lastJumpTime;

    public override void Update()
    {
        base.Update();

        isJumpLandAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_land");
        isJumpingAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_rise") || 
                                baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_fall");
        
        float h = input.GetHorisontalAxis();
        float v = input.GetVerticalAxis();
        bool jump = input.GetJumpButtonDown();

        currentDir = new Vector3(h, 0, v);
        currentDir.Normalize();

        if (v == 0 && h == 0) {
            Stop();
            isMoving = false;
        } else if (!isMoving && (v != 0 || h != 0)) {
            isMoving = true;
            float dotProduct = Vector3.Dot(currentDir, lastWalkVector);

            if (canRun && Time.time < lastWalk + tapAgainToRunTime && dotProduct > 0) {
                Run();
            } else {
                Walk();
            }

            if (h != 0) {
                lastWalkVector = currentDir;
                lastWalk = Time.time;
            }
        }

        if (jump && !isJumpLandAnim &&
            (isGrounded || (isJumpingAnim && Time.time < lastJumpTime + jumpDuration))) {
            Jump(currentDir);
        }
    }

    protected override void DidLand()
    {
        base.DidLand();
        Walk();
    }

    void Walk() {
        speed = walkSpeed;
        isRunning = false;
        baseAnim.SetFloat("Speed", speed);
        baseAnim.SetBool("isRunning", isRunning);
    }

    void Stop() {
        speed = 0;
        isRunning = false;
        baseAnim.SetFloat("Speed", speed);
        baseAnim.SetBool("isRunning", isRunning);
    }

    void Run() {
        speed = runSpeed;
        isRunning = true;
        baseAnim.SetFloat("Speed", speed);
        baseAnim.SetBool("isRunning", isRunning);
    }

    void FixedUpdate() {
        Vector3 moveVector = currentDir * speed;

        if (isGrounded) {
            body.MovePosition(transform.position + moveVector * Time.fixedDeltaTime);
            baseAnim.SetFloat("Speed", moveVector.magnitude);    
        }

        if (moveVector != Vector3.zero) {
            if (moveVector.x != 0) {
                isFacingLeft = moveVector.x < 0;
            }
            FlipSprite(isFacingLeft);
        }
    }

    void Jump(Vector3 direction) {
        if (!isJumpingAnim) {
            baseAnim.SetTrigger("Jump");
            lastJumpTime = Time.time;
            Vector3 horizontalVector = new Vector3(direction.x, 0, direction.z) * speed * 40;
            body.AddForce(horizontalVector, ForceMode.Force);
        }

        Vector3 verticalVector = Vector3.up * jumpForce * Time.deltaTime;
        body.AddForce(verticalVector, ForceMode.Force);
    }
}
