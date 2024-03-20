using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;
    public Slider TheHealthBar;
    private Person2 CharacterScript;
    private Rigidbody Ridge;
    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        TheHealthBar = GameObject.Find("PlayerHealth").GetComponent<Slider>();
        TheHealthBar.maxValue = MaxHealth;
        TheHealthBar.value = MaxHealth;
        CharacterScript = this.GetComponent<Person2>();
        Ridge = this.GetComponent<Rigidbody>();
    }
    public void TakeDamage(int Amount)
    {
        CurrentHealth = CurrentHealth - Amount;
        TheHealthBar.value = CurrentHealth;

    }
    public void Heal(int amount)
    {
        CurrentHealth = CurrentHealth + amount;
        TheHealthBar.value = CurrentHealth;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (CurrentHealth == 0)
        {
            CharacterScript.RespawnPlayer();
            CurrentHealth = MaxHealth;
            TheHealthBar.value = CurrentHealth;
        }
    }
}
