using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public enum STATE { IDLE, RUN, JUMP, FALL }
    public STATE currentState = STATE.IDLE;


    public float jumpSpeed = 12, moveSpeed = 5;

    public Rigidbody myRigidbody;

    public bool isOnGround;

    // Cache movimento salvato nell'update
    Vector3 moveVector;

    // cache input jump
    bool jumpInput = false;

    // Cache direzione di mira
    Vector3 aimVector;

    // Layer che il player considera pavimento
    public LayerMask groundLayer;

    // Proiettile base da usare per creare i nuovi
    public Bullet bulletTemplate;

    //Rocket base per creare altri missili
    public Rocket rocketTemplate;

    //timer per la state machine
    private float stateTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Equivale a trascinare il rigidbody su myRigidbody
            myRigidbody = GetComponent<Rigidbody>();

        //myRigidbody.maxvelocity = 1.0f;
    }

    /*
     * (SHOOT)
     * RUN
     * JUMP
     * IDLE
     * FALL
     * 
     * STATE IDLE
     * moveVector > RUN
     * jumpInput > JUMP
     * !onGround > FALL
     * 
     * STATE RUN
     * moveVextor == 0 > IDLE
     * jumpInput > JUMP
     * !onGround > FALL
     * 
     * STATE JUMP
     * caduta > FALL 
     * onGround > IDLE
     * 
     * STATE FALL
     * onGround > IDLE
     * 
     * 
     * ------  ------
     * IDLE 
     * //
     * 
     * RUN 
     * AddForce X
     * 
     * JUMP 
     * AddForce X
     * (Begin) AddForce Y
     * (condizione rilascio) riduzione velocità
     * 
     * FALL
     * AddForce X
     * 
     * 
     * Update lettura input
     * Update Stato corrente
     * 
     * FixedUpdate onGround
     * FixedUpdate stato corrente
     */

    // Update is called once per frame
    void Update()
    {
        InputUpdate();

        StateUpdate();

        //moveVector = Vector3.zero;

        //// Voglio muovere il personaggio verso destra di moveSpeed ogni secondo
        //if (Input.GetKey(KeyCode.D))
        //    //transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        //    moveVector += Vector3.right * moveSpeed;

        //if (Input.GetKey(KeyCode.A))
        //    //transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        //    moveVector += Vector3.left * moveSpeed;

        //// Se premo spazio e sono a terra, salto
        //if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        //    myRigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);

        //// Se lascio spazio durante un salto, rallenta il salto
        //if (Input.GetKeyUp(KeyCode.Space) && myRigidbody.velocity.y > 0)
        //{
        //    Vector3 tVelocity = myRigidbody.velocity;
        //    tVelocity.y /= 2.666f;
        //    myRigidbody.velocity = tVelocity;
        //}

        // Aggiorno vettore di mira
        AimUpdate();

        // Controllo input sparo
        FireUpdate();
    }

    void InputUpdate()
    {
        moveVector = Vector3.zero;

        // Voglio muovere il personaggio verso destra di moveSpeed ogni secondo
        if (Input.GetKey(KeyCode.D))
            moveVector += Vector3.right * moveSpeed;

        if (Input.GetKey(KeyCode.A))
            moveVector += Vector3.left * moveSpeed;

        // Se premo  salto
            jumpInput = Input.GetKey(KeyCode.Space);
    }

    void ChangeState(STATE state)
    {
        this.currentState = state;
        stateTimer = 0;
    }


    void StateUpdate()
    {

        stateTimer = Time.deltaTime;

        // update stato corrente
        switch(currentState)
        {
            case STATE.IDLE:
                IdleUpdate();
                break;
            case STATE.RUN:
                RunUpdate();
                break;
            case STATE.JUMP:
                JumpUpdate();
                break;
            case STATE.FALL:
                FallUpdate();
                break;
        }
    }

    void IdleUpdate()
    {
        if (!isOnGround) { ChangeState(STATE.FALL); return; }

        if(jumpInput) {
            ChangeState(STATE.JUMP);
            myRigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            return; 
        }

        if (moveVector.magnitude > 0) { ChangeState(STATE.RUN); return; }

    }

    void RunUpdate()
    {
        if (!isOnGround) ChangeState(STATE.FALL);

        if (jumpInput) { ChangeState(STATE.JUMP); return; }
        
        if(moveVector.magnitude == 0) {ChangeState(STATE.IDLE); return; }
    }

    void JumpUpdate()
    {

       

        if (isOnGround && stateTimer > 0.1f) ChangeState(STATE.IDLE);

        if(myRigidbody.linearVelocity.y < 0) { ChangeState(STATE.FALL); return; }

        if (!jumpInput)
        {
            Vector3 tVelocity = myRigidbody.linearVelocity;
            tVelocity.y /= 2.66f;
            myRigidbody.linearVelocity = tVelocity;
        }
    }

    void FallUpdate()
    {
        if (isOnGround) ChangeState(STATE.IDLE);

  
    }


    void FireUpdate()
    {
        // Controllo se è premuto il bottone di sparo
        if (Input.GetMouseButtonDown(0))
        {
            // Creo un'istanza (faccio una copia) del proiettile e la "sparo"
            // Instantiate(bulletTemplate, aimVector, Quaternion.identity).SetActive(true);

            Bullet tBullet = Instantiate(bulletTemplate);
            Vector3 shootVector = aimVector - transform.position;
            tBullet.transform.position = transform.position + shootVector.normalized;
            tBullet.gameObject.SetActive(true);
            tBullet.Shoot(shootVector);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Bullet tRocket = Instantiate(rocketTemplate);
            Vector3 shootVector = aimVector - transform.position;
            tRocket.transform.position = transform.position + shootVector.normalized;
            tRocket.gameObject.SetActive(true);
            tRocket.Shoot(shootVector);

        }

    }

    void AimUpdate()
    {
        Vector3 tMouseScreenToWorldPos = Input.mousePosition;
        tMouseScreenToWorldPos.z = transform.position.z - Camera.main.transform.position.z;

        aimVector = Camera.main.ScreenToWorldPoint(tMouseScreenToWorldPos);

        // Salvare in aimVector la posizione del mouse convertita a world
        // aimVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Modo brutto
        // aimVector = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

        //Debug.Log("Coordinate Screenspace: " + Input.mousePosition);
        //Debug.Log("Coordinate World: " + aimVector);
    }

    // Chiamato ad ogni update della fisica
    private void FixedUpdate()
    {
        isOnGround = Physics.Raycast(transform.position, Vector3.down, 0.502f, groundLayer);

        if (isOnGround)
            Debug.DrawRay(transform.position, Vector3.down * 0.502f, Color.green, Time.fixedDeltaTime);
        else
            Debug.DrawRay(transform.position, Vector3.down * 0.502f, Color.red, Time.fixedDeltaTime);

        // Ci muoviamo nell direzione salvata in moveVector durante l'Update
        if (moveVector.magnitude != 0)
        {
            //myRigidbody.angularDamping = 0.1f;
            //myRigidbody.linearDamping = 1;
            myRigidbody.AddForce(moveVector, ForceMode.Acceleration);
        }
        else
        {
            //myRigidbody.angularDamping = 9999;
            //myRigidbody.linearDamping = 9999;
        }



    }

    // isOnGround fatto male
    //private void OnCollisionEnter(Collision collision)
    //{
    //    // Se abbiamo colliso con un ground significa che siamo per terra
    //    if (collision.gameObject.name.Contains("Ground"))
    //        isOnGround = true;
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    // Se lascio un ground significa che non sono più per terra
    //    if (collision.gameObject.name.Contains("Ground"))
    //        isOnGround = false;
    //}
}
