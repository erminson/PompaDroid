﻿using UnityEngine;
using System;
using System.Collections;

public class Hero : Actor
{
    bool isHurtAnim;

    public Walker walker;
    public bool isAutoPiloting;
    public bool controllable = true;

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

    bool isAttackingAnim;
    float lastAttackTime;
    float attackLimit = 0.14f;

    public bool canJumpAttack = true;
    private int currentAttackChain = 1;
    public int evaluatedAttackChain = 0;

    public AttackData jumpAttack;

    public override void Update()
    {
        base.Update();

        if (!isAlive) {
            return;
        }

        isAttackingAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack1") ||
                                  baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_attack");
        isJumpLandAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_land");
        isJumpingAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_rise") ||
                                baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_fall");
        isHurtAnim = baseAnim.GetCurrentAnimatorStateInfo(0).IsName("hurt");

        if (isAutoPiloting) {
            return;
        }

        float h = input.GetHorisontalAxis();
        float v = input.GetVerticalAxis();
        bool jump = input.GetJumpButtonDown();
        bool attack = input.GetAttackButtonDown();

        currentDir = new Vector3(h, 0, v);
        currentDir.Normalize();

        if (!isAttackingAnim) {
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
                    if (h != 0) {
                        lastWalkVector = currentDir;
                        lastWalk = Time.time;
                    }
                }
            }
        }

        if (jump && !isKnockedOut && !isJumpLandAnim && !isAttackingAnim &&
            (isGrounded || (isJumpingAnim && Time.time < lastJumpTime + jumpDuration))) {
            Jump(currentDir);
        }

        if (attack && Time.time >= lastAttackTime + attackLimit && !isKnockedOut) {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    protected override void DidLand()
    {
        base.DidLand();
        Walk();
    }

    void Walk()
    {
        speed = walkSpeed;
        isRunning = false;
        baseAnim.SetFloat("Speed", speed);
        baseAnim.SetBool("isRunning", isRunning);
    }

    void Stop()
    {
        speed = 0;
        isRunning = false;
        baseAnim.SetFloat("Speed", speed);
        baseAnim.SetBool("isRunning", isRunning);
    }

    void Run()
    {
        speed = runSpeed;
        isRunning = true;
        baseAnim.SetFloat("Speed", speed);
        baseAnim.SetBool("isRunning", isRunning);
    }

    public override void Attack()
    {
        if (!isGrounded) {
            if (isJumpingAnim && canJumpAttack) {
                canJumpAttack = false;

                currentAttackChain = 1;
                evaluatedAttackChain = 0;
                baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
                baseAnim.SetInteger("CurrentChain", currentAttackChain);

                body.velocity = Vector3.zero;
                body.useGravity = false;
            }
        } else {
            currentAttackChain = 1;
            evaluatedAttackChain = 0;
            baseAnim.SetInteger("EvaluatedChain", evaluatedAttackChain);
            baseAnim.SetInteger("CurrentChain", currentAttackChain);    
        }
    }

    public void DidChain(int chain)
    {
        baseAnim.SetInteger("EvaluatedChain", chain);
    }

    public void DidJumpAttack()
    {
        body.useGravity = true;
    }

    void FixedUpdate()
    {
        if (!isAutoPiloting) {
            if (!isAlive) {
                return;
            }

            Vector3 moveVector = currentDir * speed;
            if (isGrounded && !isAttackingAnim && !isJumpLandAnim && !isKnockedOut && !isHurtAnim) {
                body.MovePosition(transform.position + moveVector * Time.fixedDeltaTime);
                baseAnim.SetFloat("Speed", moveVector.magnitude);
            }

            if (moveVector != Vector3.zero && isGrounded && !isKnockedOut && !isAttackingAnim) {
                if (moveVector.x != 0) {
                    isFacingLeft = moveVector.x < 0;
                }
                FlipSprite(isFacingLeft);
            }    
        }
    }

    public override bool CanWalk()
    {
        return (isGrounded && !isAttackingAnim && !isJumpLandAnim && !isKnockedOut && !isHurtAnim);
    }

    void Jump(Vector3 direction)
    {
        if (!isJumpingAnim) {
            baseAnim.SetTrigger("Jump");
            lastJumpTime = Time.time;
            Vector3 horizontalVector = new Vector3(direction.x, 0, direction.z) * speed * 40;
            body.AddForce(horizontalVector, ForceMode.Force);
        }

        Vector3 verticalVector = Vector3.up * jumpForce * Time.deltaTime;
        body.AddForce(verticalVector, ForceMode.Force);
    }

    public void AnimateTo(Vector3 position, bool shouldRun, Action callback)
    {
        if (shouldRun) {
            Run();
        } else {
            Walk();
        }
        walker.MoveTo(position, callback);
    }

    public void UseAutopilot(bool useAutopilot)
    {
        isAutoPiloting = useAutopilot;
        walker.enabled = useAutopilot;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.collider.name == "Floor") {
            canJumpAttack = true;
        }
    }

    private void AnalyzeSpecialAttack(AttackData attackData, Actor actor, Vector3 hitPoint, Vector3 hitVector)
    {
        actor.EvaluateAttackData(attackData, hitVector, hitPoint);
    }

    protected override void HitActor(Actor actor, Vector3 hitPoint, Vector3 hitVector)
    {
        if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("attack1")) {
            base.HitActor(actor, hitPoint, hitVector);    
        } else if (baseAnim.GetCurrentAnimatorStateInfo(0).IsName("jump_attack")) {
            AnalyzeSpecialAttack(jumpAttack, actor, hitPoint, hitVector);
        }
    }

    public override void TakeDamage(float value, Vector3 hitVector, bool knockdown = false)
    {
        if (!isGrounded) {
            knockdown = true;
        }

        base.TakeDamage(value, hitVector, knockdown);
    }

    protected override IEnumerator KnockdownRoutine()
    {
        body.useGravity = true;
        return base.KnockdownRoutine();
    }
}
