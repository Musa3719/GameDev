using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    public Rigidbody rb;
    private bool isCameraLooksForward;
    private bool isPlayerRotationFree;
    public Animator animator;
    
    private float horizontal;
    private float vertical;
    private Vector3 direction;
    public float WalkSpeed { get; private set; }
    public float RunSpeed { get; private set; }
    public float AttackMoveSpeed { get; private set; }
    public float DefendMoveSpeed { get; private set; }
    public float ThrowMoveSpeed { get; private set; }
    public float DodgeMoveSpeed { get; private set; }
    public float RollMoveSpeed { get; private set; }
    public float HurtMoveSpeed { get; private set; }
    public bool isLerpingIdle;
    private void Awake()
    {
        instance = this;
        //animator = GetComponentInChildren<Animator>();
        isPlayerRotationFree = true;
        rb = GetComponent<Rigidbody>();
        WalkSpeed = 3f;
        RunSpeed = 5f;
        AttackMoveSpeed = 1.5f;
        DefendMoveSpeed = 2.5f;
        ThrowMoveSpeed = 3f;
        DodgeMoveSpeed = 10f;
        RollMoveSpeed = 7f;
        HurtMoveSpeed = 1f;
        
    }
    private void Update()
    {
        if (GameManager.isPaused) return;

        if (isLerpingIdle)
        {
            LerpIdle();
        }
    }
    public void Idle()
    {
        //GetDirection();
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }
    public void Walk()
    {
        GetDirection(true);
        var speed = direction * WalkSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void Run()
    {
        GetDirection(true);
        var speed = direction * RunSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    private void ArrangeMoveAnimations()
    {
        //only when upper body animation plays(not while roll hurt dodge happening)
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        direction = new Vector3(horizontal, 0, vertical).normalized;
        if (direction.magnitude < 0.1f)
        {
            if (PlayerMovement.instance.animator.GetCurrentAnimatorClipInfo(0).Length>0 && !PlayerMovement.instance.animator.GetCurrentAnimatorClipInfo(0)[0].clip.ToString().StartsWith("Idle"))//0 = normal layer
                PlayerMovement.instance.animator.CrossFade("Idle", 0.1f);
        }
        else
        {
            if (PlayerMovement.instance.animator.GetCurrentAnimatorClipInfo(0).Length > 0 && !PlayerMovement.instance.animator.GetCurrentAnimatorClipInfo(0)[0].clip.ToString().StartsWith("Walk"))//0 = normal layer
                PlayerMovement.instance.animator.CrossFade("Walk", 0.1f);
        }
    }
    public void AttackMoveArrange()
    {
        //ArrangeMoveAnimations();
        GetDirection(false);
        var speed = direction * AttackMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void DefendMove()
    {
        GetDirection(true);
        ArrangeMoveAnimations();
        var speed = direction * DefendMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void ThrowMove()
    {
        GetDirection(true);
        ArrangeMoveAnimations();
        var speed = direction * ThrowMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void DodgeMoveArrange()
    {
        GetDirection(false);
        var speed = direction * DodgeMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void RollMoveArrange()
    {
        GetDirection(false);
        var speed = direction * RollMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void HurtMoveArrange()
    {
        GetDirection(false);
        var speed = direction * HurtMoveSpeed;
        speed.y = rb.velocity.y;
        rb.velocity = speed;
    }
    public void PoiseBrokenMoveArrange(float speed)
    {
        GetDirection(false);
        var newSpeed = direction * speed;
        newSpeed.y = rb.velocity.y;
        rb.velocity = newSpeed;
        isLerpingIdle = true;
    }
    public void LerpIdle()
    {
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), Time.deltaTime * 5);
    }
    public void DyingMoveArrange(float speed)
    {
        PoiseBrokenMoveArrange(speed);
        Player.instance.MakeRagdoll();
    }
    private void GetDirection(bool isLerping)
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        direction = new Vector3(horizontal, 0, vertical).normalized;
        LookToDirection(direction, isLerping);
    }
    private void LookToDirection(Vector3 dir, bool isLerping)
    {
        //Debug.Log(transform.InverseTransformDirection(rb.velocity));
        animator.SetFloat("xVel", transform.InverseTransformDirection(rb.velocity.normalized).x);
        animator.SetFloat("zVel", transform.InverseTransformDirection(rb.velocity.normalized).z);
        if (isPlayerRotationFree)
        {
            FreeLook(isLerping);
        }
        else
        {
            NormalLook(dir, isLerping);
        }
        
    }
    private void NormalLook(Vector3 dir, bool isLerping)
    {
        dir.y = 0;
        if (dir != Vector3.zero && isLerping)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.03f);
        else if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }
    private void FreeLook(bool isLerping)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            var dir = hit.point - transform.position;
            dir = dir.normalized;
            dir.y = 0;
            if (dir != Vector3.zero && isLerping)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.03f);
            else if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
