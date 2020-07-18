using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxHorizontalSpeed = 12f;
    public float maxVerticalSpeed = 16f;
    public float movementForce = 50f;
    public float jumpVelocity = 15f;
    public float jumpTime = 0.5f;

    [Header("Rigidbody")]
    public float mass = 0.75f;
    public float linearDrag = 1.5f;
    public float gravityScale = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundMask;

    [Header("Animators")]
    public RuntimeAnimatorController bigMarioAnimatorController;
    public RuntimeAnimatorController smallMarioAnimatorController;

    [Header("Death Mechanics")]
    public float invulnerabilityTime = 2f;
    public float deathHeight = -5f;                //change death height

    [Header("Sounds")]
    public AudioClip smallJumpSound;
    public AudioClip bigJumpSound;
    public AudioClip dyingSound;
    public bool isDyingSoundPlayed = false;

    private AudioSource audioSource;

    private bool isFacingRight = true;
    private bool isTouchingGround = true;

    [HideInInspector]
    public bool isJumping = false;

    [HideInInspector]
    public bool poweredUp = false;

    [HideInInspector]
    public bool isDead = false;

    [HideInInspector]
    public bool isInvulnerable = false;

    private float movementInput = 0f;
    private float jumpTimeCounter = 0f;
    private float invulnerabilityTimer = 0f;

    public Rigidbody2D playerRigidbody2D;

    private CapsuleCollider2D playerCapsuleCollider2D;

    private Animator playerAnimator;

    private SpriteRenderer playerSpriteRenderer;

    private PlayerController instance = null;

    [SerializeField]
    public GameObject firework;

    private bool takeAwayControll = true; //taking away control so Mario would not stick to the side

    public bool isTouchingFlagpole = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerRigidbody2D = GetComponent<Rigidbody2D>();
        playerRigidbody2D.mass = mass;
        playerRigidbody2D.drag = linearDrag;
        playerRigidbody2D.gravityScale = gravityScale;
        playerCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
    }


    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, new Vector2(0.4f, 0.1f), 0f, Vector2.down, groundCheckRadius, groundMask); //using this for a bigger and more accurate ground check
        isTouchingGround = (hit.collider != null) ? true : false;

        movementInput = Input.GetAxis("Horizontal");
        CheckIfStuck(); //Checks if Mario is trying to walk into the wall and get stuck

        if (!isDead)
        {
            if ((playerRigidbody2D.velocity.x > 0 && !isFacingRight) || (playerRigidbody2D.velocity.x < 0 && isFacingRight))
            {
                playerAnimator.SetBool("turning", true);
            }
            else
            {
                playerAnimator.SetBool("turning", false);
            }

            float movementForceMultiplier = Mathf.Max(maxHorizontalSpeed - Mathf.Abs(playerRigidbody2D.velocity.x), 1);

            playerRigidbody2D.AddForce(new Vector2(movementInput * movementForce * movementForceMultiplier, 0));

            playerRigidbody2D.velocity = new Vector2(Mathf.Clamp(playerRigidbody2D.velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed), Mathf.Clamp(playerRigidbody2D.velocity.y, -maxVerticalSpeed, maxVerticalSpeed));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTouchingGround)
                {
                    //Play Jump sound
                    if (!poweredUp)
                        audioSource.PlayOneShot(smallJumpSound);
                    else
                        audioSource.PlayOneShot(bigJumpSound);

                    isJumping = true;
                    playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, jumpVelocity);
                    jumpTimeCounter = jumpTime;
                }
            }

            if (jumpTimeCounter > 0 && isJumping)
                if (Input.GetKey(KeyCode.Space))
                {
                    jumpTimeCounter -= Time.deltaTime;
                    {
                        playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, jumpVelocity);
                    }
                }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
                jumpTimeCounter = 0;
            }

            playerAnimator.SetFloat("movementSpeed", Mathf.Abs(playerRigidbody2D.velocity.x));
            playerAnimator.SetBool("touchingGround", isTouchingGround);
        }

        if (movementInput > 0 && !isFacingRight)
        {
            FlipSprite();
        }
        else if (movementInput < 0 && isFacingRight)
        {
            FlipSprite();
        }
        if (playerRigidbody2D.position.y < deathHeight) //added death method
        {
            Die();
        }
    }


    private void Update()
    {
        
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;

            float newAlpha = 1f;

            if (playerSpriteRenderer.color.a > 0.51f)
            {
                newAlpha = 0.5f;
            }


            if (invulnerabilityTimer < 0)
            {
                isInvulnerable = false;
                newAlpha = 1f;
            }


            playerSpriteRenderer.color = new Color(playerSpriteRenderer.color.r, playerSpriteRenderer.color.g, playerSpriteRenderer.color.b, newAlpha);

            if (playerRigidbody2D.position.y < deathHeight) //added death method
            {
                SceneManager.LoadScene(1);
            }
        }
    }

    public void PowerUp()
    {
        if (!poweredUp)
        {
            playerAnimator.runtimeAnimatorController = bigMarioAnimatorController as RuntimeAnimatorController;
            playerCapsuleCollider2D.offset = new Vector2(0, 0.5f);
            playerCapsuleCollider2D.size = new Vector2(0.9f, 2);
            poweredUp = true;
        }

    }

    public void Die()
    {
        if (poweredUp && !isDead && !isInvulnerable)
        {
            playerAnimator.runtimeAnimatorController = smallMarioAnimatorController as RuntimeAnimatorController;
            playerCapsuleCollider2D.offset = new Vector2(0, 0f);
            playerCapsuleCollider2D.size = new Vector2(1, 1);
            poweredUp = false;
            isInvulnerable = true;
            invulnerabilityTimer = invulnerabilityTime;
        }
        else if (!isInvulnerable)
        {
            playerRigidbody2D.velocity = new Vector2(0, jumpVelocity);
            playerAnimator.SetBool("dead", true);
            playerCapsuleCollider2D.enabled = false;
            isDead = true;
            RestartTheGame();
        }

    }
    private void RestartTheGame()
    {
        StartCoroutine(LoadStartingScene());
    }
    private IEnumerator LoadStartingScene()
    {
        if (!isDyingSoundPlayed)
        {
            audioSource.PlayOneShot(dyingSound);
            isDyingSoundPlayed = true;
        }
        deathHeight = -10f;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        playerCapsuleCollider2D.enabled = true;
        playerAnimator.SetBool("dead", false);
        isDead = false;
        deathHeight = -5f;

    }
    private void FlipSprite()
    {
        isFacingRight = !isFacingRight;
        Vector3 tempScale = transform.localScale;
        tempScale.x *= -1;
        transform.localScale = tempScale;
    }

    private void CheckIfStuck()
    {
        //Taking away users control when player is not touching the ground and not moving to any direction or touching flag pole
        if ((!isTouchingGround && playerRigidbody2D.velocity == Vector2.zero) || isTouchingFlagpole)
            takeAwayControll = true;

        if (takeAwayControll)
            movementInput = 0;

        //if starts touching ground - give control back
        if (isTouchingGround && !isTouchingFlagpole)
        {
            takeAwayControll = false;
        }
        if(isTouchingFlagpole && isTouchingGround && playerRigidbody2D.velocity.x != 2f)
        {
            playerRigidbody2D.velocity = new Vector2(Mathf.Clamp(2f, -maxHorizontalSpeed, maxHorizontalSpeed), Mathf.Clamp(0, -maxVerticalSpeed, maxVerticalSpeed));
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        takeAwayControll = false; //give back control when it's no longer colliding with anything
    }
    public bool GetIsTouchingGround()
    {
        return isTouchingGround;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(isTouchingFlagpole && collision.gameObject.tag == "Castle")
        {
            collision.gameObject.GetComponent<TilemapCollider2D>().enabled = false;
        }
          if (isTouchingFlagpole && collision.gameObject.tag == "FinishingBrick")
        {
            FinishingCastleFlagMover finishingFlagMover = FindObjectOfType<FinishingCastleFlagMover>();
            finishingFlagMover.Move();
            playerSpriteRenderer.enabled = false;
            int fireworksNumber = (int)FindObjectOfType<ScoreManager>().GetCurrentTime() % 10;
            FireworkController fireworkController = new FireworkController();
            StartCoroutine(fireworkController.SpawnFireworks(fireworksNumber,firework));
        }
    }
}

