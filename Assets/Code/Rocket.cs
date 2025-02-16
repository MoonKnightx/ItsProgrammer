using DG.Tweening;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class Rocket : Bullet
{

    private enum State { IDLE = 0, FLYING = 1, EXPLODING = 2 };

    private State currentState = State.IDLE;

    private float stateTimer;

    Vector3 muoseVector;

    Vector3 tMouseScreenToWorldPos;

    Vector3 flyingVector;

    private void ChangeState(State state)
    {
        this.currentState = state;
        stateTimer = 0;
    }

    private void StateUpdate()
    {
        stateTimer = Time.deltaTime;

        switch (currentState) { 
            case State.IDLE:
                break;
            case State.FLYING:
                FlyingUpdate();
                break;
            case State.EXPLODING:
                break;
        }
    }

    private void FlyingUpdate()
    {
        //Debug.Log("FlyingUpdate");

        // aggiorno la poszione del mouse
        tMouseScreenToWorldPos = Input.mousePosition;
        tMouseScreenToWorldPos.z = transform.position.z - Camera.main.transform.position.z;
        tMouseScreenToWorldPos.x = transform.position.x - Camera.main.transform.position.x;
        tMouseScreenToWorldPos.y = transform.position.y - Camera.main.transform.position.y;

        muoseVector = Camera.main.ScreenToWorldPoint(tMouseScreenToWorldPos);

        // calcolo la direzione del vettore
        flyingVector = (muoseVector - transform.position).normalized;

        myRigidbody.AddForce(flyingVector * bulletSpeed, ForceMode.Acceleration);

    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void FixedUpdate()
    {
        StateUpdate();
    }

    public override void Shoot(Vector3 aimVector)
    {
        base.Shoot(aimVector);
        StartCoroutine(Flying_EC());
    }


    IEnumerator Flying_EC() 
    {

        //aspetta 0.5 secondi prima di passare allo stato di Flying

        yield return new WaitForSeconds(0.2f);

        ChangeState(State.FLYING);
    }
}
