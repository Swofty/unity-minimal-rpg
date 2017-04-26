using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Attack
{
    public float Damage;
    public bool IsCrit;
}

public class Weapon
{
    public int AttackDamage;
    public string Name;
    public float CritChance;
}

public class Player {

    public int Level = 1;
    public int Strength = 10;
    public float CritChance = 0.0f;
    public List<Weapon> Weapons = new List<Weapon>();
    public int Exp = 0;
    public int NextExpThreshold = 1;

    public const float DAMAGE_VARIANCE = 0.1f;
    public const float CRIT_MODIFIER = 2.0f;
    
    public Player()
    {
        Level = 1;
        Strength = 10;
        CritChance = 0.0f;
        Weapons.Add(new Weapon() { AttackDamage = 10, Name = "Stick", CritChance = 0.0f });
        Exp = 0;
        NextExpThreshold = 1;
    }

    public void LevelUp()
    {
        Level++;
        Strength += 2;
        CritChance += 0.01f;
        NextExpThreshold += Level;
    }

    public Weapon GetCurrentWeapon()
    {
        return Weapons[0];
    }
    
    public bool GainExp(int exp)
    {
        Exp += exp;
        if(Exp >= NextExpThreshold)
        {
            LevelUp();
            return true;
        }
        return false;
    }

    public float GetBaseDamage()
    {
        return Strength * 51f + GetCurrentWeapon().AttackDamage * 10;
    }

    public Attack DoAttack()
    {
        Weapon currentWeapon = GetCurrentWeapon();
        float percentage = 1f + ((Random.value * 2f - 1.0f) * DAMAGE_VARIANCE);
        float baseDamage = GetBaseDamage();
        float damage = baseDamage * percentage;

        bool isCrit = Random.value < (CritChance + currentWeapon.CritChance);
        if (isCrit)
        {
            damage *= CRIT_MODIFIER;
        }

        return new Attack() { Damage = damage, IsCrit = isCrit };
    }

    public float MinDamage()
    {
        return GetBaseDamage() * (1f - DAMAGE_VARIANCE);
    }

    public float MaxDamage()
    {
        return GetBaseDamage() * (1f + DAMAGE_VARIANCE);
    }
}

public class Controls : MonoBehaviour {

    public DamageText DamageTextPrefab;
    public Enemy EnemyPrefab;
    public LevelUp LevelUpPrefab;

    public GameObject EnemiesContainer;

    public Player P = new Player();
    public Enemy CurrentEnemy;

    public bool AllowAttacks = true;
    public Button AttackButton;

    public int EnemiesKilled = 0;

    public Text InfoText;


    void Awake()
    {
        AttackButton = transform.FindChild("AttackButton").GetComponent<Button>();
        InfoText = transform.FindChild("StatsPanel").GetComponentInChildren<Text>();
    }

	// Use this for initialization
	void Start () {
        EnemiesContainer = GameObject.Find("Enemies");
        CurrentEnemy = GameObject.FindObjectOfType<Enemy>();
        CurrentEnemy.Init(P.MinDamage() * Random.Range(4, 9));
	}
	
	// Update is called once per frame
	void Update () {
        if (CurrentEnemy == null)
        {
            CurrentEnemy = GameObject.Instantiate<Enemy>(EnemyPrefab, EnemiesContainer.transform, true);
            CurrentEnemy.Init(P.MinDamage() * Random.Range(4,9));
            AllowAttacks = true;
        }

        AttackButton.interactable = AllowAttacks;

        InfoText.text = string.Format("Enemies Killed: {0}\nLevel: {1}\nExp: {9} / {10}\nStrength: {2}\nCrit Chance: {3}\nDamage: {4} ~ {5}\nCurrent Weapon:\n    Name: {6}\n    Weapon Attack: {7}\n    Weapon Crit: {8}",
            EnemiesKilled,
            P.Level,
            P.Strength,
            P.CritChance + P.GetCurrentWeapon().CritChance,
            P.MinDamage(),
            P.MaxDamage(),
            P.GetCurrentWeapon().Name,
            P.GetCurrentWeapon().AttackDamage,
            P.GetCurrentWeapon().CritChance,
            P.Exp,
            P.NextExpThreshold);
	}

    public void AttackButtonClick()
    {
        if (AllowAttacks)
        {
            Attack a = P.DoAttack();
            int damage = Mathf.RoundToInt(a.Damage);
            bool isDead = CurrentEnemy.TakeDamage(damage);
            Vector3 loc = CurrentEnemy.transform.position + CurrentEnemy.transform.up * 1.0f;
            DamageText dt = Instantiate<DamageText>(DamageTextPrefab, loc, Quaternion.identity);
            dt.Init("" + damage, a.IsCrit);

            AllowAttacks = !isDead;

            if (isDead)
            {
                EnemiesKilled += 1;
                bool hasLevelUp = P.GainExp(1);
                if(hasLevelUp)
                {
                    Instantiate(LevelUpPrefab, transform.parent, true);
                }
            }
        }
    }
}
