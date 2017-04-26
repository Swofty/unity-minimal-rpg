using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public float Health = 100f;
    public float MaxHealth = 100f;

    private Animator anim;

    public const float DELAY_TILL_DESTROY = .5f;

    public Text HealthText;

    public void Init(float maxHealth)
    {
        MaxHealth = maxHealth;
        Health = MaxHealth;
    }

    public bool TakeDamage(float damage)
    {
        Health = Mathf.Clamp((Health - damage), 0.0f, MaxHealth);
        if (Health <= 0.0f)
        {
            anim.SetTrigger("Die");
            StartCoroutine(DestroySelf());
            return true;
        }
        return false;
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        HealthText = GetComponentInChildren<Text>();
        Init(100f);
    }

	// Use this for initialization
	void Start () {
		
	}


	
	// Update is called once per frame
	void Update () {
        HealthText.text = string.Format("Health: {0} / {1}", (int)Health, (int)MaxHealth);
	}

    public IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(DELAY_TILL_DESTROY);
        Destroy(this.gameObject);
    }
}
