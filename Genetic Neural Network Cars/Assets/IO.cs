using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IO : MonoBehaviour
{
    [SerializeField]
    int timeScale;
    string m_Path;

    [SerializeField]
    private GameObject carPrefab;
    private Transform spawnPoint;


    void Awake()
    {
        timeScale = 1;

        //Get the path of the Game data folder
        m_Path = Application.persistentDataPath;

        spawnPoint = GameObject.FindWithTag("Respawn").transform;
    }


    private void Update()
    {
        if (Input.GetKeyDown("s"))
            saveBestAgent();
        if (Input.GetKeyDown("d"))
            disableGA();
        if (Input.GetKeyDown("l"))
            loadBestAgent();

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            timeScale++;
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            timeScale--;

        Time.timeScale = timeScale;

    }

    private void saveBestAgent()
    {
        Debug.Log(m_Path);
        StreamWriter sw = new StreamWriter(m_Path + "/Saves/save.txt");
        GeneticAlgorithm GA = GameObject.FindWithTag("Genetic Algorithm").GetComponent<GeneticAlgorithm>();
        NeuralNetwork NN = GA.getBestAgent().GetComponent<NeuralNetwork>();

        sw.WriteLine(NN.numLayers);
        for (int i = 0; i < NN.numLayers; i++)
        {
            
            float[][] layerWeights = NN.getLayerWeights(i);
            sw.WriteLine(layerWeights.Length);
            for (int j = 0; j < layerWeights.Length; j++)
            {
                //string neuron = layerWeights[j].Length + "\n";
                sw.WriteLine(layerWeights[j].Length);
                for (int k = 0; k < layerWeights[j].Length; k++)
                {
                    //neuron += layerWeights[j][k] + "\n";
                    sw.WriteLine(layerWeights[j][k]);
                }
                //sw.WriteLine(neuron);
            }
        }
        sw.Close();
    }

    private void loadBestAgent() 
    {
        GameObject temp = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        NeuralNetwork NN = temp.GetComponent<NeuralNetwork>();
        StreamReader sr = new StreamReader(m_Path + "/Saves/save.txt");
        int numLayers = int.Parse(sr.ReadLine());
        for(int i = 0; i < numLayers; i++)
        {
            int numNeurons = int.Parse(sr.ReadLine());
            float[][] weights = new float[numNeurons][];
            for (int j = 0; j < numNeurons; j++)
            {
                int numWeights = int.Parse(sr.ReadLine());
                weights[j] = new float[numWeights];
                for(int k = 0; k < numWeights; k++)
                {
                    weights[j][k] = float.Parse(sr.ReadLine());
                }
            }
            if(i == numLayers - 1)
                NN.initLayer(weights, false);
            else
                NN.initLayer(weights, true);
        }
    }

    private void disableGA()
    {
        GameObject.FindWithTag("Genetic Algorithm").SetActive(false);
    }

}
