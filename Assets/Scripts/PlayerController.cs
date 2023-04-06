using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpHeight = 20f;
    [SerializeField] float climbingSpeed = 5f;
    [SerializeField] public bool isAlive = true;
    Rigidbody2D rb;
    Animator myAnimator;
    BoxCollider2D myFeetCollider;
    CapsuleCollider2D myBodyCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        if (isAlive == false) return;
        FlipTheSprite();
        Jump();
        Run();
        ClimbLadder();
        Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyMovement>())
        {
            isAlive = false;
            myAnimator.SetTrigger("Die");
            Invoke("loadNextScene", 1.2f);
        }
    }

    private void Die()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Die");
            Invoke("loadNextScene", 1.2f);
        }
    }

    private void loadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FlipTheSprite()
    {
        bool playerHadHorizontalSpeed = MathF.Abs(rb.velocity.x) > Mathf.Epsilon;

        if (playerHadHorizontalSpeed)
        {
            transform.localScale = new Vector2(MathF.Sign(rb.velocity.x), 1f);
        }
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(Input.GetAxis("Horizontal") * runSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;

        bool playerHadHorizontalSpeed = MathF.Abs(rb.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("IsRunning", playerHadHorizontalSpeed);

    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && myFeetCollider.IsTouchingLayers(LayerMask.GetMask("ForeGround")))
        {
            rb.velocity += new Vector2(0f, jumpHeight);
        }
    }

    void ClimbLadder()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ladders")))
        {
            rb.gravityScale = 0;

            Vector2 climbSpeed = new Vector2(rb.velocity.x, Input.GetAxis("Vertical") * climbingSpeed);
            rb.velocity = climbSpeed;

            bool playerHadHVerticalSpeed = MathF.Abs(rb.velocity.y) > Mathf.Epsilon;
            myAnimator.SetBool("IsClimbing", playerHadHVerticalSpeed);
        }
        else
        {
            rb.gravityScale = 5f;
            myAnimator.SetBool("IsClimbing", false);
        }
    }
}
