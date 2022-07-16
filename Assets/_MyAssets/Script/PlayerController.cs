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
        Goal,
    }

    public PlayerState currentPlayerState { get; set; }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block")) CheckBlock(other.transform);
    }
    void CheckBlock(Transform trans)
    {
        currentMaterial = trans.GetComponent<Renderer>().material;

        if (block.Count == 0)
        {
            // Debug.Log("1");
            trans.tag = "Collected";
            playerRenderer.material = currentMaterial;
            block.Add(trans);
            trans.parent = blockParent;
        }
        else if (block.Count > 0 && playerRenderer.material.color == currentMaterial.color)
        {
            // Debug.Log("2");
            trans.tag = "Collected";
            block.Add(trans);
            trans.parent = blockParent;
        }
        else if (block.Count > 0 && playerRenderer.material.color != currentMaterial.color)
        {
            // Debug.Log(playerRenderer.material);
            // Debug.Log(currentMaterial);
            // Debug.Log("3");
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
        Debug.Log(currentPlayerState);
        if (currentPlayerState == PlayerState.Start && Input.GetMouseButtonDown(0))
        {
            currentPlayerState = PlayerState.Run;
        }
        if (currentPlayerState == PlayerState.Start) return;


        if (block.Count > 0) currentPlayerState = PlayerState.Ride;
        else if (block.Count == 0) currentPlayerState = PlayerState.Run;

        currentYPosition = (block.Count) * 0.4f + initialPositionY;
        transform.position = new Vector3(transform.position.x, currentYPosition, transform.position.z);
        capsuleCollider.height = colliderHeight + Block.Count;
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, colliderCenter - (transform.localScale.y * Block.Count), capsuleCollider.center.z);


        if (currentPlayerState == PlayerState.Run)
        {

            animator.SetBool("Idle", false);
            animator.SetBool("Run", true);
            Move();
        }
        if (currentPlayerState == PlayerState.Ride)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Run", false);
            Move();
        }

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
