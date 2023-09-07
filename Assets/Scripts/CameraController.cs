using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public float speed;

    public Transform target;

    public void Awake()
    {
        instance = this;//启动相机控制器
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), speed * Time.deltaTime);
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;//将移动目标改成当前房间坐标
    }
}
