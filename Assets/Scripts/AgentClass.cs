using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class AgentClass : MonoBehaviour
{
    public bool isDebugMode = false;
    public bool isAlive = true;

    public float health = 100;
    public float energy = 100f;
    public float energyMinus = 1f;
    public int secToEnergyMinus = 3;
    public int age = 0;

    // mutation list
    public int angleVision = 120;
    public int countRaysInEye = 3;  // must be > 1
    public int ageToDuplicate = 5;
    public int atackPowerLVL = 1;
    public int defenseLVL = 1;
    public int speedLVL = 1;
    public int rotationLVL = 1;
    public bool legs = false;
    public int eyesLVL = 1;
    public bool isCanDuplicate = false;
    public int temperatureResistanceHot = 0;
    public int temperatureResistanceCold = 0;
    public int temperature = 0;
        
    public string genTag;  // ex. xyxxxkjjjv 

    float moveHorizontal;
    float moveVertical;
    float moveSpeed;
    float rotationSpeed;
    int childCount = 0;

    bool isOnFood = false;
    GameObject lastFood;

    private IEnumerator coroutine;

    public int[] structLayers;
    float[] inputs;
    public AgentNeuralNetwork Brain;
    //public float[][] neurons = new float[3][];


    void Start()
    {
        age = 0;
        health = 100;
        energy = 100;

        //changeColor();
        AddMutation();

        energyMinus = isDebugMode ? 0f : calcEnergyMinus();
        calcMoveSpeed();
        coroutine = energyMinusEverySec(secToEnergyMinus);
        StartCoroutine(coroutine);

        //structLayers = new int[]{ 10, 3, 6 };
        //Brain = new AgentNeuralNetwork(structLayers);
    }

    void FixedUpdate()
    {
        if (!isAlive)
        {
            Destroy(gameObject);
        }
        if (health <= 0)
        {
            isAlive = false;
            Destroy(gameObject);
        }

        if (energy < -20)
        {
            energy = -20;
        }
        if (health > 100)
        {
            health = 100;
        }

        if (energy > 100)
        {
            energy = 100;
        }

        if (isDebugMode)
        {
            float[] outEyes = getDataFromEye();
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
            Move();
            if (Input.GetKeyDown(KeyCode.Space)) eatFood();
            if (Input.GetKeyDown(KeyCode.R)) Duplicate();
        }
        
        
        // brainStorm
        int[] actions = GetActionFromBrain();
        if (actions != null && actions.Length > 0)
        {
            if (actions[0] == 1) moveHorizontal = 1;
            if (actions[1] == 1) moveHorizontal = -1;
            if (actions[2] == 1) moveVertical = 1;
            if (actions[3] == 1) moveVertical = -1;
            if (actions[4] == 1) eatFood();
            if (actions[5] == 1) Duplicate();
        }
        Move();
        

        // TODO: 
        /// 1) ����� ���������� ��� ����  +
        /// 2) �������� �� �������� ����� ��  + 
        /// 3) ����������� ��������� (�����������)
        /// 3.1) mutate layers ai
        /// 4) �������� �� 0 �� 1 ������� ���������  +/-
        /// 5) ������� �� �������� ��������� ���� �����������
        /// 6) ������������ ������
        /// 7) ������ ��� �������� �� ����� ���������� � ������, �������, ���
        /// 8) �������� ����� �������
        /// 9) ������� ���� ������� ������� � -50 ��������
        ///    ������ ������� - ������ ���������� ��
        /// 10) ������ ���������� � ����
        /// 11) ��������� �������� �� ����� ������ ���
        ///     �� ������������ ��� ���������� ���� �  ��.
        /// 12) ���� ����������� �� ��������� � �� ��������������   
        /// 13) ������ ����������� �� ��������
        /// 14) ������� �� ������������ � �����������
        /// 15) ����� �� �������



    }
    int[] GetActionFromBrain()
    {
        inputs = new float[15 + countRaysInEye * 2];
        inputs[0] = health;
        inputs[1] = energy;
        inputs[2] = energyMinus;
        inputs[3] = atackPowerLVL;
        inputs[4] = defenseLVL;
        inputs[5] = speedLVL;
        inputs[6] = eyesLVL;
        inputs[7] = legs ? 1 : 0;
        inputs[8] = isCanDuplicate ? 1 : 0;
        inputs[9] = isOnFood ? 1 : 0;
        inputs[10] = age;
        inputs[11] = ageToDuplicate;
        inputs[12] = temperature;
        inputs[13] = temperatureResistanceCold;
        inputs[14] = temperatureResistanceHot;
        
        float[] eyeInputs = getDataFromEye();
        for (int i = 0; i < countRaysInEye * 2; i++)
        {
            inputs[12 + i] = eyeInputs[i];
        }
        float[] actions = Brain.FeedForward(inputs);
        int[] actionToUse = new int[actions.Length];
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i] >= 0) actionToUse[i] = 1;
            else actionToUse[i] = 0;
        }
        return actionToUse;
    }

    void Duplicate()
    {
        energy -= 10;

        if (isCanDuplicate)
        {
            if (age >= ageToDuplicate)
            {
                if (energy > 20)
                {
                    Debug.Log("SMTH WAS BORN. Parent: " + gameObject.name + " Energy: " + energy);
                    childCount += 1; 
                    Vector3 spawnPos = transform.position; spawnPos.x += 3f; spawnPos.z += 3f;
                    GameObject newChild = Instantiate(gameObject, spawnPos, Quaternion.identity);
                    newChild.SetActive(false);

                    AgentClass nnNewChild = newChild.GetComponent<AgentClass>();
                    int[] structLayers = new int[] { 15+nnNewChild.countRaysInEye*2, 8, 8, 6 };
                    nnNewChild.Brain = new AgentNeuralNetwork(structLayers);
                    Brain.copy(nnNewChild.Brain);
                    nnNewChild.Brain.Mutate(10, 0.5f);
                    nnNewChild.name = this.name + "_" + childCount.ToString();

                    newChild.SetActive(true);
                    energy -= 20;
                }
            }
        }
    }

    //vision
    float[] getDataFromEye()
        // TODO: ������� ������ ���� 
    {
        float[] outputs = new float[countRaysInEye * 2]; // count Rays * 2
        int k = 0;
        RaycastHit hit;

        for (int i = -angleVision/2; i <= angleVision/2; i += angleVision/(countRaysInEye-1))
        {
            Vector3 rayTo = Quaternion.AngleAxis(i, Vector3.up) * transform.forward;
            if (Physics.Raycast( new Ray(transform.position, rayTo), out hit  ))
            {
                outputs[k] = EyeObjToDigit(hit.collider.tag);
                outputs[k+1] = (hit.collider.transform.position - transform.position).magnitude;
                k++;
            }
        }
        /*
        rayRight = new Ray(transform.position, transform.right);
        rayLeft = new Ray(transform.position, -transform.right);
        rayForward = new Ray(transform.position, transform.forward);
        rayBackwards = new Ray(transform.position, -transform.forward);
        */
        if (isDebugMode)
        {
            for (int i = -angleVision / 2; i <= angleVision / 2; i += angleVision / (countRaysInEye - 1))
            {
                Vector3 rayTo = Quaternion.AngleAxis(i, Vector3.up) * transform.forward;
                Debug.DrawRay(transform.position, rayTo * 10f, Color.red);
            }
        }
        return outputs;
    }
    
    float EyeObjToDigit(string tag)
    {
        if (tag == "agent") return -1f;
        if (tag == "food") return 1f;


        return 0f;
    }

    void AddMutation()
    {
        int typeMutation = Random.Range(0, 13);
        int LVLMutation = Random.Range(-1,2);
        if (typeMutation == 1) atackPowerLVL += LVLMutation;
        else if (typeMutation == 2) defenseLVL += LVLMutation;
        else if (typeMutation == 3) speedLVL += LVLMutation;
        else if (typeMutation == 4) eyesLVL += LVLMutation;
        else if (typeMutation == 5) legs = !legs;
        else if (typeMutation == 6) isCanDuplicate = !isCanDuplicate;
        else if (typeMutation == 7) angleVision += LVLMutation * 10;
        //else if (typeMutation == 8) countRaysInEye += LVLMutation;
        else if (typeMutation == 9) ageToDuplicate += LVLMutation;
        else if (typeMutation == 10) rotationLVL += LVLMutation; 
        else if (typeMutation == 11) temperatureResistanceHot += LVLMutation*10;
        else if (typeMutation == 12) temperatureResistanceCold += LVLMutation*10;


        if (atackPowerLVL < 0) atackPowerLVL = 0;
        if (defenseLVL < 0) defenseLVL = 0;
        if (speedLVL < 0) speedLVL = 0;
        if (eyesLVL < 0) eyesLVL = 0;
        if (angleVision < 0) angleVision = 0;
        if (angleVision > 300) angleVision = 300;
        //if (countRaysInEye < 2) countRaysInEye = 2;
        if (ageToDuplicate < 0) ageToDuplicate = 0;
        if (rotationLVL < 0) rotationLVL = 0;
    }

    void eatFood()
    {
        energy -= 10;
        if (isOnFood)
        {
            Destroy(lastFood);
            energy += 90;
            health += 30;
            isOnFood =false;
        }
    }

    void Move()
    {
        //Vector3 moveDirection = new Vector3(moveHorizontal, 0.0f, moveVertical);
        //transform.position += moveDirection * moveSpeed;

        transform.position += moveVertical * transform.forward * moveSpeed;
        transform.Rotate(0, moveHorizontal * rotationSpeed, 0);
    }

    public void changeColor()
    {
        Renderer rend =  GetComponent<Renderer>();
        rend.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    float calcEnergyMinus()
    {
        float resLegs = 0f, resIsCanDuplicate = 0f;

        float kAtack = 2f, kDef = 2f, kSpeed = 5f, kLegs = 2f, kEyes = 2f, kIsCanDuplicate = 1f;
        float kAngleVision = 0.05f, kCountRaysInEye = 0.5f, kAgeToDuplicate = -1f, kRotation = 0.25f;

        if (legs) resLegs += kLegs;
        if (isCanDuplicate) resIsCanDuplicate += kIsCanDuplicate;
        float minusEnergy = kAtack * atackPowerLVL + kDef * defenseLVL + kSpeed * speedLVL + kEyes * eyesLVL + resLegs + resIsCanDuplicate
                      + kAngleVision * angleVision + kCountRaysInEye * countRaysInEye + kAgeToDuplicate * ageToDuplicate + kRotation * rotationLVL;
        return minusEnergy;
    }

    void calcMoveSpeed()
    {
        moveSpeed = .2f * speedLVL;
        rotationSpeed = 2f * rotationLVL;
    }

    private IEnumerator energyMinusEverySec(float sec)
    {
        while (true)
        {
            yield return new WaitForSeconds(sec);
            energy -= energyMinus;
            if (energy < 0)
            {
                health += energy;
            }

            if (temperature > temperatureResistanceHot) health -= (temperature - temperatureResistanceHot);
            if (temperature < temperatureResistanceCold) health -= (temperatureResistanceCold - temperature);
            // age
            age++;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "barrier")
        {
            transform.position = new Vector3(Random.Range(0f, 30f), 2.2f, Random.Range(-40.0f, 0f));
        }
        // Debug.Log(collision.gameObject.name);
        if ( collision.gameObject.tag != "water")
        {
            if (!legs)
            {
                transform.position = new Vector3(Random.Range(0f, 30f), 2.2f, Random.Range(-40.0f, 0f));
            }
        }

        if (collision.gameObject.tag == "desert")
        {
            temperature = 40;
            Debug.Log(this.name + " desert");
        }

        else if (collision.gameObject.tag == "snow")
        {
            temperature = -20;
        }

        else if (collision.gameObject.tag == "water")
        {
            temperature = 20;
        }

        else if (collision.gameObject.tag == "grass")
        {
            temperature = 25;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "food")
        {
            isOnFood = true;   
            lastFood = other.gameObject;
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "food")
        {
            isOnFood = false;
        }
    }

}