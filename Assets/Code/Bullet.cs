using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using DG.Tweening;
using System;

public class Bullet : MonoBehaviour
{
    public Rigidbody myRigidbody;
    [SerializeField]protected float bulletSpeed = 15f;

    public AudioSource hitSound;

    public Ease explosionEasing = Ease.Linear;

    //test di esempio, pratica non cosigliata per inzializzare il rigidBody
    public Rigidbody MyRigidBody { //proprieta
        get {
            if (!myRigidbody)
            {
                myRigidbody = GetComponent<Rigidbody>();
            }
            return myRigidbody;
        }
    }


    public virtual void Shoot(Vector3 aimVector)
    {
        //due metodi per assegnare la velocita al proiettile
        //myRigidbody.linearVelocity = aimVector;
        MyRigidBody.AddForce(aimVector.normalized * bulletSpeed, ForceMode.Impulse);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //MyStart();
    }



    // Update is called once per frame
    void Update()
    {
        //MyUpdate(); 
    }


    private void FixedUpdate()
    {
        //MyFixedUpdate();
    }

    

    public virtual void OnCollisionEnter(Collision collision)
    {

        StartCoroutine(Explosion_EC());
    }

    IEnumerator Explosion_EC()//echo return
    {
        //fa qulcosa e aspetta
        //esplosione che varia la scala del proiettile
        //transform.localScale = Vector3.one * 5.95f;

        hitSound.Play();

        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;

        transform.DOScale(3f, 0.4f).SetEase(explosionEasing);


        yield return new WaitForSeconds(0.4f);

        transform.DOScale(0, 1).SetEase(Ease.Linear);

        yield return new WaitForSeconds(1);

        //esegue un'altra cosa
        //distruggiamo il proiettile
        Destroy(gameObject);
    }


    private void OnBecameInvisible() {
        Destroy(gameObject);
    }


    //protected virtual void MyStart()
    //{

    //}

    //protected virtual void MyUpdate()
    //{

    //}
    //protected virtual void MyFixedUpdate()
    //{

    //}
}
