using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    
    private Animator animator;

    [Header("Move & Jump Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float doubleJumpForce = 8f;
    [SerializeField] private float fallMultiplier = 2.5f; // Faster fall
    [SerializeField] private float lowJumpMultiplier = 2f; // Short jump
    [SerializeField] private int maxJumps = 3; // Total allowed jumps (1 ground + 2 air)

    [Header("Duck Settings")]
    [SerializeField] private float duckSpeed = 2.5f;
    [SerializeField] private Vector2 duckColliderSize = new Vector2(1f, 0.5f);
    [SerializeField] private Vector2 normalColliderSize = new Vector2(1f, 1f);

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 15f;
    
    [Header("Weight Impact Settings")]
    [SerializeField] private float speedReductionFactor = 3f; // Speed reduction per collectible
    [SerializeField] private float jumpReductionFactor = 0.5f; // Jump reduction per collectible
    [SerializeField] private float minMoveSpeed = 2f; // Minimum allowed speed
    [SerializeField] private float minJumpForce = 5f;
    
    [Header("Collectibles Settings")]
    [SerializeField] private int totalCollectibles = 5;
    [SerializeField] private TextMeshProUGUI collectiblesCounterText;

    [Header("Lives")] 
    [SerializeField] private GameObject[] lifeIcons;
    private int lifeCounter;
    [SerializeField] private TextMeshProUGUI collectibleIndication;

    private Rigidbody2D player;
    private BoxCollider2D boxCollider;
    private CanvasGroup collectibleCanvasGroup;
    private bool isGrounded;
    private bool isDucking = false;
    private bool isDashing = false;
    private bool canDash = true;
    
    private int collectedCount = 0;

    private int jumpCount = 0; 

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

        player = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        collectibleCanvasGroup = GetComponent<CanvasGroup>();
        

        if (player == null)
            Debug.LogWarning("PlayerController: Rigidbody2D component is missing.");
        if (boxCollider == null)
            Debug.LogWarning("PlayerController: BoxCollider2D component is missing.");
        if (animator == null)
            Debug.Log("PlayerController: Animator component is missing.");
        if (collectibleCanvasGroup == null)
            Debug.Log("Player Controller: CanvasGroup component is missing");
    }

    private void Start()
    {
        UpdateCollectiblesUI();
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
        float targetSpeed = moveInput * speed;

        float smoothSpeed = Mathf.Lerp(player.velocity.x, targetSpeed, 0.2f); // Smoothly adjusted velocity using Mathf.Lerp
        player.velocity = new Vector2(smoothSpeed, player.velocity.y);

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(6f, 6f, 2); // Face right
            animator.SetBool("isRunning", true);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-6f, 6f, 2); // Face left
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            jumpCount++;
            animator.SetBool("isJumping", true);
            if (jumpCount == 1) // Ground jump
            {
                player.velocity = new Vector2(player.velocity.x, jumpForce);
            }
            else if (jumpCount == 2) // Double jump
            {
                player.velocity = new Vector2(player.velocity.x, doubleJumpForce);
            }
            else
            {
                player.velocity = new Vector2(player.velocity.x, doubleJumpForce);
            }
        }

        // Apply fall multiplier for faster fall and better jump feel
        if (player.velocity.y < 0)
        {
            player.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (player.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            player.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        
        if (isGrounded)
        {
            animator.SetBool("isJumping", false);
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
                animator.SetBool("isDucking", true);
            }
        }
        else
        {
            if (isDucking)
            {
                isDucking = false;
                boxCollider.size = normalColliderSize;
                animator.SetBool("isDucking", false);
            }
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDucking)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float originalGravity = player.gravityScale;
        player.gravityScale = 0; // Temporarily disabling gravity
        player.velocity = new Vector2(dashSpeed * transform.localScale.x, 0); // Dash direction based on facing

        yield return new WaitForSeconds(dashDuration);

        player.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0; // Reset jump count when grounded
        }
        if (collision.gameObject.CompareTag("DisappearingPlatform"))
        {
            Debug.Log("Did we reach here");
            PlatformDisappear(collision.gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lava"))
        {
            HandleLavaCollision();
        }
        if (collision.CompareTag("Collectible"))
        {
            Collect(collision.gameObject);
        }
    }
    
    private bool IsPlayerFeetTouchingPlatform(Collider2D platformCollider)
    {
        BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();

        if (playerCollider != null)
        {
            Bounds playerBounds = playerCollider.bounds;
            float playerBottomEdge = playerBounds.min.y; 
            
            Bounds platformBounds = platformCollider.bounds;
            float platformTopEdge = platformBounds.max.y; 
            
            return Mathf.Approximately(playerBottomEdge, platformTopEdge) || 
                   (playerBottomEdge > platformTopEdge - 0.1f && playerBottomEdge < platformTopEdge + 0.1f);
        }

        return false; // Default to false if no player collider is found
    }


    private void PlatformDisappear(GameObject disappearingPlatform)
    {
        Debug.Log("Did we reach here 2");
        BoxCollider2D boxCollider = disappearingPlatform.GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Debug.Log("did we reach here 3");
            // Switch to trigger mode
            boxCollider.isTrigger = true;
            Debug.Log($"istrigger: {boxCollider.isTrigger}");
        }
        
        SpriteRenderer spriteRenderer = disappearingPlatform.GetComponent<SpriteRenderer>();
    
        if (spriteRenderer != null)
        {
            spriteRenderer.DOFade(0f, 2f).SetDelay(3f).OnComplete(() =>
            {
                Destroy(disappearingPlatform); // Destroy the collectible after fading out
            });
        }
        else
        {
            // If no SpriteRenderer is found, destroy it immediately
            Debug.LogWarning("Collectible does not have a SpriteRenderer component. Destroying it directly.");
            Destroy(disappearingPlatform);
        }
    }
    
    
    
    private void Collect(GameObject collectible)
    {
        collectedCount++; // Increment collected count
        UpdateCollectiblesUI(); 
        Destroy(collectible); // Remove the collectible from the scene
        
        MoveSpeed = Mathf.Max(minMoveSpeed, MoveSpeed - speedReductionFactor);
        JumpForce = Mathf.Max(minJumpForce, JumpForce - jumpReductionFactor);
        collectibleIndication.gameObject.SetActive(true); 
        StartCoroutine(FadeOutText());

        Debug.Log($"Collectible collected! New Move Speed: {MoveSpeed}, New Jump Force: {JumpForce}");
    }

    private void UpdateCollectiblesUI()
    {
        collectiblesCounterText.text = $"{collectedCount}/{totalCollectibles}";
    }
    private IEnumerator FadeOutText()
    {
        collectibleCanvasGroup.alpha = 1f; 
        yield return new WaitForSeconds(1f); 

        float fadeDuration = 1f; 
        float fadeSpeed = 1f / fadeDuration;

        while (collectibleCanvasGroup.alpha > 0)
        {
            collectibleCanvasGroup.alpha -= fadeSpeed * Time.deltaTime; // Gradually reduce alpha
            yield return null; 
        }

        collectibleIndication.gameObject.SetActive(false);
    }
    private void HandleLavaCollision()
    {
        Debug.Log("Are we reaching here");
        if (lifeCounter > 0)
        {
            lifeCounter--; 
            UpdateLivesUI(); // Update the UI to reflect remaining lives

            if (lifeCounter <= 0)
            {
                GameTimer.Instance.TimeUp(); //Show reset button if lives are exhausted
            }
        }

        Debug.Log($"Player hit lava! Lives remaining: {lifeCounter}");
    }
    
    private void UpdateLivesUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].SetActive(i < lifeCounter); // Enable icons for remaining lives
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void SetPlayerInactive()
    {
        player.gameObject.SetActive(false);
    }
}
