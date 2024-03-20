using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Person2 : MonoBehaviour
{
    [Header ("Movement")]
    public float BaseSpeed;
    private float Speed;
    private Rigidbody ridge;
    private float Hoz;
    private float Vert;
    public float JumpForce; 
    private bool Grounded;
    public float Distance;
    private Vector3 Raycast;
    public LayerMask Ground;
    public float Sensitivity;
    private float Rotation;
    private Vector3 MoveDirection;
    public GameObject RespawnPlain;
    public GameObject RespawnPoint;
    private bool HasJumped;
    private bool IsInAir;
    public GameObject DoubleJumpCollectable;
    private bool HasCollectedDoubleJump;
    private GameManager gameManager;
    private HealthBar healthBarsript;
    private EnemyControls enemyscript;
    private Animator SwordAnimator;
    public int SwordDamage;
    // Start is called before the first frame update
    void Start()
    {
        ridge = GetComponent<Rigidbody>();
        Speed = BaseSpeed;
        Rotation = Input.GetAxisRaw("Mouse X");
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        healthBarsript = gameObject.GetComponent<HealthBar>();
        SwordAnimator = GameObject.Find("Hand").GetComponent<Animator>();
    }
    private void Attack()
    {
        SwordAnimator.SetTrigger("SwordSwing");
    }
    private void DamageTaken(int Amount, float Knockback)
    {
        Knockback = enemyscript.KnockBackDistance;
        Amount = enemyscript.EnemyDamage;
        healthBarsript.CurrentHealth = healthBarsript.CurrentHealth - Amount;
        ridge.AddForce(Vector3.back * Knockback, ForceMode.Force);
        healthBarsript.TheHealthBar.value = healthBarsript.CurrentHealth;
    }
    private void OnTriggerEnter(Collider Collider)
    {
        Debug.Log("CurrentlyTouching " + Collider.tag);
        if (Collider.tag == ("RespawnPlain"))
        {
            RespawnPlayer();
        }
        if (Collider.gameObject.tag == "Coin")
        {
            gameManager.coinsCounter += 1;
            Destroy(Collider.gameObject);
            Debug.Log("Player has collected a coin!");
        }
        if (Collider.gameObject.tag == ("EnemyDetectionRadious"))
        {
            Debug.Log("BeingDetected");
            Collider.gameObject.GetComponent<EnemyDetectionSphere>().Target.gameObject.GetComponent<EnemyControls>().PlayerDetect();
        }
    }
    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown (KeyCode.Space))
        {
            if (HasJumped == true && HasCollectedDoubleJump == true) 
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    DoubleJump();
                }
            }
            Jump(); 
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Sprint();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Walk();
        }
        GroundCheck();
        MoveDirection = transform.forward * Vert + transform.right * Hoz;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
        if (!Grounded)
        {
            ridge.drag = 0;
        }
        else if (Grounded)
        {
            ridge.drag = 5;
        }
    }
    private void FixedUpdate()
    {
        Hoz = Input.GetAxisRaw("Horizontal") * Speed;
        Vert = Input.GetAxisRaw("Vertical") * Speed;
        ridge.AddForce(MoveDirection, ForceMode.Force);
        Rotation = Input.GetAxisRaw("Mouse X") * Sensitivity;
        Quaternion deltaRotation = Quaternion.Euler(transform.rotation.x, Rotation, transform.rotation.z * Time.fixedDeltaTime);
        ridge.MoveRotation(ridge.rotation * deltaRotation);
    }
    
    private void Jump() 
    {
        
        if (Grounded == true) 
        {
            HasJumped = true;
            Debug.Log("HasJumped? " + HasJumped);
            ridge.AddForce(new Vector3(Hoz, JumpForce, Vert));
            Speed = BaseSpeed / 3;
        }
    }
    private void DoubleJump()
    {
        Debug.Log("DoubleJump");
        if (IsInAir == true && HasJumped == true)
        {
            ridge.AddForce(new Vector3(Hoz, JumpForce, Vert));
            HasJumped = false;
        }
    }


    private void Sprint()
    {
        if (Grounded)
        {
            Speed = Speed * 2;
            Debug.Log("is Sprinting");
        }
    }
    private void Walk()
    {
        Speed = BaseSpeed;
        Debug.Log("Walking");
    }
    private void GroundCheck()
    {

        Grounded = Physics.Raycast(transform.position, Vector3.down,Distance,Ground);
        Debug.DrawRay(transform.position, Vector3.down * Distance, Color.magenta);
        Debug.Log("Ray is touching " + Grounded);
        if (Grounded == false && HasJumped == true)
        {
            IsInAir = true;
        }
        else
        {
            IsInAir = false;
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == DoubleJumpCollectable)
        {
            HasCollectedDoubleJump = true;
            Object.Destroy(DoubleJumpCollectable);
        }
        if (collision.gameObject.layer == 6)
        {
            Speed = BaseSpeed;
            Grounded = true;
        }
        if (collision.gameObject.tag == ("Enemy"))
        {
            enemyscript = collision.gameObject.GetComponent<EnemyControls>();
            DamageTaken(enemyscript.EnemyDamage,enemyscript.KnockBackDistance);
        }
    }
    public void RespawnPlayer()
    {
        transform.position = RespawnPoint.transform.position;
        ridge.velocity = new Vector3(0,0, 0);
    }
}
