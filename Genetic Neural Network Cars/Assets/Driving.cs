using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driving : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    [SerializeField] private float throttle;
    [SerializeField] private float steer = 0;
    [SerializeField] private float steeringWheelRot = 0;
    [SerializeField] 
    private NeuralNetwork input;

    private bool stopped = false;
    private float TOP_SPEED = 10;
    private float STEERING_DEADZONE = 0.1f;
    private float WHEEL_TURNING_SPEED = 0.1f;

    // Start is called before the first frame update
    void Start()
    {   
        
    }

    private void move()
    {

        speed += (throttle / (1 + speed));
        if (speed < 0) speed = 0;
        else if (speed > TOP_SPEED) speed = TOP_SPEED;

        Vector3 forward = transform.up.normalized * (speed / 100);
        transform.position += forward;

        if (speed > 0.1) //no steering standing still
        {
            Vector3 right = -Vector3.forward;
            transform.Rotate(right, steeringWheelRot);
        }
    }

    private void getInput() {
        //throttle = input.throttle;
        //steer = intput.steer;
        if (Input.GetKey("w")) throttle = 1;
        else throttle = -1;
        if (Input.GetKey("a")) steer = -1;
        else if (Input.GetKey("d")) steer = 1;
        else steer = 0;

      
      if (steer > STEERING_DEADZONE)
      { //Turn right
            steeringWheelRot += steer * WHEEL_TURNING_SPEED;
            if (steeringWheelRot > 1) steeringWheelRot = 1;
            }
      else if (steer < -STEERING_DEADZONE)
      { //Turn left
            steeringWheelRot += steer * WHEEL_TURNING_SPEED;
            if (steeringWheelRot < -1) steeringWheelRot = -1;
      }
      else
      {
            if (steeringWheelRot > STEERING_DEADZONE) steeringWheelRot -= WHEEL_TURNING_SPEED;
            else if (steeringWheelRot < -STEERING_DEADZONE) steeringWheelRot += WHEEL_TURNING_SPEED;
            else steeringWheelRot = 0;
      }
       
    }

    /*Returns speed and rotation of steering wheel*/
    public float[] getData() 
    {
        return new float[] {speed, steeringWheelRot};
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!stopped)
        {
            getInput();
            move();
        }
    }

    public void enable()
    {
        stopped = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogError("entered");
        stopped = true;
    }
}
