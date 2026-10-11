using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerState
{
    Idle,
    Running,
    Jumping,
    Attacking
}

public class PlayerController : MonoBehaviour
{    
    Rigidbody2D rb;
    SpriteRenderer sr;
    [SerializeField] Sprite idle_sprite;
    [SerializeField] Sprite running_sprite;
    [SerializeField] Sprite jumping_sprite;
    [SerializeField] Sprite attack_sprite;

    [SerializeField] GameObject hitBox;
    // [SerializeField] private InputActionReference dropActionRef;
    [SerializeField] private GameObject WinText;


    float[] gravityScale = new float[] {1f, 4f}; // Array!!!
    float move_input;
    private float moveInput;
    float speed = 8f;
    float jumpPower = 10f;
    bool isJumping = false;
    bool shouldJump = false;
    bool dropFaster = false;
    PlayerState currentState = PlayerState.Idle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        currentState = PlayerState.Idle;
    }


    // This part hasn't been implemented yet, but it will be used for the attack animation and hitbox activation.
    IEnumerator AttackRoutinue()
    {
        currentState = PlayerState.Attacking;
        hitBox.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        
        hitBox.SetActive(false);

        if (isJumping) currentState = PlayerState.Jumping;
        else if (move_input != 0) currentState = PlayerState.Running;
        else currentState = PlayerState.Idle;
    }

    #region Input System

    public void OnMove(InputValue value)
    {
        move_input = value.Get<float>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && !isJumping)
        {
            isJumping = true;
            shouldJump = true;
        }
    }

    public void OnDrop(InputValue value)
    {
        dropFaster = value.isPressed;
    }

    // This Part unfinished yet, but it will be used for the attack animation and hitbox activation.
    /*
    public void OnAttack(InputValue value)
    {
        if (value.isPressed && currentState != PlayerState.Attacking)
        {
            StartCoroutine(AttackRoutinue());
        }
    }
    */
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            currentState = PlayerState.Idle;
        }

        if (collision.gameObject.CompareTag("Outside"))
        {
            transform.position = new Vector2(-7.36000013f, -2.73000002f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            collision.gameObject.SetActive(false);
            WinText.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * move_input * speed * Time.deltaTime); // Vector2!!! But not new Vector2(1, 0);

        if (shouldJump)
        {
            shouldJump = false;
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }

        if(dropFaster)
        {
            rb.gravityScale = gravityScale[1];
        }
        else
        {
            rb.gravityScale = gravityScale[0];
        }
    }

    private void Update()
    {
        if (move_input != 0)
        {
            float faceSide = move_input > 0 ? 1f : 0;
            if(faceSide == 1f)
            {
                sr.flipX = false;
            }
            else
            {
                sr.flipX = true;
            }
        }


        
        if (currentState != PlayerState.Attacking)
        {
            if (move_input != 0)
            {
                currentState = PlayerState.Running;
            }
            else
            {
                currentState = PlayerState.Idle;
            }

            if(isJumping)
                currentState = PlayerState.Jumping;
        }



        if(currentState == PlayerState.Idle)
        {
            sr.sprite = idle_sprite;
        }
        else if (currentState == PlayerState.Running)
        {
            sr.sprite = running_sprite;
        }
        else if (currentState == PlayerState.Jumping)
        {
            sr.sprite = jumping_sprite;
        }
        else if (currentState == PlayerState.Attacking)
        {
            sr.sprite = attack_sprite;
        }
    }
}