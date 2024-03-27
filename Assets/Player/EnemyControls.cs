using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyControls : MonoBehaviour
{
    public float BaseSpeed = 2f;
    private float Speed;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask wall;
   // public Collider triggerCollider;
    private Rigidbody rb;
    private bool isGrounded = false;
    private bool isFacingFowards = true;
    private float FlipCooldown = 0;
    private bool HasFliped;
    public int EnemyHealth;
    private Slider EnemyHealthBar;
    public GameObject HealingOrb;
    private Transform WhereDied;
    public float Distance;
    public float KnockBackDistance;
    public int EnemyDamage;
    private Person2 PlayerScript;
    [Header("DetectionRadious")]
    public Collider DetectionRadious;
    private GameObject Player;
    private float LundgeTimer;
    public float LundgeIntervul;
    private bool IsLundging;
    private bool LundgeTimerActivated;
    public float LundgeDistance;
    private float LundgeingTimer;
    public float LundingInterval;
    private Vector3 LundgePosition;
    private Vector3 MoveDirection;
    private float ReturnPostimer;
    public float ReturnPostimerInterval;
    private Vector3 EnemyOrigonalPosition;
    private Quaternion EnemyOrigonalRotation;

    public enum EnemyState
    {
        [Header("AIstates")]
        PatrolArea,
        Lundge,
        Stop,
        ReturnToPosition
    }
    public EnemyState CurrentAction;
    [Header("AI")]
    private Vector3 PlayerCurrentPosition;
    private Vector3 ChargePosition;
    // private Vector3 MoveDirection;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        PlayerScript = GameObject.FindWithTag("Player").GetComponent<Person2>();
        EnemyHealthBar = gameObject.GetComponentInChildren<Slider>();
        EnemyHealthBar.maxValue = EnemyHealth;
        EnemyHealthBar.value = EnemyHealth;
        Player = GameObject.FindWithTag("Player");
        EnemyOrigonalPosition = gameObject.transform.position;
        EnemyOrigonalRotation = gameObject.transform.rotation;
        Speed = BaseSpeed;
    }
    private void FixedUpdate()
    {
    }
    private void Update()
    {
        // Check if the enemy is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, Distance, groundLayer);
        // Move the enemy horizontally
        // Check if the enemy should jump
        if (CurrentAction == EnemyState.Stop)
        {
            rb.velocity = Vector3.zero;
            LundgeTimerActivated = false;
            LundgeTimer = 0;
            ReturnPostimer += Time.deltaTime;
            if (ReturnPostimer >= ReturnPostimerInterval)
            {
                CurrentAction = EnemyState.ReturnToPosition;
            }
        }
        if (CurrentAction == EnemyState.ReturnToPosition)
        {
            transform.LookAt(EnemyOrigonalPosition);
            rb.velocity = transform.forward * BaseSpeed;
            if (Vector3.Distance(gameObject.transform.position, EnemyOrigonalPosition) < 0.2f)
            {
                gameObject.transform.rotation = EnemyOrigonalRotation;
                CurrentAction = EnemyState.PatrolArea;
            }
        }
        if (isGrounded)
        {
            //Jump();
        }
        if (EnemyHealth <= 0) 
        {
            EnemyDeath();
        }
        if (CurrentAction == EnemyState.PatrolArea)
        {
            Move();
        }
        if (CurrentAction == EnemyState.Lundge)
        {
            if (isGrounded)
            {
                PlayerCurrentPosition = Player.transform.position;
                Vector3 PlayerPositionNotY = new Vector3(Player.transform.position.x,gameObject.transform.position.y,Player.transform.position.z);
                LundgeTimer += Time.deltaTime;
                if (IsLundging == true)
                {
                    LundgeingTimer += Time.deltaTime;
                    Debug.Log("LundgingTimerIsIncreasing");
                    if (LundgeingTimer >= LundingInterval)
                    {
                        MoveDirection = transform.forward;
                        rb.velocity = MoveDirection * BaseSpeed;
                        Debug.Log("LundgeTimer=LundgeInterval");
                        if (Vector3.Distance(gameObject.transform.position, LundgePosition) < 1.2f)
                        {
                            Debug.Log("ShouldStopLunding");
                            LundgeingTimer = 0;
                            IsLundging = false;
                            CurrentAction = EnemyState.Stop;
                        }
                    }
                }
                else if (IsLundging == false)
                {
                    transform.LookAt(PlayerPositionNotY);
                    if (LundgeTimer >= LundgeIntervul)
                    {
                        Lundge(Player.transform);
                    }
                }
            }
        } 
    }
    private void Lundge(Transform Target)
    {
        Debug.Log("EnemyShouldBeLundgeing");
        LundgePosition = new Vector3(Target.position.x, 0,Target.position.z);
        DebugExtension.DebugWireSphere(LundgePosition, 1, 10, true);
        Vector3 LundgeRotaion = Target.transform.position;
        transform.LookAt(LundgeRotaion);
        IsLundging = true;
        LundgeTimer = 0;
    }
    public void DamageEnemy(int DamageAmount)
    {
        EnemyHealth = EnemyHealth - DamageAmount;
        EnemyHealthBar.value = EnemyHealth;
    }
    public void PlayerDetect()
    {
        CurrentAction = EnemyState.Lundge;
        LundgeTimerActivated = true;
        Debug.Log("PlayerHasBeenDetected");
    }
    private void Move()
    {
        float horizontalInput = isFacingFowards ? 1f : -1f;
        Vector3 movement = new Vector3(horizontalInput * BaseSpeed, rb.velocity.y);
        rb.velocity = movement;
        Debug.Log("EnemyShouldBeMoving");
        // Flip the enemy sprite based on direction
        if ((horizontalInput > 0 && !isFacingFowards) || (horizontalInput < 0 && isFacingFowards))
        {
            Flip();
        }
    }
    private void Jump()
    {
        // Add vertical force to make the enemy jump
        // rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void Flip()
    {
        // Flip the enemy's facing direction
        if (FlipCooldown == 0 )
        {
            isFacingFowards = !isFacingFowards;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            FlipCooldown = 1;
            Invoke("ResetFlip", 0.3f);
        }
    }
    private void ResetFlip()
    {
        FlipCooldown = 0;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == ("Weapon"))
        {
            DamageEnemy(PlayerScript.WeaponDamage);
            Debug.Log("EnemyHasTakenDamage");
        }
    }
    public void EnemyDeath()
    {
        WhereDied = gameObject.transform;
        Instantiate(HealingOrb, WhereDied.position, Quaternion.identity);
        GameObject.Destroy(gameObject);
        Debug.Log("HasDroppedOrb");
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision with the player or other objects
        if (collision.gameObject.CompareTag("Player"))
        {
            // Implement your combat logic here (e.g., reducing player's health)
            // You may want to use events or other mechanisms to communicate with the player script.
        }
        if (collision.gameObject.tag == ("Wall"))
        {
            Flip();
        }
    }
}