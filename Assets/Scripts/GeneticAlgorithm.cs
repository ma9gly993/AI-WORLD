using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public GameObject agent;
    public float timeSpawnAgent = 3.0f;
    public int countAgentSpawn = 5;
    private IEnumerator coroutine;
    private int agentSpawned = 0;


    void Start()
    {
        coroutine = AgentGenerator(timeSpawnAgent);
        StartCoroutine(coroutine);

    }

    private IEnumerator AgentGenerator(float waitTime)
    {
        while (true)
        {
            for (int i = 0; i  < countAgentSpawn; i++)
            {
                Vector3 spawnPos = new Vector3(Random.Range(-25.0f, 30f), 2.2f, Random.Range(-65.0f, -2f));
                AgentClass newAgent = (Instantiate(agent, spawnPos, Quaternion.identity)).GetComponent<AgentClass>();

                int[] structLayers = new int[] { 12 + newAgent.countRaysInEye * 2, 8, 8, 6 };

                newAgent.Brain = new AgentNeuralNetwork(structLayers);
                newAgent.health = 100; newAgent.energy = 100; agentSpawned++;
                newAgent.name = "Agent_" + agentSpawned.ToString();
                newAgent.changeColor();
            }
            yield return new WaitForSeconds(waitTime);
        }

    }
}
