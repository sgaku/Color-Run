using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform targetPosition;
    [SerializeField] float _smoothTime = 0.3f;
    float _currentVelocity = 0f;
    float offsetZ;
    float currentZ;
    // Start is called before the first frame update
    void Start()
    {
        offsetZ = transform.position.z - targetPosition.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        currentZ = Mathf.SmoothDamp(transform.position.z, targetPosition.position.z + offsetZ, ref _currentVelocity, _smoothTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);
    }
}
