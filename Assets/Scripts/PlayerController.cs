using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float move_speed;
    [SerializeField]
    float jumpPower;

    private Transform m_Cam;
    private Vector3 m_CamForward;
    private Vector3 m_Move;

    private Animator animator;
    private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = GroundCheck();

        animator.SetBool("isGrounded", isGrounded);

        // 移動方向の取得
        m_Move = GetMoveInput();

        var velocity = m_Move.magnitude;

        // 回転と前進
        RotateAndForward(velocity);

        // アニメーターのパラメータを設定
        animator.SetFloat("velocity", velocity);

        // スペースキーを押したらジャンプ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.AddForce(Vector3.up * jumpPower);
            animator.SetTrigger("jump");
        }

    }

    private void RotateAndForward(float velocity)
    {
        if (velocity > 0.1)
        {
            // 取得した方向に回転
            transform.rotation = Quaternion.LookRotation(m_Move);

            // 前に進む
            transform.Translate(Vector3.forward * Time.deltaTime * move_speed);
        }
    }

    // プレイヤーの入力を取得して移動
    private Vector3 GetMoveInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (m_Cam != null)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v * m_CamForward + h * m_Cam.right;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            m_Move = v * Vector3.forward + h * Vector3.right;
        }

        return m_Move;
    }

    // 着地判定用フィールド
    public float hitLength;
    private Vector3 centerOffsetV3;
    private CapsuleCollider m_capsule;
    private RaycastHit hit;
    [SerializeField]
    bool isGrounded;

    public bool GroundCheck()
    {

        m_capsule = GetComponent<CapsuleCollider>();

        hitLength = (m_capsule.height / 2.0f - m_capsule.radius + 0.1f);

        centerOffsetV3 = new Vector3(0, hitLength, 0);
        var gcStartPosition = transform.position + centerOffsetV3;

        var isHit = Physics.SphereCast(gcStartPosition, m_capsule.radius, Vector3.down, out hit, hitLength, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        if (isHit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    // 着地判定を可視化するためのメソッド（処理とは関係ない）
    private void OnDrawGizmos()
    {
        RaycastHit hit;

        // Playerのカプセルコライダーを取得
        var m_capsule = GetComponent<CapsuleCollider>();

        // カプセルコライダーを基準にレイキャストを射出する長さを設定
        hitLength = (m_capsule.height / 2.0f - m_capsule.radius + 0.1f);

        var centerOffsetV3 = new Vector3(0, hitLength, 0);

        // スフィアキャストの開始点を設定
        var gcStartPosition = transform.position + centerOffsetV3;

        // スフィアキャスト射出
        var isHit = Physics.SphereCast(gcStartPosition, m_capsule.radius, Vector3.down, out hit, hitLength, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        if (isHit)
        {
            // スフィアキャストが何かに当たったら赤く表示
            Gizmos.color = Color.red;
            Gizmos.DrawRay(gcStartPosition, transform.up * (-1) * hit.distance);
            Gizmos.DrawWireSphere(gcStartPosition + transform.up * (-1) * (hit.distance), m_capsule.radius);
        }
        else
        {
            // スフィアキャストに何も当たらなければ緑色で表示
            Gizmos.color = Color.green;
            Gizmos.DrawRay(gcStartPosition, transform.up * (-1) * hitLength);
            Gizmos.DrawWireSphere(gcStartPosition + transform.up * (-1) * (hitLength), m_capsule.radius);
        }
    }
}
