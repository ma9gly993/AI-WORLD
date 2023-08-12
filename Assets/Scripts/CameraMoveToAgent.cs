using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveToAgent : MonoBehaviour
{
    private GameObject agent;
    private GameObject[] agentsList;
    private int agentNum = 0;
    private IEnumerator coroutine;
    public float timeFindAgents = 1.0f;

    public float cameraFollowSpeed = 0.2f;
    public float cameraRotateSpeed = 0.1f;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    /*
    void OnEnable()
    {
        coroutine = FindAgentsForTimer(timeFindAgents);
        StartCoroutine(coroutine);
    }
    
    private IEnumerator FindAgentsForTimer(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            agentsList = GameObject.FindGameObjectsWithTag("agent");
            agentNum = 0;
            agent = agentsList[agentNum];
        }

    }
    */
    
    void FindAgents()
    {
        agentsList = GameObject.FindGameObjectsWithTag("agent");
        agentNum = 0;
        agent = agentsList[agentNum];
    }

    void MoveCamera()
    {
        Vector3 targetMove = agent.transform.position;
        Quaternion targetRotation = agent.transform.rotation;
        targetMove.y += 15f;
        targetMove.z -= 5f;
        transform.position = Vector3.SmoothDamp(transform.position, targetMove, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.eulerAngles = new Vector3(45f, 0f, 0f);

        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, cameraRotateSpeed);
        //transform.rotation.Set(-45f, 0f, 0f, 0f);   
        //= new Quaternion(-50.0f, .0f, .0f, .0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (agentsList.Length > 0)
            {
                agentNum++;
                agentNum = Mathf.Min(agentNum, agentsList.Length - 1);
                //if (agentNum > agentsList.Length - 1) agentNum = agentsList.Length - 1;
                agent = agentsList[agentNum];
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (agentsList.Length > 0)
            {
                agentNum--;
                agentNum = Mathf.Max(agentNum, 0);
                agent = agentsList[agentNum];
            }
        }
        
        if (agent)
        {
            MoveCamera();
        }
        else
        {
            FindAgents();
        }


    }
}
