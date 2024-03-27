using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject[] Handles;
    private GameObject Player;
    private GameObject PlayersHand;
    public int WeaponDamage;
    public float WeaponSpeed;
    public enum WeaponType
    {
        Sword,
        Mace
    }
    public WeaponType TheWeapon;
    public enum Elements 
    {
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
        PlayersHand = Player.transform.GetChild(2).gameObject;
    }
    public void Equip()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Destroy(rb);
        transform.parent = PlayersHand.transform;
        transform.localPosition = new Vector3(-0.09f, 1.93f, 0.13f);
        transform.localEulerAngles = new Vector3(0.407f, 21.171f, 3.791f);
        foreach (GameObject Handle in Handles)
        {
            Handle.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
    public void UnEquip()
    {
        gameObject.AddComponent<Rigidbody>();
        transform.parent = null;
        foreach (GameObject Handle in Handles)
        {
            Handle.gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
