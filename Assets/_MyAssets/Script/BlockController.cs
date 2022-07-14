using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{

    [SerializeField] BoxCollider boxCollider;
    float initialBlockPositionY = -0.43f;

    enum BlockState
    {
        Idle,
        Collected,
    }
    BlockState currentBlockState;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boxCollider.isTrigger = false;
            currentBlockState = BlockState.Collected;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentBlockState = BlockState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBlockState == BlockState.Collected)
        {
            CollectedMove();
        }
    }

    void CollectedMove()
    {
        var index = Locator.i.playerController.Block.IndexOf(transform);
        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(0, ((index + 1) - Locator.i.playerController.Block.Count) + initialBlockPositionY, 0);
    }
}
