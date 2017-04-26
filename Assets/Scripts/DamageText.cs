using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour {

    public string Number;
    public bool IsCrit;

    public Vector3 dir;

    public const float SPEED = .01f;

    private Text TextComponent;

    public void Init(string number, bool isCrit)
    {
        Number = number;
        IsCrit = isCrit;
    }

    void Awake()
    {
        TextComponent = GetComponentInChildren<Text>();
        dir = new Vector3((Random.value * 2f - 1f) * .5f, 1f, 0.0f).normalized;
    }

	// Use this for initialization
	void Start () {
        TextComponent.text = Number;
        TextComponent.color = IsCrit ? Color.red : Color.black;
        TextComponent.fontSize = 20;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += dir * SPEED;
        TextComponent.color = new Color(TextComponent.color.r, TextComponent.color.g, TextComponent.color.b, TextComponent.color.a * .90f);

        if(TextComponent.color.a <= .2f)
        {
            Destroy(this.gameObject);
        }
	}
}
