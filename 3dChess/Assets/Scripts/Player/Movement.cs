using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController Controller;
    private Animator Animator;
    public Transform Cam;
    public float speed = 12f;
    public float jumpForce = 5;
    public float gravity = -9.81f;

    public Vector3 velocity, move;
    private Vector2 MovementInput;

    public bool isGrounded;

    private Player PlayerInput;

    private Vector3 LastPos = Vector3.zero;

    public bool Moving = false;
    public static bool IsPaused = false;
    public static int SprintAdditive;

    public float RayDist;

    private Coroutine WaitForSadIdle;
    public float SadIdleTimeout = 10;

    private void Awake()
    {
        //Cam = Camera.main.transform;
        Controller = GetComponent<CharacterController>();
        Animator = GetComponentInChildren<Animator>();
        PlayerInput = new();
        IsPaused = false;
    }
    private void Start()
    {

    }

    private void OnEnable()
    {
        PlayerInput.Enable();
    }
    private void OnDisable()
    {
        PlayerInput.Disable();
    }

    void Update()
    {
        if (IsPaused)
        {
            if(Moving)
            {
                Moving = false;
                Animator.SetBool("moving", false);
            }
            return;
        }

        isGrounded = Controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        /*if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravity);
        }*/

        if (Input.GetKey(KeyCode.LeftShift))
            SprintAdditive = 2;
        else
            SprintAdditive = 1;

        MovementInput = PlayerInput.Main.Move.ReadValue<Vector2>();
        move = Cam.forward * MovementInput.y + Cam.right * MovementInput.x;
        move.y = 0;
        Controller.Move(speed * SprintAdditive * Time.deltaTime * move);

        velocity = AdjustVelocityToSlope(velocity);
        velocity.y += gravity * Time.deltaTime;

        Controller.Move(velocity * Time.deltaTime);

        if (MovementInput != Vector2.zero && !Moving && LastPos != gameObject.transform.position)
        {
            Moving = true;

            if(WaitForSadIdle != null)
                StopCoroutine(WaitForSadIdle);
            Animator.SetBool("moving", true);
        }
        if ((MovementInput == Vector2.zero || LastPos == gameObject.transform.position) && Moving)
        {
            Moving = false;
            Animator.SetBool("moving", false);
            WaitForSadIdle = StartCoroutine(Helper.ActionAfterTimeCor(SadIdleTimeout, () => { Animator.SetTrigger("sad"); }));
        }

        if(Moving)
        {
            var current = Animator.gameObject.transform.rotation;
            var target = Quaternion.Euler(0f, Movement2DToRot(MovementInput), 0f);
            Animator.gameObject.transform.rotation = Quaternion.Slerp(current, target, 0.1f);
        }

        LastPos = gameObject.transform.position;
    }

    public Vector3 AdjustVelocityToSlope(Vector3 velo)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit rch, RayDist))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, rch.normal);
            var adjustedVelo = slopeRotation * velo;

            if (adjustedVelo.y < 0)
            {
                return adjustedVelo;
            }
        }

        return velo;
    }

    public void ToggleMovement(bool toggle)
    {
        IsPaused = toggle;
    }
    private float Movement2DToRot(Vector2 Movement)
    {
        
        var roundY = Mathf.RoundToInt(Movement.y);
        var roundX = Mathf.RoundToInt(Movement.x);

        return (roundY, roundX) switch
        {
            (1, 0) => 0,
            (1, 1) => 45,
            (0, 1) => 90,
            (-1, 1) => 135,
            (-1, 0) => 180,
            (-1, -1) => 225,
            (0, -1) => 270,
            (1, -1) => 315,
            _ => 0,

        };
    }

}
