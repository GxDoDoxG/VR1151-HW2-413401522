using System.Collections;
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


    float move_input;
    private float moveInput;
    float speed = 8f;
    float jumpPower = 10f;
    bool isJumping = false;
    bool shouldJump = false;
    PlayerState currentState = PlayerState.Idle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        currentState = PlayerState.Idle;
    }

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

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && currentState != PlayerState.Attacking)
        {
            StartCoroutine(AttackRoutinue());
        }
    }

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
            transform.position = new Vector3(-7.36000013f, -2.73000002f, 0);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * move_input * speed * Time.deltaTime);

        if (shouldJump)
        {
            shouldJump = false;
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    private void Update()
    {
        if (move_input != 0)
        {
            float targetScaleX = move_input > 0 ? 1f : -1f;
            transform.localScale = new Vector3(targetScaleX, transform.localScale.y, transform.localScale.z);
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