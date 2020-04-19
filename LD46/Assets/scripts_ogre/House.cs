using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public Sprite FullLife;
    public Sprite HalfDamaged;
    public Sprite Damaged;

    public int Life = 1;

    public SpriteRenderer sr;

    public void Damage()
    {
        Life -= 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = FullLife;
        this.Life = Constants.HOUSE_MAX_HP;
    }

    // Update is called once per frame
    void Update()
    {
        if (Life == 0) 
            sr.sprite = Damaged;
        else if ( this.Life <= Constants.HOUSE_DAMAGED_THR ) 
            sr.sprite = HalfDamaged;
    }
}
