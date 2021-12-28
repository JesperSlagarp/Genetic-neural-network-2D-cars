using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    [SerializeField]
    private Sensor[] sensors;
    [SerializeField]
    private Driving car;

    private List<Neuron[]> layers;
    public int numLayers = 0;

    public void init()
    {
        layers = new List<Neuron[]>();
    }

    public float[] tick() 
    {
        //Debug.Log("Start of tick");

        float[] inputs = new float[1 + sensors.Length];
        inputs[0] = car.adjustedSpeed;
        //Debug.Log("adjusted speed: " + inputs[0]);

        //inputs[1] = car.steeringWheelRot;
        //Debug.Log("steeringWheelRot: " + inputs[1]);
        for (int i = 0; i < sensors.Length; i++)
        {
            inputs[i + 1] = sensors[i].adjustedSensorDistance;
            //Debug.Log("Sensor: " + inputs[i + 2]);
        }

        //Debug.Log("Inputs = " + string.Join(" | ", new List<float>(inputs).ConvertAll(i => i.ToString()).ToArray()));

        float[] curr = copyArray(inputs);

        for (int i = 0; i < layers.Count; i++) //For each layer
        {
            //Debug.Log("Layer nr. " + (i+1));
            float[] next = new float[layers[i].Length];
            //Debug.Log("Num of neurons: " + (layers[i].Length));
            for (int j = 0; j < next.Length; j++) { //For each neuron in layer
                next[j] = layers[i][j].activate(curr);
            }
            //Debug.Log("next = " + string.Join(" | ", new List<float>(next).ConvertAll(i => i.ToString()).ToArray()));
            curr = next;
            //Debug.Log("Layer outputs = " + string.Join(" | ", new List<float>(curr).ConvertAll(i => i.ToString()).ToArray()));
        }

        return curr;
    }

    public void initLayer(int size, int prevSize, bool isHidden) 
    {
        Neuron[] neurons = new Neuron[size];
        for (int i = 0; i < neurons.Length; i++) 
        {
            neurons[i] = new Neuron(prevSize, isHidden);
        }
        layers.Add(neurons);
        numLayers++;
    }

    public void initLayer(float[][] weights, bool isHidden) 
    {
        Neuron[] neurons = new Neuron[weights.Length];
        for (int i = 0; i < neurons.Length; i++) 
        {
            neurons[i] = new Neuron(weights[i], isHidden);
        }
        layers.Add(neurons);
        numLayers++;
    }

    public float[][] getLayerWeights(int layerIndex) {
        float[][] weights = new float[layers[layerIndex].Length][];
        for (int i = 0; i < weights.Length; i++) {
            weights[i] = layers[layerIndex][i].getWeights();
        }

        return weights;
    }

    public void mutate(float amount) { 
        for(int i = 0; i < layers.Count; i++)
        {
            for(int j = 0; j < layers[i].Length; j++)
            {
                layers[i][j].mutate(amount);
            }
        }
    }


    private float[] copyArray(float[] original)
    {
        float[] copy = new float[original.Length];
        for (int i = 0; i < original.Length; i++) 
        {
            copy[i] = original[i];
        }
        return copy;
    }

}
