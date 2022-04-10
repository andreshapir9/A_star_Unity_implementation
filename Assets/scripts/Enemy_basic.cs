//using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_basic : MonoBehaviour
{
    //used to traverse path
    int index = 1;
    //health of enemy
    int health = 100;
    //speed of enemy
    public float speed = 4f;
    //list of path to follow
    public int damage = 1;
    public List<GameObject> path = new List<GameObject>();
    //destination of enemy
    public GameObject destination;
    //has enemy reached destination
    public bool has_reach_destination = false;

    bool updating_path = false;
     GameObject[][] map;
     public struct coordinate
    {
        public int x;
        public int y;
        public coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    void Start(){
    }

    void FixedUpdate()
    {
     //Debug.Log(transform.position);   
        //if enemy has not reached destination
        if(!updating_path){
            if (index < path.Count && !has_reach_destination)
            {   
                //move towards next point in path
                Vector3 target = path[index].transform.position;
                target.y = transform.position.y;
                transform.position = Vector3.MoveTowards(transform.position,target, Time.deltaTime * speed);
                //if enemy has reached next point in path
                if (Vector3.Distance(transform.position, target) < 0.05f)
                {
                    //increment index
                    transform.position = target;
                    index++;
                }
                //if enemy has reached destination
            }
            else{
                //has_reach_destination = true;
                    //destoy object
                    GameObject canvas = GameObject.Find("Canvas");
                    canvas.transform.GetChild(0).GetComponent<statistics>().Player_health-=1;
                    Debug.Log("we destory object");
                   // Destroy(gameObject);
            }
        }
            
    }
    public void set_enemy(int health, float speed, int damage, List<GameObject> path, GameObject destination, Color color, GameObject[][] map)
    {
        this.health = health;
        this.transform.position = new Vector3(path[0].transform.position.x, path[0].transform.position.y+1, path[0].transform.position.z);
        this.speed = speed;
        this.damage = damage;
        this.path = path;
        this.destination = destination;
        this.has_reach_destination = false;
        this.index = 1;
        this.GetComponent<Renderer>().material.color = color;
        this.map = map;
    }
    public void Update_path( GameObject[][] map){
        //set map
        this.map = map;
        //print mapsize
        Debug.Log(map.Length);
        updating_path = true;
        coordinate current_coordinate = new coordinate(path[index].GetComponent<cube_information>().x, path[index].GetComponent<cube_information>().y);
        //print current coordinate
        Debug.Log("this enemy is in " +current_coordinate.x + " " + current_coordinate.y);
        index =1;
        path.Clear();
        path = find_shortest_path(map, current_coordinate.x, current_coordinate.y, destination.GetComponent<cube_information>().x, destination.GetComponent<cube_information>().y);
        GameObject temp = path[path.Count - 1];
        print_path_list(temp,ref path);
        updating_path = false;
    }
    float calcl_H_cost(coordinate start, coordinate end, coordinate current)
    {
       return Mathf.Pow(current.x - end.x, 2) + Mathf.Pow(current.y - end.y, 2);
    }
    void set_FValue(ref GameObject[][] map){
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map.Length; j++)
            {
                //sets fcost to infinity
                map[i][j].GetComponent<cube_information>().FCost = Mathf.Infinity;
                //g score to 0
                map[i][j].GetComponent<cube_information>().GCost = 0;
            }
        }
    }
       bool isValid(int row, int col)
    {

        // If cell lies out of bounds
        if (row < 0 || col < 0 ||
            row >= map.Length || col >= map[0].Length)
            return false;
        // Otherwise
        return true;
    }
       List<GameObject> find_shortest_path(GameObject[][] map, int entrance_X, int entrance_Y, int exit_X, int exit_Y)
   {
        instantiante_neighbors(ref map);
        set_FValue(ref map);
        GameObject start_spot = map[entrance_X][entrance_Y];
        start_spot.GetComponent<cube_information>().reset_cost();
        GameObject exit_spot = map[exit_X][exit_Y];
        //exit_spot.GetComponent<cube_information>().reset_cost();
        //create open and close queues
        List<GameObject> open = new List<GameObject>();
        List<GameObject> closed = new List<GameObject>();
        //add start to open
        open.Add(start_spot);
        closed.Add(start_spot);
        //while open is not empty
        while(open.Count >0){
            //Debug.Log("open count: " + open.Count);
            GameObject current_spot = open[0];
            int current_index = 0;
            //itterate through open
            current_spot = get_lowest_f_cost(open, ref current_index);
            //remove current spot from open
            open.RemoveAt(current_index);
            //add current spot to closed
            closed.Add(current_spot);
            //if current spot is exit
            //print current index coordinates
           // Debug.Log(current_spot.GetComponent<cube_information>().x + "," + current_spot.GetComponent<cube_information>().y);
            if(current_spot == exit_spot){
                //return path
                return closed;
            }
            //vector of neighbors
            List<GameObject> neighbors = current_spot.GetComponent<cube_information>().neighbors;
            //loop through neighbors
            // if child is not in closed and is path
            // g cost = current spot g cost + 1
            // h cost =  ((child.position[0] - end_node.position[0]) ** 2) + ((child.position[1] - end_node.position[1]) ** 2)
            // calculate f cost
            // if child is not in open
            // add child to open
            for(int i = 0; i < neighbors.Count; i++){
                if(!closed.Contains(neighbors[i]) && neighbors[i].GetComponent<cube_information>().value != 4){
                    bool add_to_open = true;
                    foreach (GameObject cube in open){
                        if(open.Contains(neighbors[i]) ){ 
                            add_to_open = false;
                        }
                        if(neighbors[i].GetComponent<cube_information>().GCost >current_spot.GetComponent<cube_information>().GCost+ 1){
                            add_to_open = false;
                        }
                    }
                    if(add_to_open){
                        neighbors[i].GetComponent<cube_information>().GCost = current_spot.GetComponent<cube_information>().GCost + 1;

                        neighbors[i].GetComponent<cube_information>().HCost = calcl_H_cost(new coordinate(entrance_X, entrance_Y),new coordinate(exit_X, exit_Y), new coordinate(neighbors[i].GetComponent<cube_information>().x, neighbors[i].GetComponent<cube_information>().y));
                        neighbors[i].GetComponent<cube_information>().CalculateFCost();
                        open.Add(neighbors[i]);
                        //add curent as neighbor parent
                        neighbors[i].GetComponent<cube_information>().parent = current_spot;
                    }
                }
            }
        
        }
        return null;

   }

       void instantiante_neighbors(ref GameObject[][] map)
       {
       // int []dRow = { -1, 0, 1, 0, -1, 1, 1, -1 };
       // int []dCol = { 0, 1, 0, -1, -1, -1, 1, 1 };
       int []dRow = { -1, 0, 1, 0};
       int []dCol = { 0, 1, 0, -1 };
        //traverse map use drow and dcol to get neighbors
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map.Length; j++)
            {
                //traverse neighbors
                for (int k = 0; k < dCol.Length; k++)
                {
                    int adjx = i + dRow[k];
                    int adjy = j + dCol[k];
                    //if neighbor is valid
                    if (isValid(adjx, adjy))
                    {
                        //add neighbor to map
                        map[i][j].GetComponent<cube_information>().neighbors.Add(map[adjx][adjy]);
                    }
                }
            }
        }
    }
     GameObject get_lowest_f_cost(List<GameObject> list, ref int index){
        GameObject lowest = list[0];
        index = 0;
        for (int i = 0; i < list.Count; i++){
            if(list[i].GetComponent<cube_information>().FCost < lowest.GetComponent<cube_information>().FCost){
                lowest = list[i];
                index = i;
            }
        }
        return lowest;
    }
     void print_path_list(GameObject current, ref List<GameObject> list){
        if(current.GetComponent<cube_information>().parent != null){
            print_path_list(current.GetComponent<cube_information>().parent, ref list);
            current.GetComponent<Renderer>().material.color = Color.blue;
           // Debug.Log("(" + current.GetComponent<cube_information>().x + "," + current.GetComponent<cube_information>().y + ")");
        }
    }
    //function takes map returns coordinate x and y of square enemy is in
    public coordinate find_enemy(GameObject[][] map){
        coordinate enemy_coord = new coordinate(0,0);
        float minDistnace = float.MaxValue;
        for(int i = 0; i < map.Length; i++){
            for(int j = 0; j < map.Length; j++){
                float curdist = Vector2.Distance(new Vector2(map[i][j].transform.position.x, map[i][j].transform.position.z), new Vector2(transform.position.x, transform.position.z));
                if(minDistnace < curdist){
                    enemy_coord= new coordinate(i,j);
                    minDistnace = curdist;
                }
            }
        }
        return enemy_coord;
    }
    public void test(){
        Debug.Log("test");
    }
    



}

