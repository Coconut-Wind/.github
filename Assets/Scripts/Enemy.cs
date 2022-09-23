using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Movement
{
    public int health = 2; //生命值
    public int maxChaseDistance = 2; //距离营地的最大值
    public int maxZoomDistance = 2; //距离营地的最大值
    public int maxWatchingDistance = 2; //官网距离到达离营最大值后，使其不返回营地的与玩家距离最大值
    private Vector2Int homePosition;
    private int state = 0; // 0不动 1追赶 2返回;


    public override void init(Map map, Vector2Int index)
    {
        base.init(map, index);
        homePosition = index;
    }
    
    private struct Node
    {
        public Vector2Int pos;
        public int step;
        public int last;
        public Node(Vector2Int pos, int step, int last)
        {
            this.pos = pos;
            this.step = step;
            this.last = last;
        }
    }

    //追赶玩家
    public void ChasePlayer()
    {
        
        Node nA = GetNextPosition(GetIndex(), homePosition); //从自身到营地
        Node nB = GetNextPosition(GetIndex(),  GameManager.GM.GetPlayerPosition()); //从自身到玩家
        Node nC = GetNextPosition(GameManager.GM.GetPlayerPosition(), homePosition); //从营地到玩家
        //Debug.Log("从自身到营地, dis:"+ (nA.step) + ", pos:" + nA.pos);
        //Debug.Log("从自身到玩家, dis:"+ (nB.step) + ", pos:" + nB.pos);
        //Debug.Log("从营地到玩家, dis:"+ (nC.step) + ", pos:" + nC.pos);
        Vector2Int nextPos = Vector2Int.zero;

        if (nC.step <= maxZoomDistance)
        {
            state = 1;
            
            
        }
        else if (nA.step > maxZoomDistance)
        {
            state = 2;
        }
        else if (nA.step == maxZoomDistance )
        {
            if (nB.step <= maxWatchingDistance)
            {
                state = 0;
            }
            else
            {
                state = 2;
            }
        }
        //else 保持上一次的状态


        if (state == 1)
        {
            nextPos = nB.pos;
        }
        else if (state == 2)
        {
            nextPos = nA.pos;
        }
        else if (state == 0)
        {
            nextPos = GetIndex();
        }

        Debug.Log("next position:" + nextPos);
        WalkTo(nextPos);
    }

    //获取要前往的下一个点
    private Node GetNextPosition(Vector2Int startPos, Vector2Int targetPos)
    {
        if (startPos == targetPos)
        {
            //Debug.Log(startPos + "->" + targetPos +" dis:"+ 0 + ", pos:" + targetPos);
            return new Node(targetPos, 0, 0);
        }

        //BFS
        List<Node> list = new List<Node>();
        Node first = new Node(startPos, 0, 0);

        bool[,] map = new bool[mapShape.y, mapShape.x];
        map[startPos.x, startPos.y] = true;
        
        list.Add(first);

        //bool isFound = false;
        int head = 0, tail = 1;
        int dis = 0;
        bool isFound = false;

        while (head < tail && !isFound)
        {
            Node curr = list[head];
            Cell currCell = mapObject.GetCellByIndex(curr.pos);
            List<Cell> adjs = currCell.GetAdjCellList(); // 获取邻接表

            //枚举所有可行路径
            for (int i = 0; i < adjs.Count; i++)
            {
                //当前位置是否检索过？
                Vector2Int npos = adjs[i].GetIndex();
                int id = npos.x + npos.y * mapObject.GetMapShape().y;
                if (map[ npos.x, npos.y])
                {
                    continue;
                }
                else
                {
                    list.Add(new Node(npos, curr.step + 1, head));
                    tail++;
                
                    //比对玩家位置
                    if (targetPos == npos)
                    {
                        dis = curr.step + 1;
                        //Debug.Log(startPos + "->" + targetPos +" dis:"+ dis + ", pos:" + npos);
                        isFound = true;
                        break;
                    }

                    //添加搜索记录
                    //searchPointIds.Add(id);
                    map[ npos.x, npos.y] = true;
                }
                
            }
            head++;
        }

        Node node = list[tail - 1];
        //int step = node.step;

        //回溯，找到第二个点
        while (node.last != 0)
        {
            node = list[node.last];
        }
        //node.step = step + 1;
        return new Node(node.pos, dis, 0);
    }

    public override void WalkTo(Vector2Int to)
    {
        Debug.Log("Walk to " + to);
        targetPos = mapObject.AdjustPosition(to.y, to.x);
        SetIndex(to.x, to.y);
        StartCoroutine(Walk());
        
    }

    IEnumerator Walk()
    {
        Vector2 substract = targetPos - (Vector2)transform.position;
        Vector2 targetPosDir = (substract).normalized;
        float dis = Vector2.Distance(transform.position, targetPos);
        float num = dis / speed;
        Vector2 per = substract / num;
        Debug.Log(dis + ", " + num);
        for (int i = 0; i < num-1; i++)
        {
            transform.position = transform.position + (Vector3)per;
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = targetPos;

        //轮到敌方回合
        //GameManager.GM.nextTurn();
        
        yield return new WaitForSeconds(0.05f);
    }
}