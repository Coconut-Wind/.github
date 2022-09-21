using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //����һ������
    private const int NONE = 0;     //��ʾ�����ߵĵط�
    private const int POINT = 1;    //��ʾ�����ߵĵط�
    private const int PLAYER = 2;
    private const int ENEMY = 3;
    private const int BUILDING = 4;
    private const int DESTINATION = 5;

    private const int WEST = 0;
    private const int NORTH_WEST = 1;
    private const int NORTH = 2;
    private const int NORTH_EAST = 3;
    private const int EAST = 4;
    private const int SOUTH_EAST = 5;
    private const int SOUTH = 6;
    private const int SOUTH_WEST = 7;
    

    private int[,] map = new int[25, 25];     //��Ϸ��ͼ
    private byte[,] lineMatrix = new byte[25, 25];      //��·�Ĵ洢����

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddElement(string element,int x, int y)
    {
        if (x >= 0 && x <= 25 && y >=0 && y <= 25)
        {
            switch (element)
            {
                case "Player":
                    map[x, y] = PLAYER;
                    break;
                case "Enemy":
                    map[x, y] = ENEMY;
                    break;
                case "None":
                    map[x, y] = NONE;
                    break;
                case "Building":
                    map[x, y] = BUILDING;
                    break;
                case "Point":
                    map[x, y] = POINT;
                    break;
                default:
                    break;
            }
        }
    }

    public void AddLine(int x, int y, int direction)
    {
        if (x >= 0 && x <= 25 && y >= 0 && y <= 25 && direction >=0 && direction <= 7)
        {
            lineMatrix[x, y] = UtilChangeBit(direction, 1, lineMatrix[x, y]);
        }
    }

    //�����ͨ�õİ�����Һ͵��ˣ�����x��y�Ǵ�����ʼ������꣬������ԭ����������
    //dx��dy������������ƶ�ȡֵ�Ǵ�-1��1
    public void MoveCharacter(int x, int y, int dx, int dy)
    {
        int character = map[x, y];  //�Ȼ�ȡ��˭
        if (character != PLAYER && character != ENEMY)  //����ָ��λ������һ���ˣ������ƶ�
        {
            return;
        }
        if (map[x + dx, y + dy] == POINT || map[x + dx, y + dy] == DESTINATION)      //��������
        {
            map[x + dx, y + dy] = character;
            map[x, y] = POINT;
        }
    }

    public void CreateMap()
    {
        AddElement("Point", 1, 24);
        AddElement("Point", 1, 23);
    }


    //�����õĺ�����ͨ������̨�������ͼ����,�������еĹ��߷�������ǰ��Ӹ�util
    private void UtilCheckMap()
    {
        string str = null;
        string newline = null;
        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                newline += (map[i, j] + " ");
            }
            str += newline;
            str += ("\n");
            newline = null;
        }
        Debug.Log(str);
    }

    private byte UtilChangeBit(int index, byte state, byte target)      //index��ָ�ڼ�λ��stateָ�ı��״̬��targetָĿ��
    {
        if (index >= 0 && index <= 7)
        {
            if (state == 0)
            {
                target &= (byte)~(1 << index);
            }
            else if (state == 1)
            {
                target |= (byte)~(1 << index);
            }
        }
        
        return target;
    }

    private bool UtilIsBitOn(int index, byte target)
    {
        if (index >= 0 && index <= 7)
        {
            return ((target & (1 << index)) == 0);
        }
        else
        {
            return false;
        }
    }
}