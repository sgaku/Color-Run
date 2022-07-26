using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{

    [SerializeField] BoxCollider boxCollider;

    [SerializeField] Renderer blockMaterial;
    float initialBlockPositionY = -0.43f;
    float index;

    public enum BlockState
    {
        Idle,
        Collected,
        Checked,
        Removed,
    }
    public BlockState currentBlockState { get; set; }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) CheckCPlayerMaterial();
    }

    void CheckCPlayerMaterial()
    {
        if (Locator.i.playerController.Block.Count == 0
        || Locator.i.playerController.Block.Count > 0 && Locator.i.playerController.PlayerRenderer.material.color == blockMaterial.material.color)
        {
            boxCollider.isTrigger = false;
            currentBlockState = BlockState.Collected;
        }
        else if (Locator.i.playerController.Block.Count > 0 && Locator.i.playerController.PlayerRenderer.material.color != blockMaterial.material.color) currentBlockState = BlockState.Removed;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentBlockState = BlockState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーがチェックポジションに入ったら強制的にチェック状態にする
        if (currentBlockState == BlockState.Checked)
        {
            transform.parent = null;
            return;
        }
        if (currentBlockState == BlockState.Removed)
        {
            transform.parent = null;
            transform.gameObject.SetActive(false);
            return;
        }
        if (currentBlockState == BlockState.Collected)
        {
            CollectedMove();
        }

    }

    void CollectedMove()
    {
        index = Locator.i.playerController.Block.IndexOf(transform);
        if (index == -1) currentBlockState = BlockState.Removed;
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localPosition = new Vector3(0, ((index + 1) - Locator.i.playerController.Block.Count) + initialBlockPositionY, 0);
        }
    }
}
