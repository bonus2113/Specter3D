﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

public enum PlayerTriggerType
{
    Stab,
    Jump,
    Kick,
    DoubleJump,
    Run,
    Turn,
    StopWalking,
    Flutter
}

public enum PlayerFlagType
{
    Ducking,
    Fluttering,
    OnGround,
    Running
}

public enum PlayerState
{
    Flutter
}

public class PlayerAnimationController : MonoBehaviour
{
    public Rigidbody2D MovementRigidbody;
    public EntityPhysics EntityPhysics;

    private Animator anim;

    private float prevSpeed = 0.0f;

    private float dampedSpeed = 0.0f;
    private float speedDampVel = 0.0f;

    private List<PlayerTriggerType> triggersToReset = new List<PlayerTriggerType>();

    private int prevDir = 0;

    public void SetFacing(int dir)
    {
        if(dir == 0)
        {
            return;
        }

        var scale = transform.localScale;
        scale.x = dir;
        transform.localScale = scale;
    }

    public void SetFlag(PlayerFlagType flag, bool value)
    {
        anim.SetBool(flag.ToString(), value);
    }

    public void SetState(PlayerState state)
    {
        anim.Play(state.ToString());
    }

    public void ActivateTrigger(PlayerTriggerType type, bool resetNextFrame = false)
    {
        anim.SetTrigger(type.ToString());

        if (resetNextFrame)
        {
            triggersToReset.Add(type);
        }
    }

	// Use this for initialization
	void Start ()
	{
	    anim = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        float speed = Mathf.Abs(MovementRigidbody.velocity.x);
        dampedSpeed = Mathf.SmoothDamp(dampedSpeed, speed, ref speedDampVel, 0.1f);

        anim.SetFloat("speed", speed);
        anim.SetFloat("dampedSpeed", dampedSpeed);

        int dir = (int)Mathf.Sign(transform.localScale.x);
        if (prevDir != dir && speed > 0.1f)
        {
            ActivateTrigger(PlayerTriggerType.Turn, true);
        }

        if (prevSpeed > 0.1f && speed <= 0.1f)
        {
            ActivateTrigger(PlayerTriggerType.StopWalking, true);
        }

        prevSpeed = speed;
        prevDir = dir;
	}

    void LateUpdate()
    {
        SetFlag(PlayerFlagType.OnGround, EntityPhysics.IsGrounded);

        for (int i = 0; i < triggersToReset.Count; i++)
        {
            anim.ResetTrigger(triggersToReset[i].ToString());
        }

        triggersToReset.Clear();
    }
}