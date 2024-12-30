using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Move & Jump Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    
    [Header("Duck Settings")]
    [SerializeField] private float duckSpeed = 2.5f;
    [SerializeField] private Vector2 duckColliderSize = new Vector2(1f, 0.5f); 
    [SerializeField] private Vector2 normalColliderSize = new Vector2(1f, 1f);
    
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 5f;

    private Rigidbody2D player;
    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private bool isDucking = false;
    private bool isDashing = false;
    private bool canDash = true;
    
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0, value); 
    }

    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = Mathf.Max(0, value); 
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        player = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (!isDashing)
        {
            HandleMovement();
            HandleDucking(); 
            HandleJump();
        } 
        HandleDash();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal"); 
        float speed = isDucking ? duckSpeed : moveSpeed;
        player.velocity = new Vector2(moveInput * moveSpeed, player.velocity.y);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            player.velocity = new Vector2(player.velocity.x, jumpForce);
        }
    }
    
    private void HandleDucking()
    {
        if (Input.GetKey(KeyCode.DownArrow)) 
        {
            if (!isDucking)
            {
                isDucking = true;
                boxCollider.size = duckColliderSize;
            }
        }
        else
        {
            if (isDucking)
            {
                isDucking = false;
                boxCollider.size = normalColliderSize; 
                Debug.Log($"Position after ducking: {transform.position}");
                Debug.Log($"Collider size: {boxCollider.size}");
            }
        }
    }
    
    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDucking) 
        {
            Debug.Log($"KEY: {KeyCode.LeftShift}");
            Debug.Log($"KEY: {canDash}");
            Debug.Log($"KEY: {isDucking}");
            StartCoroutine(Dash());
        }
    }
    
    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float originalGravity = player.gravityScale; 
        player.gravityScale = 0; // Temporarily disabling gravity
        player.velocity = new Vector2(dashSpeed, 0); 

        yield return new WaitForSeconds(dashDuration); 

        player.gravityScale = originalGravity; 
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }
}

