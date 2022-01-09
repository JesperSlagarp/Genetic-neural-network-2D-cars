using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Transform bestAgentPos;
    GeneticAlgorithm GA;

    private void Start()
    {
        try 
        {
            GA = GameObject.FindGameObjectWithTag("Genetic Algorithm").GetComponent<GeneticAlgorithm>(); 
        }
        catch { }
    }
    void FixedUpdate()
    {
        try
        {
            bestAgentPos = GA.getBestAgent().transform;
            this.transform.position = new Vector3(bestAgentPos.position.x, bestAgentPos.position.y, this.transform.position.z);
        } 
        catch
        {
            GameObject Agent = GameObject.FindGameObjectWithTag("Cars");
            if (Agent != null)
                this.transform.position = new Vector3(Agent.transform.position.x, Agent.transform.position.y, this.transform.position.z);
        }
    }
}
