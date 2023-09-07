using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public GameObject doorLeft, doorRight, doorUp, doorDown; 
    public bool roomLeft, roomRight, roomUp, roomDown;
    public Text text;
    public int stepToStart;
    public int doorNumber;

    // Start is called before the first frame update
    void Start()
    {
        doorLeft.SetActive(roomLeft);//如果左边存在房间，则在左边激活门
        doorRight.SetActive(roomRight);
        doorUp.SetActive(roomUp);
        doorDown.SetActive(roomDown);
    }

    public void UpdateRoom(float xOffset,float yOffset)
    {
        stepToStart = (int)(Mathf.Abs(transform.position.x / xOffset) + Mathf.Abs(transform.position.y / yOffset));//记录每个房间距离初始的格数
        //text.text = stepToStart.ToString();//文本显示为格数

        if (roomUp)//记录当前房间的门数量
            doorNumber++;
        if (roomDown)
            doorNumber++;
        if (roomLeft)
            doorNumber++;
        if (roomRight)
            doorNumber++;
    }

    private void OnTriggerEnter2D(Collider2D other)//判断角色是否进入房间
    {
        if(other.CompareTag("Player"))//如果检测到tag为player的物体进入
        {
            CameraController.instance.ChangeTarget(transform);//改变transform为该房间位置
        }
    }
}
