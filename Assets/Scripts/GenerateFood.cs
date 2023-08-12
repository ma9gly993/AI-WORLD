using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFood : MonoBehaviour
{
    public GameObject food;
    public float timeSpawnFood = 3.0f;
    public float timeDestroyFood = 10.0f;
    public int countFoodSpawn = 5;
    private IEnumerator coroutine;
    
    
    void Start()
    {
        coroutine = foodGenerator(timeSpawnFood);
        StartCoroutine(coroutine);

    }

    private IEnumerator foodGenerator(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            for (int i = 0; i+1 < countFoodSpawn; i++)
            {
                
                Vector3 spawnPos = new Vector3(Random.Range(-100.0f, 110.0f), 2.2f, Random.Range(-70.0f, 80.0f));
                GameObject newFood = Instantiate(food, spawnPos, Quaternion.identity);
                newFood.transform.parent = gameObject.transform;
                StartCoroutine(destroyFood(newFood));
            }
        }
        
    }

    private IEnumerator destroyFood(GameObject oldFood)
    {
        yield return new WaitForSeconds(timeDestroyFood);
        Destroy(oldFood);

    }
}
