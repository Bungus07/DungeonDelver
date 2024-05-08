using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

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
    private Animator PlayerAnimator;
    public int WeaponDamage;
    [Header("Weapon Stuff")]
    private GameObject BelowFeet;
    public GameObject EquipedWeaponRightHand;
    public GameObject EquipedWeaponLeftHand;
    [Header("Rolling")]
    public bool IsRolling;
    private float RollingAngle = 0;
    public float RollForce;
    public float RollSpeed;
    private Vector3 CurrentMoveDirectionInputs;
    public BoxCollider[] PlayerCollision;
    public GameObject Pivot;
    [Header("RightHandAttacking")]
    public GameObject PlayersRightHand;
    public GameObject PlayersLeftHand;
    private bool NoRotate;
    [Header("Levels")]
    public int PlayerLevel = 1;
    public int Vitality;
    public int Strength;
    public int StrenghtLevel;
    public int Agility;
    private int AgilityLevel;
    private TextMeshProUGUI HpText;
    private TextMeshProUGUI StrengthText;
    private TextMeshProUGUI AgilityText;
    private int AttackStat;
    private int GainsLevel;
    public GameObject LevelPanel;
    public int PlayersTotalSouls;
    public float SoulsForOneLevel;
    private float SoulsCost;
    private int PlayersUnspentLevel;
    private int PlayersUndecidedLevel;
    public TextMeshProUGUI PlayersLevelText;
    private bool LevelUIIsOpen;
    private TextMeshProUGUI VitalityAfterLevel;
    private TextMeshProUGUI StrenghtAfterLevel;
    private TextMeshProUGUI AgilityAfterLevel;
    void Start()
    {
        ridge = GetComponent<Rigidbody>();
        Speed = BaseSpeed;
        Rotation = Input.GetAxisRaw("Mouse X");
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        healthBarsript = gameObject.GetComponent<HealthBar>();
        PlayerAnimator = gameObject.GetComponent<Animator>();
        ridge.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        NoRotate = true;
        HpText = GameObject.Find("Hp").GetComponent<TextMeshProUGUI>();
        StrengthText = GameObject.Find("Strength").GetComponent<TextMeshProUGUI>();
        AgilityText = GameObject.Find("Agility").GetComponent<TextMeshProUGUI>();
        HpText.text = "Hp " + gameObject.GetComponent<HealthBar>().CurrentHealth;
        StrengthText.text = "Strength " + AttackStat;
        AgilityText.text = "Agility " + BaseSpeed;
        VitalityAfterLevel = GameObject.Find("HpAfterLevel").GetComponent<TextMeshProUGUI>();
        StrenghtAfterLevel = GameObject.Find("AttackAfterLevel").GetComponent<TextMeshProUGUI>();
        AgilityAfterLevel = GameObject.Find("SpeedAfterLevel").GetComponent<TextMeshProUGUI>();
        RefreshLevels();
    }
    private void RefreshLevels()
    {
        HpText.text = "Vitality Level = " + Vitality;
        VitalityAfterLevel.text = "Hp = " + healthBarsript.MaxHealth;

        StrengthText.text = "Strength Level = " + StrenghtLevel;
        StrenghtAfterLevel.text = "Strength = " + Strength;

        AgilityText.text = "Agility Level = " + AgilityLevel;
        AgilityAfterLevel.text = "Agility = " + Agility;
    }
    public void Death()
    {
        Debug.Log("Player Died");
        healthBarsript.CurrentHealth = healthBarsript.MaxHealth;
        PlayersTotalSouls = 0;
        RespawnPlayer();
    }
    public void LevelUpVitality()
    {
        if (PlayersUnspentLevel > 0)
        {
            TextMeshProUGUI HpAfterLevel = GameObject.Find("HpAfterLevel").GetComponent<TextMeshProUGUI>();
            gameObject.GetComponent<HealthBar>().CurrentHealth += 2;
            gameObject.GetComponent<HealthBar>().MaxHealth += 2;
            HpAfterLevel.text = gameObject.GetComponent<HealthBar>().CurrentHealth.ToString();
            gameObject.GetComponent<HealthBar>().TheHealthBar.maxValue = gameObject.GetComponent<HealthBar>().MaxHealth += 2;
            gameObject.GetComponent<HealthBar>().TheHealthBar.value = gameObject.GetComponent<HealthBar>().CurrentHealth += 2;
            PlayersUnspentLevel--;
            PlayersUndecidedLevel++;
            PlayerLevel++;
            PlayersLevelText.text = "PlayerLevel " + PlayerLevel;
            RefreshLevels();
        }
        else if (PlayersUnspentLevel !> 0)
        {
            Debug.Log("You Need More Souls");
        }
    }
    public void LevelDownVitality()
    {
        if (PlayersUndecidedLevel > 0)
        {
            gameObject.GetComponent<HealthBar>().CurrentHealth -= 2;
            gameObject.GetComponent<HealthBar>().MaxHealth -= 2;
            PlayersUnspentLevel++;
            PlayersUndecidedLevel--;
            PlayerLevel--;
            gameObject.GetComponent<HealthBar>().TheHealthBar.maxValue = gameObject.GetComponent<HealthBar>().MaxHealth -= 2;
            gameObject.GetComponent<HealthBar>().TheHealthBar.value = gameObject.GetComponent<HealthBar>().CurrentHealth -= 2;
            PlayersLevelText.text = "PlayerLevel " + PlayerLevel;
            RefreshLevels();
        }
        else if (PlayersUndecidedLevel !> 0)
        {
            Debug.Log("You havent got any undecided levels");
        }
    }
    public void LevelUpStrength()
    {
        if (PlayersUnspentLevel > 0)
        {
            StrenghtLevel += 1;
            PlayersUnspentLevel--;
            PlayersUndecidedLevel++;
            Strength += 1;
            PlayerLevel++;
            PlayersLevelText.text = "PlayerLevel " + PlayerLevel;
            RefreshLevels();
        }
        else if (PlayersUnspentLevel! > 0)
        {
            Debug.Log("You need More Souls");
        }
    }
    public void LevelDownStrength()
    {
        if (PlayersUndecidedLevel > 0)
        {
            Strength -= 1;
            StrenghtLevel -= 1;
            PlayersUnspentLevel++;
            PlayersUndecidedLevel--;
            PlayerLevel--;
            PlayersLevelText.text = "PlayerLevel " + PlayerLevel;
            RefreshLevels();
        }
        else if (PlayersUndecidedLevel! > 0)
        {
            Debug.Log("You havent got any undecided levels");
        }
    }
    public void LevelUpAgility()
    {
        if (PlayersUnspentLevel > 0)
        {
            AgilityLevel += 1;
            PlayersUnspentLevel--;
            PlayersUndecidedLevel++;
            Agility += 2;
            BaseSpeed += Agility;
            Speed = BaseSpeed;
            PlayerLevel++;
            PlayersLevelText.text = "PlayerLevel " + PlayerLevel;
            RefreshLevels();
        }
        else if (PlayersUnspentLevel! > 0)
        {
            Debug.Log("You need More Souls");
        }
    }
    public void LevelDownAgility()
    {
        if (PlayersUndecidedLevel > 0)
        {
            AgilityLevel -= 1;
            PlayersUnspentLevel++;
            PlayersUndecidedLevel--;
            BaseSpeed -= Agility;
            Agility -= 2;
            Speed = BaseSpeed;
            PlayerLevel--;
            PlayersLevelText.text = "PlayerLevel " + PlayerLevel;
            RefreshLevels();
        }
        else if (PlayersUndecidedLevel! > 0)
        {
            Debug.Log("You havent got any undecided levels");
        }
    }
    public void ConfirmLevel()
    {
        if (PlayersUnspentLevel == 0)
        {
            PlayersUndecidedLevel = 0;
            RefreshLevels();
        }
    }
    public void CheckForLevels()
    {
        if (PlayersTotalSouls >= SoulsForOneLevel) 
        {
            PlayersUnspentLevel++;
            SoulsCost = SoulsForOneLevel;
            SoulsForOneLevel = SoulsCost * 1.1f;
        }
    }
    private void RightHandAttack()
    {
        if (EquipedWeaponRightHand != null)
        {
            PlayerAnimator.SetTrigger("RightHandSwing");
            WeaponDamage = EquipedWeaponRightHand.GetComponent<Weapon>().WeaponDamage;
        }
    }
    private void LeftHandAttack()
    {
        if (EquipedWeaponLeftHand != null)
        {
            PlayerAnimator.SetTrigger("LeftHandSwing");
            WeaponDamage = EquipedWeaponLeftHand.GetComponent<Weapon>().WeaponDamage;
        }
    }
    private void Roll()
    {
        if (Grounded)
        {
            CurrentMoveDirectionInputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            ridge.velocity = Vector3.zero;
            if (CurrentMoveDirectionInputs == Vector3.zero)
            {
                CurrentMoveDirectionInputs = Vector3.forward;
            }
            IsRolling = true;
            Debug.Log("ShouldBeRolling");
            ridge.constraints = RigidbodyConstraints.None;
            NoRotate = false;
        }
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
        if (Collider.gameObject.tag == ("Weapon"))
        {
            BelowFeet = Collider.gameObject;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == ("Weapon"))
        {
            BelowFeet = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            if (LevelPanel.activeSelf == true)
            {
                LevelPanel.SetActive(false);
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }
            else
            {
                LevelPanel.SetActive(true);
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            BelowFeet.GetComponent<Weapon>().PickUp();
            if (EquipedWeaponRightHand == null)
            {
                EquipedWeaponRightHand = BelowFeet;

                BelowFeet = null;
            }
            else if (EquipedWeaponLeftHand == null)
            {
                EquipedWeaponLeftHand = BelowFeet;
                
                BelowFeet = null;
            }
            else
            {
                Debug.Log("HandsAreFull");
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (EquipedWeaponRightHand != null)
            {
                EquipedWeaponRightHand.GetComponent<Weapon>().Drop();
                EquipedWeaponRightHand = null;
            }
        }
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
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RightHandAttack();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            LeftHandAttack();
        }
        if (!Grounded)
        {
            ridge.drag = 0;
        }
        else if (Grounded)
        {
            ridge.drag = 5;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Roll();
        }
        if (IsRolling)
        {
            RollingAngle += (RollSpeed + 1);
            ridge.useGravity = false;
            ridge.velocity = ((CurrentMoveDirectionInputs.x * transform.right + CurrentMoveDirectionInputs.z * transform.forward) * RollForce);
            if (CurrentMoveDirectionInputs.z != 0 && CurrentMoveDirectionInputs.z != -1) //Fowards
            {
                Pivot.transform.Rotate(CurrentMoveDirectionInputs.z + RollSpeed, 0, -CurrentMoveDirectionInputs.x);
            }
            else if (CurrentMoveDirectionInputs.x != 0 && CurrentMoveDirectionInputs.x != -1) //Right
            {
                Pivot.transform.Rotate(CurrentMoveDirectionInputs.z, 0, -(CurrentMoveDirectionInputs.x + RollSpeed));
            }
            else if (CurrentMoveDirectionInputs.z != 0 && CurrentMoveDirectionInputs.z == -1) //Backwards
            {
                Pivot.transform.Rotate(-(CurrentMoveDirectionInputs.z + (RollSpeed + 2)), 0, -CurrentMoveDirectionInputs.x);
            }
            else if (CurrentMoveDirectionInputs.x != 0 && CurrentMoveDirectionInputs.x == -1) //Left
            {
                Pivot.transform.Rotate(CurrentMoveDirectionInputs.z, 0, CurrentMoveDirectionInputs.x + (RollSpeed + 2));
            }
            foreach (BoxCollider boxCollider in PlayerCollision)
            {
                boxCollider.enabled = false;
            }
            if (RollingAngle >= 360) 
            {
                Pivot.transform.localEulerAngles = new Vector3 (0, Pivot.transform.localEulerAngles.y, 0);
                IsRolling = false;
                RollingAngle = 0;
                ridge.useGravity = true;
                ridge.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                NoRotate = true;
                foreach (BoxCollider boxCollider in PlayerCollision)
                {
                    boxCollider.enabled = true;
                }
            }
        }
        if (NoRotate == true)
        {
            gameObject.transform.localEulerAngles = new Vector3(0, gameObject.transform.localEulerAngles.y, 0);
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
        if (!IsRolling)
        {
            ridge.MoveRotation(ridge.rotation * deltaRotation);
        }
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
