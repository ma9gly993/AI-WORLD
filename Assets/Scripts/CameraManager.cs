using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject cameraTopPos;
    public GameObject camepaTopThirdPos;
    public GameObject cameraAgentThirdPos;


    // Start is called before the first frame update
    void Start()
    {
        cameraTopPos.SetActive(false);
        camepaTopThirdPos.SetActive(false);
        cameraAgentThirdPos.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1
        {
            cameraTopPos.SetActive(true);
            camepaTopThirdPos.SetActive(false);
            cameraAgentThirdPos.SetActive(false);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2)) // 2
        { 
            cameraTopPos.SetActive(false);
            camepaTopThirdPos.SetActive(true);
            cameraAgentThirdPos.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) // 3
        {
            cameraTopPos.SetActive(false);
            camepaTopThirdPos.SetActive(false);
            cameraAgentThirdPos.SetActive(true);
        }
    }
}
