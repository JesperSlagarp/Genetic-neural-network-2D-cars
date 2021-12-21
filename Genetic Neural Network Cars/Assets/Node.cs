using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    private int inputs;
    private float[] weights;
    private float bias;
    // Start is called before the first frame update
    void Start()
    {
        inputs = 7;
        weights = new float[7];
        bias = 1;
    }

    void activationFuntion() { 
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
