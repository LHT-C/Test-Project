using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{
    public enum Direction { up,down,left,right};//对房间生成的四个方向进行枚举
    public Direction direction;//枚举变量

    [Header("房间信息")]
    public GameObject roomPrefab;
    public int roomNumber;//生成的房间数量
    public Color startColor, endColor;
    private GameObject endRoom;//离初始房间最远的房间（终点）
    private int endRoomNumber;
    private GameObject bossRoom;//在终点房间后额外生成boss房
    //List<GameObject> farRooms = new List<GameObject>();//最远和次远房间

    [Header("位置控制")]
    public Transform generetorPoint;
    public float xOffset;
    public float yOffset;
    public LayerMask roomLayer;//需要检测是否重复的图层

    public int maxStep = 0;
    public List<Room>rooms = new List<Room>();//存储生成的房间列表到Room.cs
    public WallType wallType;
    
    void Start()
    {
        for(int i=0;i<roomNumber;i++)
        {
            AddRooms();
            ChangePointPos();//改变point位置
        }

        foreach (var room in rooms)
        {
            SetupRoom(room, room.transform.position);//造门
        }

        FindEndRoom();

        foreach (var room in rooms)
        {
            SetupWall(room, room.transform.position);//造墙
        }
    }

    void Update()
    {
        //if(Input.anyKeyDown)
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);//获得当前激活的场景，按下任意键后重置场景（方便测试）
        //}
    }

    public void AddRooms()//生成房间
    {
        rooms.Add(Instantiate(roomPrefab, generetorPoint.position, Quaternion.identity).GetComponent<Room>());//生成房间，不需旋转；
    }

    public void ChangePointPos()//移动监测点
    {
        while(true)
        {
            direction = (Direction)UnityEngine.Random.Range(0, 3);//从四个方向中随机生成1个
            switch (direction)//根据四种情况来进行房间生成点位的偏移
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
            if(Physics2D.OverlapCircle(generetorPoint.position, 0.2f, roomLayer))//在当前点位0.2范围内检测对应图层是否已存在其他碰撞盒
            {
                switch (direction)//如果存在，则将监测点还原
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

    public void SetupRoom(Room newRoom,Vector3 roomPosition)//造门
    {
        newRoom.roomUp = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayer);//判断房间上方是否存在其他房间，将检测结果传给Room.cs
        newRoom.roomDown = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayer);
        newRoom.roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayer);
        newRoom.roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayer);

        newRoom.UpdateRoom(xOffset,yOffset);//将unity中设定的xy偏移值传入rom.cs
    }

    public void SetupWall(Room newRoom, Vector3 roomPosition)//造墙
    {
        switch (newRoom.doorNumber)//根据门数生成对应墙面
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
    //    // 获取房间最大（远）值，并将最远房间赋值给endRoom
    //    foreach (var room in rooms)
    //    {
    //        if (room.stepToStart > maxStep)
    //        {
    //            maxStep = room.stepToStart;
    //            endRoom = room.gameObject;
    //        }
    //    }
    //    // 获取最远和次远房间
    //    foreach (var room in rooms)
    //    {
    //        if (room.stepToStart >= maxStep - 1)
    //            farRooms.Add(room.gameObject);
    //    }
    //    // 获取最远和次远房间中的单门房间，如果有那么替换之前的endRoom赋值
    //    foreach (var room in farRooms)
    //    {
    //        if (room.GetComponent<Room>().doorNumber == 1)
    //            endRoom = room;
    //    }
    //}

    public void FindEndRoom()//找到最远房间，并制造单连的boss房间
    {
        //if(room.transform.position.sqrMagnitude> endRoom.transform.position.sqrMagnitude)//循环检测每个房间距离原点的向量长度，将最远的设为终点
        //{
        //    endRoom = room.gameObject;
        //}

        // 获取房间最大（远）值，并将最远房间赋值给endRoom
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
                endRoomNumber = i;//记录下endroom的房间号
            }
        }
        generetorPoint.position = endRoom.transform.position;//将生成点的位置移到endroom的位置

        while(true)//生成一个单连endroom的房间作为bossroom
        {
            ChangePointPos();
            AddRooms();
            SetupRoom(rooms.Last(), rooms.Last().transform.position);
            bossRoom = rooms.Last().gameObject;
            if(bossRoom.GetComponent<Room>().doorNumber > 1)//不满足单连条件则删除生成的房间
            {
                Destroy(bossRoom);
                rooms.RemoveAt(rooms.Count);
                generetorPoint.position = endRoom.transform.position;
            }
            else { break;}
        }
        endRoom.GetComponent<Room>().doorNumber = 0;//将endroom的门计数器归零
        SetupRoom(rooms[endRoomNumber], rooms[endRoomNumber].transform.position);//重新在endroom处生成一次门
    }
}

[System.Serializable]//使该类可以被unity系统识别
public class WallType
{
    public GameObject singleLeft, singleRight, singleUp, singleDown,
                      doubleUL, doubleUR, doubleUD, doubleRL, doubleDR, doubleDL,
                      tripleURL, tripleUDR, tripleUDL, tripleDRL,
                      fourDoors;
}