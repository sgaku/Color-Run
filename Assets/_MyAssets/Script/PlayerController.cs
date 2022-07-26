using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody rigidBody;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] Animator animator;
    //ブロックの親オブジェクトの位置情報
    [SerializeField] Transform blockParent;
    public Transform BlockParent
    {
        get { return blockParent; }
        private set { blockParent = value; }
    }
    //デバッグ用にシリアライズを設定
    [SerializeField] List<Transform> block = new List<Transform>();
    public List<Transform> Block
    {
        get { return block; }
        private set { block = value; }
    }

    float preMouseX;
    float deltaMouseX;
    float speedX;
    //プレイヤーの現在のY軸のポジション
    float currentYPosition;
    //プレイヤーの初期のY軸のポジション
    float initialPositionY;
    float colliderHeight = 3f;
    float colliderCenter = 1.5f;
    Material currentMaterial;
    //移動スピード
    [SerializeField] float speedZ;
    [SerializeField] Renderer playerRenderer;
    public Renderer PlayerRenderer
    {
        get { return playerRenderer; }
        private set { playerRenderer = value; }
    }

    readonly float pixelToUnitAdjustment = 0.8f; // px/frameをunit/frameに変換する補正用の定数
    float ResolutionAdjustment => (float)Screen.width / 750f; // iPhone678SE2を基準に開発している場合
    readonly float speedXInterpolate = 0.7f; // スピードを滑らかに変化させる


    public enum PlayerState
    {
        Start,
        Run,
        Ride,
        Check,
        Goal,
        Fail,
    }

    public PlayerState currentPlayerState { get; set; }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block")) CheckBlock(other.transform);
        else if (other.CompareTag("FinishCheck"))
        {
            if (block.Count <= 0)
            {
                currentPlayerState = PlayerState.Fail;
                return;
            }
            currentPlayerState = PlayerState.Check;
            BlockController blockController = block[0].GetComponent<BlockController>();
            blockController.currentBlockState = BlockController.BlockState.Checked;
            block.RemoveAt(0);

            // block.RemoveAt(block.Count - 1);
        }
        else if (other.CompareTag("Finish"))
        {
            if (block.Count <= 0)
            {
                currentPlayerState = PlayerState.Fail;
                return;
            }
            BlockController blockController = block[0].GetComponent<BlockController>();
            blockController.currentBlockState = BlockController.BlockState.Checked;
            block.RemoveAt(0);
            currentPlayerState = PlayerState.Goal;
        }
    }

    // void OnCollisionEnter(Collision collisionInfo)
    // {

    // }
    void CheckBlock(Transform trans)
    {
        currentMaterial = trans.GetComponent<Renderer>().material;

        if (block.Count == 0)
        {
            trans.tag = "Collected";
            playerRenderer.material = currentMaterial;
            block.Add(trans);
            trans.parent = blockParent;
        }
        else if (block.Count > 0 && playerRenderer.material.color == currentMaterial.color)
        {
            trans.tag = "Collected";
            block.Add(trans);
            trans.parent = blockParent;
        }
        else if (block.Count > 0 && playerRenderer.material.color != currentMaterial.color)
        {
            block.RemoveAt(block.Count - 1);
        }

    }

    void Start()
    {
        currentPlayerState = PlayerState.Start;
        initialPositionY = transform.position.y;
    }
    void Update()
    {
        // Debug.Log(currentPlayerState);
        if (currentPlayerState == PlayerState.Start && Input.GetMouseButtonDown(0)) currentPlayerState = PlayerState.Run;
        if (currentPlayerState == PlayerState.Start) return;

        //プレイヤーのコライダーの長さ調節
        capsuleCollider.height = colliderHeight + blockParent.childCount;
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, colliderCenter - (transform.localScale.y * blockParent.childCount), capsuleCollider.center.z);

        switch (currentPlayerState)
        {
            case PlayerState.Run:
                animator.SetBool("Idle", false);
                animator.SetBool("Run", true);
                Move();
                break;
            case PlayerState.Ride:
                animator.SetBool("Idle", true);
                animator.SetBool("Run", false);
                Move();
                break;
            case PlayerState.Check:
                animator.SetBool("Idle", true);
                animator.SetBool("Run", false);
                Move();
                break;
            case PlayerState.Fail:
                rigidBody.velocity = Vector3.zero;
                break;
            case PlayerState.Goal:
                Move();
                break;

        }
        if (currentPlayerState == PlayerState.Check || currentPlayerState == PlayerState.Goal || currentPlayerState == PlayerState.Fail) return;

        if (currentPlayerState == PlayerState.Run && block.Count > 0) currentPlayerState = PlayerState.Ride;
        else if (block.Count == 0) currentPlayerState = PlayerState.Run;

        currentYPosition = (block.Count) * 0.4f + initialPositionY;
        transform.position = new Vector3(transform.position.x, currentYPosition, transform.position.z);

        if (Input.GetMouseButtonDown(0))
        {
            preMouseX = Input.mousePosition.x;
        }
        if (Input.GetMouseButton(0))
        {
            // どの解像度でも感度が一定になるように
            deltaMouseX = (Input.mousePosition.x - preMouseX) * ResolutionAdjustment;
            preMouseX = Input.mousePosition.x;
        }
        else
        {
            // 指を離した場合に横移動を止めるため
            deltaMouseX = 0;
        }

        // 横の速度の計算
        // フレームレートによってdeltaMouseXの値が変わってしまうため、補正をかける
        var frameRateAdjustment = Time.fixedDeltaTime / Time.deltaTime;
        var targetSpeedX = deltaMouseX * pixelToUnitAdjustment * frameRateAdjustment;
        // 滑らかに速度変化するようにLerpを使う
        speedX = Mathf.Lerp(speedX, targetSpeedX, speedXInterpolate / frameRateAdjustment);
    }

    void Move()
    {
        // rigidBodyに速度を反映
        var vel = rigidBody.velocity;
        vel.x = speedX;
        vel.y = 0;
        vel.z = speedZ;
        rigidBody.velocity = vel;
    }
}
