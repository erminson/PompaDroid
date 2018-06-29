﻿using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    float horizontal;
    float vertical;
    bool jump;
    bool attack;

    float lastJustTime;
    bool isJumping;
    public float maxJumpDuration = 0.2f;

    bool didAttack;
    public bool useUI = true;

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

    public bool GetAttackButtonDown()
    {
        return attack;
    }

    private void Update()
    {
        if (useUI)
        {
            if (didAttack)
            {
                didAttack = attack = false;
            }
            else if (attack)
            {
                didAttack = true;
            }
        } 
        else
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            attack = Input.GetButtonDown("Attack");

            if (!jump && !isJumping && Input.GetButton("Jump"))
            {
                jump = true;
                lastJustTime = Time.time;
                isJumping = true;
            }
            else if (!Input.GetButton("Jump"))
            {
                jump = false;
                isJumping = false;
            }

            if (jump && Time.time > lastJustTime + maxJumpDuration)
            {
                jump = false;
            } 
        }
    }

    public void DidPressAttack(BaseEventData data)
    {
        attack = true;
        didAttack = false;
    }

    public void DidPressJump(BaseEventData data)
    {
        if (!jump)
        {
            jump = true;
            lastJustTime = Time.time;
        }
    }

    public void DidReleaseJump(BaseEventData data)
    {
        jump = false;
    }

    public Vector2 VectorForPadDirection(ActionDPad.ActionPadDirection padDirection)
    {
        float maxX = 1.0f;
        float maxY = 1.1f;
        switch (padDirection)
        {
            case ActionDPad.ActionPadDirection.None:
                return Vector2.zero;
            case ActionDPad.ActionPadDirection.Up:
                return new Vector2(0, maxY);
            case ActionDPad.ActionPadDirection.UpRight:
                return new Vector2(maxX, maxY);
            case ActionDPad.ActionPadDirection.Right:
                return new Vector2(maxX, 0);
            case ActionDPad.ActionPadDirection.DownRight:
                return new Vector2(maxX, -maxY);
            case ActionDPad.ActionPadDirection.Down:
                return new Vector2(0, -maxY);
            case ActionDPad.ActionPadDirection.DownLeft:
                return new Vector2(-maxX, -maxY);
            case ActionDPad.ActionPadDirection.Left:
                return new Vector2(-maxX, 0);
            case ActionDPad.ActionPadDirection.UpLeft:
                return new Vector2(-maxX, maxY);
            default:
                return Vector2.zero;
        }
    }

    public void OnActionPadChangeDirection(ActionDPad.ActionPadDirection direction)
    {
        Vector2 directionVector = VectorForPadDirection(direction);
        horizontal = directionVector.x;
        vertical = directionVector.y;
    }
}
