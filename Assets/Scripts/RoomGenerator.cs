using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{
    public enum Direction { up,down,left,right};//�Է������ɵ��ĸ��������ö��
    public Direction direction;//ö�ٱ���

    [Header("������Ϣ")]
    public GameObject roomPrefab;
    public int roomNumber;//���ɵķ�������
    public Color startColor, endColor;
    private GameObject endRoom;//���ʼ������Զ�ķ��䣨�յ㣩
    private int endRoomNumber;
    private GameObject bossRoom;//���յ㷿����������boss��
    //List<GameObject> farRooms = new List<GameObject>();//��Զ�ʹ�Զ����

    [Header("λ�ÿ���")]
    public Transform generetorPoint;
    public float xOffset;
    public float yOffset;
    public LayerMask roomLayer;//��Ҫ����Ƿ��ظ���ͼ��

    public int maxStep = 0;
    public List<Room>rooms = new List<Room>();//�洢���ɵķ����б�Room.cs
    public WallType wallType;
    
    void Start()
    {
        for(int i=0;i<roomNumber;i++)
        {
            AddRooms();
            ChangePointPos();//�ı�pointλ��
        }

        foreach (var room in rooms)
        {
            SetupRoom(room, room.transform.position);//����
        }

        FindEndRoom();

        foreach (var room in rooms)
        {
            SetupWall(room, room.transform.position);//��ǽ
        }
    }

    void Update()
    {
        //if(Input.anyKeyDown)
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);//��õ�ǰ����ĳ�������������������ó�����������ԣ�
        //}
    }

    public void AddRooms()//���ɷ���
    {
        rooms.Add(Instantiate(roomPrefab, generetorPoint.position, Quaternion.identity).GetComponent<Room>());//���ɷ��䣬������ת��
    }

    public void ChangePointPos()//�ƶ�����
    {
        while(true)
        {
            direction = (Direction)UnityEngine.Random.Range(0, 3);//���ĸ��������������1��
            switch (direction)//����������������з������ɵ�λ��ƫ��
            {
                case Direction.up:
                    generetorPoint.position += new Vector3(0, yOffset, 0);
                    break;
                case Direction.down:
                    generetorPoint.position += new Vector3(0, -yOffset, 0);
                    break;
                case Direction.left:
                    generetorPoint.position += new Vector3(-xOffset, 0, 0);
                    break;
                case Direction.right:
                    generetorPoint.position += new Vector3(xOffset, 0, 0);
                    break;
            }
            if(Physics2D.OverlapCircle(generetorPoint.position, 0.2f, roomLayer))//�ڵ�ǰ��λ0.2��Χ�ڼ���Ӧͼ���Ƿ��Ѵ���������ײ��
            {
                switch (direction)//������ڣ��򽫼��㻹ԭ
                {
                    case Direction.up:
                        generetorPoint.position += new Vector3(0, -yOffset, 0);
                        break;
                    case Direction.down:
                        generetorPoint.position += new Vector3(0, yOffset, 0);
                        break;
                    case Direction.left:
                        generetorPoint.position += new Vector3(xOffset, 0, 0);
                        break;
                    case Direction.right:
                        generetorPoint.position += new Vector3(-xOffset, 0, 0);
                        break;
                }
            }
            else { break; }
        }
    }

    public void SetupRoom(Room newRoom,Vector3 roomPosition)//����
    {
        newRoom.roomUp = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayer);//�жϷ����Ϸ��Ƿ�����������䣬�����������Room.cs
        newRoom.roomDown = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayer);
        newRoom.roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayer);
        newRoom.roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayer);

        newRoom.UpdateRoom(xOffset,yOffset);//��unity���趨��xyƫ��ֵ����rom.cs
    }

    public void SetupWall(Room newRoom, Vector3 roomPosition)//��ǽ
    {
        switch (newRoom.doorNumber)//�����������ɶ�Ӧǽ��
        {
            case 1:
                if (newRoom.roomUp)
                    Instantiate(wallType.singleUp, roomPosition, Quaternion.identity);
                if (newRoom.roomDown)
                    Instantiate(wallType.singleDown, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft)
                    Instantiate(wallType.singleLeft, roomPosition, Quaternion.identity);
                if (newRoom.roomRight)
                    Instantiate(wallType.singleRight, roomPosition, Quaternion.identity);
                break;
            case 2:
                if (newRoom.roomLeft && newRoom.roomUp)
                    Instantiate(wallType.doubleUL, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft && newRoom.roomRight)
                    Instantiate(wallType.doubleRL, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft && newRoom.roomDown)
                    Instantiate(wallType.doubleDL, roomPosition, Quaternion.identity);
                if (newRoom.roomRight && newRoom.roomUp)
                    Instantiate(wallType.doubleUR, roomPosition, Quaternion.identity);
                if (newRoom.roomDown && newRoom.roomUp)
                    Instantiate(wallType.doubleUD, roomPosition, Quaternion.identity);
                if (newRoom.roomDown && newRoom.roomRight)
                    Instantiate(wallType.doubleDR, roomPosition, Quaternion.identity);
                break;
            case 3:
                if (newRoom.roomLeft && newRoom.roomUp && newRoom.roomRight)
                    Instantiate(wallType.tripleURL, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft && newRoom.roomDown && newRoom.roomRight)
                    Instantiate(wallType.tripleDRL, roomPosition, Quaternion.identity);
                if (newRoom.roomDown && newRoom.roomUp && newRoom.roomRight)
                    Instantiate(wallType.tripleUDR, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft && newRoom.roomUp && newRoom.roomDown)
                    Instantiate(wallType.tripleUDL, roomPosition, Quaternion.identity);
                break;
            case 4:
                if (newRoom.roomLeft && newRoom.roomUp && newRoom.roomRight && newRoom.roomDown)
                    Instantiate(wallType.fourDoors, roomPosition, Quaternion.identity);
                break;
        }
    }

    //public void FindEndRoom()
    //{
    //    // ��ȡ�������Զ��ֵ��������Զ���丳ֵ��endRoom
    //    foreach (var room in rooms)
    //    {
    //        if (room.stepToStart > maxStep)
    //        {
    //            maxStep = room.stepToStart;
    //            endRoom = room.gameObject;
    //        }
    //    }
    //    // ��ȡ��Զ�ʹ�Զ����
    //    foreach (var room in rooms)
    //    {
    //        if (room.stepToStart >= maxStep - 1)
    //            farRooms.Add(room.gameObject);
    //    }
    //    // ��ȡ��Զ�ʹ�Զ�����еĵ��ŷ��䣬�������ô�滻֮ǰ��endRoom��ֵ
    //    foreach (var room in farRooms)
    //    {
    //        if (room.GetComponent<Room>().doorNumber == 1)
    //            endRoom = room;
    //    }
    //}

    public void FindEndRoom()//�ҵ���Զ���䣬�����쵥����boss����
    {
        //if(room.transform.position.sqrMagnitude> endRoom.transform.position.sqrMagnitude)//ѭ�����ÿ���������ԭ����������ȣ�����Զ����Ϊ�յ�
        //{
        //    endRoom = room.gameObject;
        //}

        // ��ȡ�������Զ��ֵ��������Զ���丳ֵ��endRoom
        //foreach (var room in rooms)
        //{
        //    if (room.stepToStart > maxStep)
        //    {
        //        maxStep = room.stepToStart;
        //        endRoom = room.gameObject;
        //    }
        //}
        for (int i = 0; i < roomNumber; i++)
        {
            if (rooms[i].stepToStart > maxStep)
            {
                maxStep = rooms[i].stepToStart;
                endRoom = rooms[i].gameObject;
                endRoomNumber = i;//��¼��endroom�ķ����
            }
        }
        generetorPoint.position = endRoom.transform.position;//�����ɵ��λ���Ƶ�endroom��λ��

        while(true)//����һ������endroom�ķ�����Ϊbossroom
        {
            ChangePointPos();
            AddRooms();
            SetupRoom(rooms.Last(), rooms.Last().transform.position);
            bossRoom = rooms.Last().gameObject;
            if(bossRoom.GetComponent<Room>().doorNumber > 1)//�����㵥��������ɾ�����ɵķ���
            {
                Destroy(bossRoom);
                rooms.RemoveAt(rooms.Count);
                generetorPoint.position = endRoom.transform.position;
            }
            else { break;}
        }
        endRoom.GetComponent<Room>().doorNumber = 0;//��endroom���ż���������
        SetupRoom(rooms[endRoomNumber], rooms[endRoomNumber].transform.position);//������endroom������һ����
    }
}

[System.Serializable]//ʹ������Ա�unityϵͳʶ��
public class WallType
{
    public GameObject singleLeft, singleRight, singleUp, singleDown,
                      doubleUL, doubleUR, doubleUD, doubleRL, doubleDR, doubleDL,
                      tripleURL, tripleUDR, tripleUDL, tripleDRL,
                      fourDoors;
}