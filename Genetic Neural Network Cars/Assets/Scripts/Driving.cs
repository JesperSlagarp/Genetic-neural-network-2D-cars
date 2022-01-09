using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driving : MonoBehaviour
{
    public float fitness = 0;
    public float adjustedSpeed;
    [SerializeField] private float speed = 0;
    [SerializeField] private float throttle;
    [SerializeField] private float steer = 0;
    [SerializeField] public float steeringWheelRot = 0;
    [SerializeField] 
    private NeuralNetwork input;

    private bool stopped = false;
    private float TOP_SPEED = 15;
    //private float STEERING_DEADZONE = 0.1f;
    //private float WHEEL_TURNING_SPEED = 0.1f;


    // Start is called before the first frame update
    void Awake()
    {
        input.init();
    }

    private void move()
    {

        speed += (throttle / (1 + speed));
        if (speed < 0) speed = 0;
        else if (speed > TOP_SPEED) speed = TOP_SPEED;
        adjustedSpeed = speed / TOP_SPEED;

        Vector3 forward = transform.up.normalized * (speed / 100);
        transform.position += forward;

        if (speed > 2f) //no steering standing still
        {
            Vector3 right = -Vector3.forward;
            transform.Rotate(right, steeringWheelRot);
        }
    }

    private void Reset()
    {
        speed = 0;
        throttle = 0;
        steer = 0;
        steeringWheelRot = 0;
        adjustedSpeed = 0;
    }

    private void getInput() {
        //Uncomment for neural network input
        //if (Input.GetKey("t"))
        //{
                //Debug.Log("T pressed");
                float[] inputs = input.tick();
                throttle = inputs[0];
                throttle = 2 * (throttle - 0.5f); //Adjustment for sigmoid range, from (0,1) to (-1,1)
                steer = inputs[1];
                steer = 4 * (steer - 0.5f); //Adjustment for sigmoid range, from (0,1) to (-4,4)

        steeringWheelRot = steer;
        //}
        /*
        if (Input.GetKey("w")) throttle = 1;
        else throttle = -1;
        if (Input.GetKey("a")) steer = -1;
        else if (Input.GetKey("d")) steer = 1;
        else steer = 0;*/

      
      /*if (steer > STEERING_DEADZONE)
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
      }*/
       
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
            fitness += speed * 0.1f;
        }
    }

    public void enable()
    {
        stopped = false;
        Reset();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.LogError("entered");
        if (collision.collider.tag == "Walls")
        {
            try
            {
                GameObject.FindGameObjectWithTag("Genetic Algorithm").GetComponent<GeneticAlgorithm>().decreaseAlive();
            }
            catch { }
            stopped = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        fitness += 100;
        //Debug.Log("Collider tag: " + collision.gameObject.tag);
        if(collision.gameObject.tag == "Finish")
        {
            //Debug.Log("Finished");
            GameObject.FindGameObjectWithTag("Genetic Algorithm").GetComponent<GeneticAlgorithm>().finThisGen++;
        }
    }
}
