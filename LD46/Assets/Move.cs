using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private Rigidbody rb;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerForward = Vector3.forward;

        //playerBasis = Instantiate(transform, transform.position, transform.rotation); ;
        //worldBasis = Instantiate(transform, transform.position, transform.rotation); ;
        faceBasis = GameObject.Find("faceBasis").transform;
    }

    void FixedUpdate()
    {
        // compute flatten position aka as if we were not going around a cube
        Vector3 FlattenPosition = LastFlattenPosition 
            + (Vector3.forward*inputX*speed*Time.fixedDeltaTime
            + Vector3.right * inputY * speed * Time.fixedDeltaTime) / 10; // 10 = world scale

        // find player forward vector, it should always go according to world vectors
        // projected on current face if it is a new input
        // if it is not a new input we continue to use previous forward vectors
        // Dot == 1 ou -1 si // , 0 si perpendiculaire
        if(Mathf.Abs(Vector3.Dot( Vector3.up, transform.up )) == 1 )
        {
            playerForward = Vector3.ProjectOnPlane(Vector3.forward, transform.up);
        }
        else
        {
            playerForward = Vector3.ProjectOnPlane(Vector3.up, transform.up);
        }
        playerForward.Normalize();
        playerRight = Quaternion.AngleAxis(90, transform.up) * playerForward;

        // find player direction from inputs
        Vector3 playerDirection = inputY * playerRight + inputX * playerForward;
        Debug.DrawRay(transform.position, playerDirection, Color.white);

        //rotate player to face the newly computed player direction
        if (inputX != 0 || inputY != 0)
        {
            // transform.forward = playerDirection;
            // transform.up = faceBasis.up;
            var angle = Vector3.SignedAngle(transform.forward, playerDirection, transform.up);
            Debug.Log(angle);
            transform.RotateAround(transform.up, Mathf.Deg2Rad*angle);
        }
        // apply physics
        transform.position = transform.position + playerDirection * speed * Time.fixedDeltaTime;

        if (inputX != 0)
        {
            newinput = false;
          //  transform.position = transform.position + playerDirection * inputX * speed * Time.fixedDeltaTime;
            
            if (inputX > 0 && Mathf.Abs(LastFlattenPosition.z) < 0.6 && Mathf.Abs(FlattenPosition.z) >= 0.6)
            {
                Vector3 clamped = new Vector3( Mathf.Clamp(transform.position.x, -5, 5f), Mathf.Clamp(transform.position.y, -5, 5f),Mathf.Clamp(transform.position.z, -5, 5f));
                transform.RotateAround(clamped, faceBasis.right, 90);
                faceBasis.RotateAround(faceBasis.right, Mathf.Deg2Rad * 90);
            }
            if ( inputX < 0 && Mathf.Abs(LastFlattenPosition.z) < 0.6 && Mathf.Abs(FlattenPosition.z) >= 0.6)
            {
                Vector3 clamped = new Vector3( Mathf.Clamp(transform.position.x, -5, 5f), Mathf.Clamp(transform.position.y, -5, 5f),Mathf.Clamp(transform.position.z, -5, 5f));
                transform.RotateAround(clamped, faceBasis.right, -90);
                faceBasis.RotateAround(faceBasis.right, Mathf.Deg2Rad * -90);
            }
            if (inputX > 0 && Mathf.Abs(LastFlattenPosition.z) >= 0.6 && Mathf.Abs(FlattenPosition.z) < 0.6)
            {
                Vector3 clamped = new Vector3( Mathf.Clamp(transform.position.x, -5, 5f), Mathf.Clamp(transform.position.y, -5, 5f),Mathf.Clamp(transform.position.z, -5, 5f));
                transform.RotateAround(clamped, faceBasis.right,90);
                faceBasis.RotateAround(faceBasis.right, Mathf.Deg2Rad * 90);
            }
            if ( inputX < 0 && Mathf.Abs(LastFlattenPosition.z) >= 0.6 && Mathf.Abs(FlattenPosition.z) < 0.6)
            {
                Vector3 clamped = new Vector3( Mathf.Clamp(transform.position.x, -5, 5f), Mathf.Clamp(transform.position.y, -5, 5f),Mathf.Clamp(transform.position.z, -5, 5f));
                transform.RotateAround(clamped, faceBasis.right, -90);
                faceBasis.RotateAround(faceBasis.right, Mathf.Deg2Rad * -90);
            }
        }

        if (inputY != 0)
        {
            newinput = false;
           //transform.position = transform.position + playerRight * inputY * speed * Time.fixedDeltaTime;
            if (inputY > 0 && Mathf.Abs(LastFlattenPosition.x) < 0.6 && Mathf.Abs(FlattenPosition.x) >= 0.6)
            {
                Vector3 clamped = new Vector3( Mathf.Clamp(transform.position.x, -5, 5f), Mathf.Clamp(transform.position.y, -5, 5f),Mathf.Clamp(transform.position.z, -5, 5f));
                transform.RotateAround(clamped, faceBasis.forward,  -90);
                faceBasis.RotateAround(faceBasis.forward, Mathf.Deg2Rad * -90);
            }
            if ( inputY < 0 && Mathf.Abs(LastFlattenPosition.x) < 0.6 && Mathf.Abs(FlattenPosition.x) >= 0.6)
            {
                Vector3 clamped = new Vector3( Mathf.Clamp(transform.position.x, -5, 5), Mathf.Clamp(transform.position.y,-5, 5f),Mathf.Clamp(transform.position.z, -5, 5f));
                transform.RotateAround(clamped, faceBasis.forward,  90);
                faceBasis.RotateAround(faceBasis.forward, Mathf.Deg2Rad * 90);
            }
            if (inputY > 0 && Mathf.Abs(LastFlattenPosition.x) >= 0.6 && Mathf.Abs(FlattenPosition.x) < 0.6)
            {
                Vector3 clamped = new Vector3( Mathf.Clamp(transform.position.x, -5, 5f), Mathf.Clamp(transform.position.y, -5, 5f),Mathf.Clamp(transform.position.z, -5, 5f));
                transform.RotateAround(clamped, faceBasis.forward,  -90);
                faceBasis.RotateAround(faceBasis.forward, Mathf.Deg2Rad * -90);
            }
            if ( inputY < 0 && Mathf.Abs(LastFlattenPosition.x) >= 0.6 && Mathf.Abs(FlattenPosition.x) < 0.6)
            {
                Vector3 clamped = new Vector3( Mathf.Clamp(transform.position.x, -5, 5f), Mathf.Clamp(transform.position.y, -5, 5f),Mathf.Clamp(transform.position.z, -5, 5f));
                transform.RotateAround(clamped, faceBasis.forward,  90);
                faceBasis.RotateAround(faceBasis.forward, Mathf.Deg2Rad * 90);
            }

        }

        var go = GameObject.Find("Ground");
        Collider[] hitColliders = Physics.OverlapBox(go.transform.position, go.GetComponent<BoxCollider>().size, go.transform.rotation );
        int i = 0;
        OnGround = false;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].name != "Ground" && hitColliders[i].name != "Player")
              OnGround = true;

            i++;
        }

        if (!OnGround)
            UpdateVelocity();

        if (WantToJump && OnGround) 
           Jump();

            LastFlattenPosition = FlattenPosition;

    }

    void Jump()
    {
        velocityY = jumpForce;
        transform.position += transform.up * velocityY;
        OnGround = false;
    } 
    void UpdateVelocity()
    {
        velocityY = 0.1f * Physics.gravity.y * Time.fixedDeltaTime;
        transform.position += transform.up * velocityY;
    }

    // Update is called once per frame
    public float speed = 1;
    public float inputX = 0;
    public float inputY = 0;

    private int currentFace = 0;
    public Vector3 LastFlattenPosition;

    private bool WantToJump = false;
    public float fallMultiplier = 1;
    public float jumpForce = 1;
    public float velocityY = 0;
    public bool OnGround = false;

    public bool newinput = true;

    public Vector3 playerForward;
    public Vector3 playerRight;
    public Vector3 faceForward;
    public Vector3 faceRight;

    public Transform faceBasis;
    //public Transform worldBasis;
   // public Transform playerBasis;

    void Update()
    {
        inputX = Input.GetAxisRaw("Vertical");
        inputY = Input.GetAxisRaw("Horizontal");

        if (inputX == 0 && inputY == 0)
        {
            newinput = true;
        }

        //Debug.DrawRay(transform.position, Vector3.forward *12, Color.red);
        //Debug.DrawRay(transform.position, Vector3.right *12, Color.blue);
        //Debug.DrawRay(transform.position, Vector3.up *12, Color.green);

        Debug.DrawRay(transform.parent.position, faceBasis.forward *12, Color.red);
        Debug.DrawRay(transform.parent.position, faceBasis.right *12, Color.blue);
        Debug.DrawRay(transform.parent.position, faceBasis.up *12, Color.green);

        Debug.DrawRay(transform.position, playerForward, Color.magenta);
        //Debug.DrawRay(transform.position, transform.forward * 4, Color.cyan );


        WantToJump = Input.GetKey(KeyCode.E);
    }
}
