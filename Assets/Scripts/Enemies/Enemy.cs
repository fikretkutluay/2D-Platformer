using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class Enemy : MonoBehaviour
{
    protected Transform player;
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Collider2D[] colliders;

    
    [Header("General info")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float idleDuration = 1.5f;
    protected float idleTimer;
    protected bool canMove = true;



    [Header("Death Details")]
    [SerializeField] private float deathImpact = 5f;
    [SerializeField] private float deathRotationSpeed = 150f;
    private int deathRotationDirection = 1;
    protected bool isDead;


    [Header("Basic collision")]
    [SerializeField] protected float groundCheckDistance = 1.1f;
    [SerializeField] protected float wallCheckDistance =.7f;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float playerDetectionDistance = 15;
    protected bool isPlayerDetected;
    protected bool isGrounded;
    protected bool isWallDetected;
    protected bool isGroundInfrontDetected;

    protected int facingDir = -1;
    protected bool facingRight = false;


    protected virtual void Awake()
    {
       anim = GetComponent<Animator>();
       rb = GetComponent<Rigidbody2D>();
       colliders = GetComponentsInChildren<Collider2D>();

    }

    protected virtual void Start()
    {
        InvokeRepeating(nameof(UpdatePlayersRef), 0 , 1);
    }
    private void UpdatePlayersRef()
    {
        if (player == null)
            player = GameManager.instance.player.transform;
    }
    protected virtual void Update()
    {
        HandleCollision();
        HandleAnimator();
        idleTimer -= Time.deltaTime;

        if (isDead)
            HandleDeath();
    }

    public virtual void Die()
    {
        foreach (var collider in colliders)
        {
            collider.enabled = false;

        }
        anim.SetTrigger("hit");
        rb.velocity = new Vector2(rb.velocity.x, deathImpact);
        isDead = true;
        if (Random.Range(0, 100) < 50)
            deathRotationDirection = deathRotationDirection * -1;
    }

    private void HandleDeath()
    {
        transform.Rotate(0, 0, (deathRotationSpeed * deathRotationDirection) * Time.deltaTime);
    }

    protected virtual void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && facingRight || xValue > transform.position.x && !facingRight)
            Flip();
    }

    protected virtual void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    protected virtual void HandleAnimator()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
    }

    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isGroundInfrontDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        isPlayerDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, playerDetectionDistance, whatIsPlayer);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (playerDetectionDistance * facingDir), transform.position.y));

    }
}


