using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack Details")]
    public Vector2[] attackMovement;

    public bool isBusy { get; private set; }

    [Header("Move Info")]
    public float moveSpeed = 10f;
    public float jumpForce = 10f;

    [Header("Dash Info")]
    public float dashSpeed = 10f;
    public float dashDuration = 1.5f;
    public float dashCooldown = 4f;
    public float dashCooldownTimer = 0f;
    public float dashDirection { get; private set; }



    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }

    #endregion


    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
      
    }

   protected override void Start()
   {
       base.Start();
       stateMachine.Initialize(idleState);
   }

   protected override void Update()
   {
       base.Update();
       stateMachine.CurrentState.Update();
       CheckForDashInput();

   }

   public IEnumerator BusyFor(float seconds)
   {
       isBusy = true;

       yield return new WaitForSeconds(seconds);

       isBusy = false;
   }

   public void AnimationTrigger() => stateMachine.CurrentState.AnimationFinishTrigger();

   private void CheckForDashInput()
   {
       if(IsWallDetected()) return;

       dashCooldownTimer -= Time.deltaTime;
       if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer < 0)
       {
           dashCooldownTimer = dashCooldown;

           dashDirection = Input.GetAxisRaw("Horizontal");

           if (dashDirection == 0)
           {
                dashDirection = facingDir;
           }

           stateMachine.ChangeState(dashState);

       }
    }




   

}
