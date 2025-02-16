using System;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : Bullet
{

    private enum State { IDLE = 0, FLYING = 1, EXPLODING = 2 };

    private State currentState = State.IDLE;

    private float stateTimer;


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
       myRigidbody.AddForce(transform.forward * bulletSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);


        if (myRigidbody.linearVelocity.normalized != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(transform.position);
        }

    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void FixedUpdate()
    {
        StateUpdate();
    }

    public override void Shoot(Vector3 aimVector)
    {
        base.Shoot(aimVector);
        
        ChangeState(State.FLYING);
    }

}
