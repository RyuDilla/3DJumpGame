using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float xAngle;
    public float rotate_speed = 1;
    private const float ANGLE_LIMIT_UP = 60f;
    private const float ANGLE_LIMIT_DOWN = -60f;

    [SerializeField]
    float cameraHeight = 1.0f;

    private GameObject player;
    private GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーのゲームオブジェクトを取得
        player = GameObject.FindGameObjectWithTag("Player");
        // メインカメラのゲームオブジェうとを取得
        mainCamera = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // カメラの高さを調整
        transform.position = player.transform.position + new Vector3(0f, cameraHeight, 0f);

        // xの角度を可視化(ハンズオン用)
        xAngle = transform.eulerAngles.x;

        // プレイヤーのマウス入力をベクトル情報として取得
        RotateCamera();

        // カメラの回転する角度を制限
        BlockAngle();
    }

    private void BlockAngle()
    {
        /*
         * X軸の角度が180°よりも大きかったらその値から360を引いた値を返す
         * そうでなかったらそのままの値を返す。
         */
        float angle_x = 180f <= transform.eulerAngles.x ? transform.eulerAngles.x - 360 : transform.eulerAngles.x;

        // カメラの角度を指定された角度に制限
        transform.eulerAngles = new Vector3(
            Mathf.Clamp(angle_x, ANGLE_LIMIT_DOWN, ANGLE_LIMIT_UP),
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );
    }

    private void RotateCamera()
    {
        // プレイヤーのマウス入力をベクトル情報として取得
        Vector3 angle = new Vector3(
            Input.GetAxis("Mouse X") * rotate_speed,
            Input.GetAxis("Mouse Y") * rotate_speed * (-1.0f),
            0
            );

        // 取得したベクトル情報を回転に変える
        transform.eulerAngles += new Vector3(angle.y, angle.x);
    }
}
