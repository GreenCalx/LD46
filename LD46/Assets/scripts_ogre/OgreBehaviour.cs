using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreBehaviour : MonoBehaviour
{
    public int food;
    public int moral;

    public GameObject rightHand;
    public GameObject rightHandRestingPosition;
    public GameObject leftHand;

    public GameObject mouth;
    private GameObject currentTarget;
    public bool needEat = false;

    public float EatingAnimationDuration = 2; // 2 seconds animation
    public float CurrentAnimationTime = 0;

    enum States { EATING, REST, GETTING_TARGET, ANGRY, HUNGRY }
    private States currentState = States.REST;

    // Start is called before the first frame update
    void Start()
    {
        this.food = Constants.MAX_FOOD;
        this.moral = Constants.MAX_MORAL;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // this merans something on the bgelt is copming, start to go towards it to take it
        // first find collider
        if (collision.gameObject.name != "conveyor_belt")
        {
            currentTarget = collision.gameObject;
            currentState = States.GETTING_TARGET;
        }
    }

    void EatingAnimationStart()
    {
        // here is the animation process if needed
        CurrentAnimationTime = EatingAnimationDuration;
    }
    void EatingAnimationStop()
    {
        // here is the animation stopping proicess if needed
    }
    void EatingAnimation()
    {
        // here is during eatig animation process if needed
        Debug.Log("NOM NOM NOM");
        CurrentAnimationTime -= Time.deltaTime;
        if (CurrentAnimationTime <= 0)
        {
            currentState = States.REST;
            CurrentAnimationTime = 0;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (this.moral > 0)
            this.moral--;

        if (currentState == States.GETTING_TARGET)
        {
            rightHand.transform.position = Vector3.Lerp(rightHand.transform.position, currentTarget.transform.position, Time.deltaTime);
            if (Physics2D.IsTouching(rightHand.GetComponent<BoxCollider2D>(), currentTarget.GetComponent<BoxCollider2D>()))
            {
                // remove from conmveyor belt
                var conveyor = GameObject.Find("conveyor_belt");
                var conveyor_script = conveyor ? conveyor.GetComponent<BeltConveyor>() : null;
                if (conveyor_script) conveyor_script.removeOnBelt(currentTarget);

                currentTarget.transform.parent = rightHand.transform;
                needEat = true;
            }


            if (needEat)
            {
                // we have something to eat, let s go to the mouth position
                rightHand.transform.position = Vector3.Lerp(rightHand.transform.position, mouth.transform.position, Time.deltaTime);
                if (Physics2D.IsTouching(rightHand.GetComponent<BoxCollider2D>(), mouth.GetComponent<BoxCollider2D>()))
                {
                    currentState = States.EATING;
                    currentTarget.GetComponent<Villager>().Kill();
                    EatingAnimationStart();
                }
            }

        }


        if (currentState == States.EATING) EatingAnimation();

        if (currentState == States.REST)
        {
            // if we are not in the resting position, let s reset the position
            if (rightHand.transform.position != rightHandRestingPosition.transform.position)
            {
                rightHand.transform.position = Vector3.Lerp(rightHand.transform.position, rightHandRestingPosition.transform.position, Time.deltaTime);
                if (Vector3.Dot(rightHand.transform.position, rightHandRestingPosition.transform.position) >= 0.9f)
                {
                    // close enough we reset the position
                    rightHand.transform.position = rightHandRestingPosition.transform.position;
                }
            }
        }

        if (currentState == States.HUNGRY)
        {
            // ask for food
        }

        if (currentState == States.ANGRY)
        {
            // go in village and rampage
        }
    }
}
