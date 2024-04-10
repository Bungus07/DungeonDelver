using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public GameObject[] Handles;
    private GameObject Player;
    private GameObject PlayersRightHand;
    private GameObject PlayersLeftHand;
    public int WeaponDamage;
    public float WeaponSpeed;
    private BoxCollider[] WeaponCollision;
    private LayerMask Ground;
    private LayerMask PlayerLayer;
    private Person2 PlayerScript;
    public Vector3 RotaionInRightHand;
    public Vector3 RotaionInLeftHand;
    public Vector3 PositionInRightHand;
    public Vector3 PositionInLeftHand;
    public enum WeaponType
    {
        Sword,
        Mace,
        Sheild,
        Bow,
        Crossbow
    }
    public WeaponType TheWeapon;
    public enum Elements 
    {
        None,
        Fire,
        Ice,
        Electric,
        Poison
    }
    public Elements WeaponElement;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        PlayerScript = Player.GetComponent<Person2>();
        PlayersRightHand = PlayerScript.PlayersRightHand;
        PlayersLeftHand = PlayerScript.PlayersLeftHand;
        WeaponCollision = gameObject.transform.GetComponentsInChildren<BoxCollider>();
        Ground = LayerMask.GetMask("Ground");
        PlayerLayer = LayerMask.GetMask("Player");
        foreach (var collider in WeaponCollision)
        {
            collider.excludeLayers = PlayerLayer;
        }
        gameObject.GetComponent<CapsuleCollider>().excludeLayers = LayerMask.GetMask("Default");
    }
    public void PickUp()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Destroy(rb);
        if (PlayerScript.EquipedWeaponRightHand == null)
        {
            transform.parent = PlayersRightHand.transform;
            transform.localPosition = PositionInRightHand;
            transform.localEulerAngles = RotaionInRightHand;
        }
        else if (PlayerScript.EquipedWeaponLeftHand == null)
        {
            transform.parent = PlayersLeftHand.transform;
            transform.localPosition = PositionInLeftHand;
            transform.localEulerAngles = RotaionInLeftHand;
        }
        else
        {
            Debug.Log("AllHandAreFull");
        }
        foreach (GameObject Handle in Handles)
        {
            Handle.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        foreach (var collider in WeaponCollision)
        {
            collider.excludeLayers = PlayerLayer + Ground;
        }
        Debug.Log("WeaponHasBeenEquiped");
    }
    public void Drop()
    {
        gameObject.AddComponent<Rigidbody>();
        transform.parent = null;
        foreach (GameObject Handle in Handles)
        {
            Handle.gameObject.GetComponent<BoxCollider>().enabled = true;
        }
        foreach (var collider in WeaponCollision)
        {
            collider.excludeLayers = PlayerLayer;
        }
    }
}
