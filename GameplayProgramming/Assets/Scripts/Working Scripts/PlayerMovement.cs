using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private float moveX;
    private float moveY;

    public CharacterController controller;
    public Transform cam;
    public GameObject particleEffect;

    Animator Anim;
    PlayerControls controls;

    Vector2 move;

    public float speed;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    private float speedBoostTimer = 3;
    private bool speedBoost = false;
    private bool doublejump = false;

    public float ySpeed;

    Vector3 motion;
    Vector3 moveDir;
    public float Gravity;
    private float yDirection;
    private float jumpNo;

    //jumping
    float jumpHeight; 

    void Awake()
    {
        Anim = GetComponent<Animator>();

        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => move = Vector2.zero;

        controls.Enable();

        controls.Player.DefaultActionRoll.performed += Roll;

        controls.Player.CombatAction.performed += Attack;
    }
    void Start()
    {
        particleEffect.SetActive(false);

    }
    private void OnMove(InputValue movementValue)
    {
        Vector2 moveVector = movementValue.Get<Vector2>();

        moveX = moveVector.x;
        moveY = moveVector.y;

    }
    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(moveX, 0f, moveY);
        controller.SimpleMove(Vector3.forward * 0);

        if(controller.isGrounded)
        {
            print("yes");
            yDirection = 0f;
            jumpNo = 0;
            Anim.SetBool("IsJumping", false);
            Anim.SetBool("HasLanded", true);
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            motion = direction * speed * Time.deltaTime;

            Anim.SetBool("IsRunning", true);
            Anim.SetBool("IsIdle", false);
        }
        else
        {
            direction = new Vector3(0f, 0f, 0f);
            motion = direction * speed * Time.deltaTime;

            Anim.SetBool("IsRunning", false);
            Anim.SetBool("IsIdle", true);
        }

        Vector3 velocity = Vector3.forward * 0;
        velocity.y = ySpeed;
        controller.Move(velocity * Time.deltaTime);

        if(speedBoost)
        {
            speed = 20;
            speedBoostTimer -= Time.deltaTime;
            particleEffect.SetActive(true);
            if(speedBoostTimer <= 0)
            {
                speed = 10;
                speedBoost = false;
                speedBoostTimer = 3;
                particleEffect.SetActive(false);
            }
        }

        if(doublejump)
        {

        }


        if (Input.GetButtonDown("Jump") && jumpNo < 1)
        {
            yDirection = ySpeed;
            jumpNo += 1;
            Anim.SetBool("IsJumping", true);
        }

        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * Gravity);
        }

        yDirection -= Gravity * Time.deltaTime;
        motion.y = yDirection;
        controller.Move(motion);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PowerUpsSpeed")
        {
            speedBoost = true;
            Destroy(other.gameObject);
        }

        if (other.tag == "PowerUpsJump")
        {
            doublejump = true;
            Destroy(other.gameObject);
        }
    }

    void Roll(InputAction.CallbackContext context)
    {
        Anim.SetTrigger("Dodge");
    }

    void Attack(InputAction.CallbackContext context)
    {
        Anim.SetTrigger("Attack");
    }
}