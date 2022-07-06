using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody rigidBody;
    [SerializeField] Animator animator;
    float preMouseX;
    float deltaMouseX;
    float speedX;
    [SerializeField] float speedZ;
    [SerializeField] Renderer playerRenderer;

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
        if (other.CompareTag("Block"))
        {
            var currentMaterial = other.GetComponent<Renderer>().material;
            Debug.Log(currentMaterial);
            playerRenderer.material = currentMaterial;
        }
    }

    void Start()
    {
        currentPlayerState = PlayerState.Start;
    }
    void Update()
    {

        if (currentPlayerState == PlayerState.Start && Input.GetMouseButtonDown(0))
        {
            currentPlayerState = PlayerState.Run;
        }
        if (currentPlayerState == PlayerState.Start) return;

        if (Input.GetMouseButtonDown(0))
        {
            preMouseX = Input.mousePosition.x;
        }

        if (currentPlayerState == PlayerState.Run)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Run", true);
            Move();
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
