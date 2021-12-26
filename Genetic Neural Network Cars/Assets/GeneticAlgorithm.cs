using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    [SerializeField]
    private int intervall = 500;
    [SerializeField]
    private int intervallCounter;
    [SerializeField]
    private GameObject carPrefab;
    [SerializeField]
    private Transform spawnPoint;

    private GameObject[] cars;
    private NeuralNetwork[] carsNN;
    private Driving[] carsDriving;

    private int parentIndex;

    private int genSize = 100;
    // Start is called before the first frame update
    void Start()
    {
        cars = new GameObject[genSize];
        carsNN = new NeuralNetwork[genSize];
        carsDriving = new Driving[genSize];

        for (int i = 0; i < genSize; i++) 
        { 
            GameObject temp = Instantiate(carPrefab, spawnPoint.position, Quaternion.Euler(0, 0, 0));
            cars[i] = temp;
            carsNN[i] = temp.GetComponent<NeuralNetwork>();
            //Debug.Log("Before initLayer");
            carsNN[i].initLayer(6, 6, true);
            //Debug.Log("After first initLayer");
            carsNN[i].initLayer(6, 6, true);
            //carsNN[i].initLayer(5, 7, true);
            //Debug.Log("After second initLayer");
            carsNN[i].initLayer(2, 6, false);
            //Debug.Log("After last initLayer");
            carsDriving[i] = temp.GetComponent<Driving>();
        }

        
    }
    void FixedUpdate()
    {
        intervallCounter++;
        if (intervallCounter >= intervall) 
        {
            parentIndex = mostFitAgentIndex();
            newGen();
            intervallCounter = 0;
        }
    }

    private int mostFitAgentIndex() {
        float bestFitness = 0;
        int index = 0;

        for (int i = 0; i < genSize; i++) 
        {
            if (carsDriving[i].fitness > bestFitness)
            {
                bestFitness = carsDriving[i].fitness;
                index = i;
            }
        }
        Debug.Log("Best fitness: " + bestFitness);

        return index;
    }

    private void newGen() 
    {

        for (int i = 0; i < genSize; i++) 
        {
            if (i != parentIndex)
            {
                Destroy(cars[i]);
                carsNN[i] = null;
                carsDriving[i] = null;

                cars[i] = Instantiate(carPrefab, spawnPoint.position, Quaternion.Euler(0, 0, 0));
                carsNN[i] = cars[i].GetComponent<NeuralNetwork>();
                for (int j = 0; j < 2; j++) // hidden layers
                {
                    float[][] hiddenweights = cars[parentIndex].GetComponent<NeuralNetwork>().getLayerWeights(j);
                    carsNN[i].initLayer(hiddenweights, true);
                }
                float[][] weights = cars[parentIndex].GetComponent<NeuralNetwork>().getLayerWeights(2);
                carsNN[i].initLayer(weights, false);
                carsNN[i].mutate();
                carsDriving[i] = cars[i].GetComponent<Driving>();
            }
            else 
            {
                cars[i].transform.position = spawnPoint.position;
                cars[i].transform.rotation = spawnPoint.rotation;
                carsDriving[i].enable();
            }
        }


    }
}
