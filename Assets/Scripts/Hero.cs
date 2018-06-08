using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    public Animator baseAnim;
    public Rigidbody body;
    public SpriteRenderer shadowSprite;

    public float speed = 2;
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
    protected Vector3 frontVector;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

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
        body.MovePosition(transform.position + moveVector * Time.fixedDeltaTime);
        baseAnim.SetFloat("Speed", moveVector.magnitude);

        if (moveVector != Vector3.zero) {
            if (moveVector.x != 0) {
                isFacingLeft = moveVector.x < 0;
            }
            FlipSprite(isFacingLeft);
        }
    }

    public void FlipSprite(bool isFacingLeft) {
        if (isFacingLeft) {
            frontVector = new Vector3(-1, 0, 0);
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            frontVector = new Vector3(1, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
