using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCreator : MonoBehaviour
{


    [SerializeField] Transform[] block = new Transform[3];

    int[,] instantiatePosition = new int[10, 3];

    float positionZ;
    float positionX;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < instantiatePosition.GetLength(0); i++)
        {
            positionZ = 5 + (5 * i);
            for (int j = 0; j < instantiatePosition.GetLength(1); j++)
            {
                positionX = j - 1;
                var randomNum = Random.Range(0, block.Length);

                Instantiate(block[randomNum], new Vector3(positionX, 0.8f, positionZ), Quaternion.identity, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
