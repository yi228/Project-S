using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aoiti.Pathfinding; //import the pathfinding library 
using static UnityEngine.GraphicsBuffer;
using static Define;

public class BossMovementController2D : MonoBehaviour
{
    [Header("Navigator options")]
    [SerializeField] float gridSize; //increase patience or gridSize for larger maps
    
    Pathfinder<Vector2> pathfinder; //the pathfinder object that stores the methods and patience
    [Tooltip("The layers that the navigator can not pass through.")]
    [SerializeField] LayerMask obstacles;
    [Tooltip("Deactivate to make the navigator move along the grid only, except at the end when it reaches to the target point. This shortens the path but costs extra Physics2D.LineCast")] 
    [SerializeField] bool searchShortcut =false; 
    [Tooltip("Deactivate to make the navigator to stop at the nearest point on the grid.")]
    [SerializeField] bool snapToGrid =false; 
    List <Vector2> path;
    List<Vector2> pathLeftToGo= new List<Vector2>();
    [SerializeField] bool drawDebugLines;

    private SingleMonsterBossController _mon;

    private float currentTime = 0f;
    private float updateTime = 0.15f;
    private bool canUpdate = true;

    void Start()
    {
        pathfinder = new Pathfinder<Vector2>(GetDistance,GetNeighbourNodes,1500); //increase patience or gridSize for larger maps
        _mon = GetComponent<SingleMonsterBossController>();
    }
    void Update()
    {
        SetUpdateTime();

        if(_mon._canChase && canUpdate)
        {
            currentTime = 0f;
            canUpdate = false;
            GetMoveCommand(_mon.Target.transform.position);
        }

        if (pathLeftToGo.Count > 0) //if the target is not yet reached
        {
            Vector3 dir =  (Vector3)pathLeftToGo[0]-transform.position ;
            transform.position += dir.normalized * _mon.boss.movingspeed * Time.deltaTime;
            if (((Vector2)transform.position - pathLeftToGo[0]).sqrMagnitude < _mon.boss.movingspeed * Time.deltaTime * _mon.boss.movingspeed * Time.deltaTime) 
            {
                transform.position = pathLeftToGo[0];
                pathLeftToGo.RemoveAt(0);
            }
        }
        else if(_mon._canChase && Vector2.Distance(_mon.Target.transform.position, transform.position) > 4)
        {
            if (canUpdate)
            {
                currentTime = 0f;
                canUpdate = false;
                GetMoveCommand(_mon.Target.transform.position);
            }
            if (pathLeftToGo.Count == 0)
                transform.position = Vector3.MoveTowards(transform.position, _mon.Target.transform.position, _mon.boss.movingspeed * Time.deltaTime);
        }

        if (drawDebugLines)
        {
            for (int i = 0; i < pathLeftToGo.Count - 1; i++) //visualize your path in the sceneview
            {
                Debug.DrawLine(pathLeftToGo[i], pathLeftToGo[i + 1]);
            }
        }
    }
    private void SetUpdateTime()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= updateTime)
            canUpdate = true;
    }
    private void GetMoveCommand(Vector2 target)
    {
        Vector2 closestNode = GetClosestNode(transform.position);
        if (pathfinder.GenerateAstarPath(closestNode, GetClosestNode(target), out path)) //Generate path between two points on grid that are close to the transform position and the assigned target.
        {
            if (searchShortcut && path.Count>0)
                pathLeftToGo = ShortenPath(path);
            else
            {
                pathLeftToGo = new List<Vector2>(path);
                if (!snapToGrid) pathLeftToGo.Add(target);
            }
        }
    }
    private Vector2 GetClosestNode(Vector2 target) 
    {
        return new Vector2(Mathf.Round(target.x/gridSize)*gridSize, Mathf.Round(target.y / gridSize) * gridSize);
    }
    private float GetDistance(Vector2 A, Vector2 B) 
    {
        return (A - B).sqrMagnitude; //Uses square magnitude to lessen the CPU time.
    }
    private Dictionary<Vector2,float> GetNeighbourNodes(Vector2 pos) 
    {
        Dictionary<Vector2, float> neighbours = new Dictionary<Vector2, float>();
        for (int i=-1;i<2;i++)
        {
            for (int j=-1;j<2;j++)
            {
                if (i == 0 && j == 0) continue;

                Vector2 dir = new Vector2(i, j)*gridSize;
                if (!Physics2D.Linecast(pos,pos+dir, obstacles))
                {
                    neighbours.Add(GetClosestNode( pos + dir), dir.magnitude);
                }
            }

        }
        return neighbours;
    }
    private List<Vector2> ShortenPath(List<Vector2> path)
    {
        List<Vector2> newPath = new List<Vector2>();
        
        for (int i=0;i<path.Count;i++)
        {
            newPath.Add(path[i]);
            for (int j=path.Count-1;j>i;j-- )
            {
                if (!Physics2D.Linecast(path[i],path[j], obstacles))
                {
                    
                    i = j;
                    break;
                }
            }
            newPath.Add(path[i]);
        }
        newPath.Add(path[path.Count - 1]);
        return newPath;
    }
}
