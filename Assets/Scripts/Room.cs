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
        doorLeft.SetActive(roomLeft);//�����ߴ��ڷ��䣬������߼�����
        doorRight.SetActive(roomRight);
        doorUp.SetActive(roomUp);
        doorDown.SetActive(roomDown);
    }

    public void UpdateRoom(float xOffset,float yOffset)
    {
        stepToStart = (int)(Mathf.Abs(transform.position.x / xOffset) + Mathf.Abs(transform.position.y / yOffset));//��¼ÿ����������ʼ�ĸ���
        //text.text = stepToStart.ToString();//�ı���ʾΪ����

        if (roomUp)//��¼��ǰ�����������
            doorNumber++;
        if (roomDown)
            doorNumber++;
        if (roomLeft)
            doorNumber++;
        if (roomRight)
            doorNumber++;
    }

    private void OnTriggerEnter2D(Collider2D other)//�жϽ�ɫ�Ƿ���뷿��
    {
        if(other.CompareTag("Player"))//�����⵽tagΪplayer���������
        {
            CameraController.instance.ChangeTarget(transform);//�ı�transformΪ�÷���λ��
        }
    }
}
