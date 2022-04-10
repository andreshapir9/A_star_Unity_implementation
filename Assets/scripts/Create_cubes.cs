using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// A Data Structure for queue used in BFS
public class Create_cubes : MonoBehaviour
{   //struct coordinate 
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
    int level =1;
    GameObject[][] map = new GameObject[50][];
    coordinate entrance = new coordinate(0, 0);
    coordinate exit = new coordinate(45, 35);
    List<GameObject> path = new List<GameObject>();
    List<GameObject> enemies = new List<GameObject>();
    List<int> enemies_index = new List<int>();

    GameObject canvas;
    void Start()
    {
        //game object jagged array
        create_map(ref map);
        add_entrance_exit(ref map,entrance.x,entrance.y,exit.x,exit.y);
        canvas = GameObject.Find("Canvas");
        Debug.Log(canvas.transform.position);

        Camera.main.transform.position = new Vector3(map.Length-20, map.Length-5, map.Length/2);
        //set camaras x rotation to 90
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
        //vectors used in bfs t
        //bfs(map);
        //print path

        add_walls_randomly(ref map);
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            List<GameObject> path = find_shortest_path(map,entrance.x,entrance.y,exit.x,exit.y);
            if(path!=null)
            {
                GameObject temp = path[path.Count - 1];
                print_path_list(temp, ref path);
                //print path was found and path length is:
                Debug.Log("path was found and path length is: " + path.Count);
                //StartCoroutine(create_enemy(path));
                instantiate_enemy(path[0].transform.position, path);
            }
           
        }
        //if key R is pressed reset map
        if (Input.GetKeyDown("r"))
        {
            reset_map(ref map);
            path.Clear();
            update_enemy_paths();
        }
        //if key C is pressed test enemy path
        if (Input.GetKeyDown("c"))
        {
            foreach (GameObject enemy in enemies)
            {
                Debug.Log("we are testing :");
                enemy.GetComponent<Enemy_basic>().test();
            }
        }
    }
    //function takes map and fills it with 8 cubes in each index
    void create_map(ref GameObject[][] map)
    {
        for (int i = 0; i < map.Length; i++)
        {
            GameObject[] temp = new GameObject[map.Length];
            for (int j = 0; j < map.Length; j++)
            {
                //create cube
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //set position
                cube.transform.position = new Vector3(i, 0, j);
                cube.transform.SetParent(this.transform);
                temp[j] = cube;
                //cube example = ScriptableObject.CreateInstance<cube>();
                cube.AddComponent<cube_information>();
                cube.GetComponent<cube_information>().value = 3;
                //set the distance to j+i
                cube.GetComponent<cube_information>().distance = j + i;
                cube.GetComponent<cube_information>().x = i;
                cube.GetComponent<cube_information>().y = j;

            }
            map [i] = temp;
        }
    }
    //function takes map adds entrance and exit
    void add_entrance_exit(ref GameObject[][] map,int entrance_X, int entrance_Y, int exit_X, int exit_Y)
    {
        //set entrance
        map[entrance_X][entrance_Y].GetComponent<cube_information>().value = 1;
        //set entrance color to green
        map[entrance_X][entrance_Y].GetComponent<Renderer>().material.color = Color.green;
        //set exit
        map[exit_X][exit_Y].GetComponent<cube_information>().value = 2;
        //set exit color to red
        map[exit_X][exit_Y].GetComponent<Renderer>().material.color = Color.red;
    }
    //implement A* algorithm on map
    //function takes map and returns queue of shortest path
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
    bool isValid(int row, int col)
    {

        // If cell lies out of bounds
        if (row < 0 || col < 0 ||
            row >= map.Length || col >= map[0].Length)
            return false;
        // Otherwise
        return true;
    }
    //function takes map and print the distance of each cube
    void print_distance(GameObject[][] map)
    {
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map.Length; j++)
            {
                Debug.Log(map[i][j].GetComponent<cube_information>().distance);
            }
        }
    }
    //takes a map and assigns the neighbors to each node
    void instantiante_neighbors(ref GameObject[][] map){
        // (0, -1), (0, 1), (-1, 0), (1, 0), (-1, -1), (-1, 1), (1, -1), (1, 1)
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
    void add_walls_randomly(ref GameObject[][] map){
        //add walls to map
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map.Length; j++)
            {
                //if random number is less than 0.2
                if (Random.Range(0, map.Length) < 10 && map[i][j].GetComponent<cube_information>().value == 3)
                {
                    //set value to 4
                    map[i][j].GetComponent<cube_information>().value = 4;
                    //set color to black
                    map[i][j].GetComponent<Renderer>().material.color = Color.black;
                }
            }
        }
    }

    float calcl_H_cost(coordinate start, coordinate end, coordinate current)
    {
       return Mathf.Pow(current.x - end.x, 2) + Mathf.Pow(current.y - end.y, 2);
    }
    void reset_map(ref GameObject[][] map){
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map.Length; j++)
            {
                if(map[i][j].GetComponent<cube_information>().value != 1 && map[i][j].GetComponent<cube_information>().value != 2){
                    map[i][j].GetComponent<cube_information>().value = 3;
                    //make blocks white
                    map[i][j].GetComponent<Renderer>().material.color = Color.white;
                    map[i][j].GetComponent<cube_information>().reset_all();
                }
            }
        }
        add_walls_randomly(ref map);
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
    


    void print_path(GameObject current){
        Debug.Log("(" + current.GetComponent<cube_information>().x + "," + current.GetComponent<cube_information>().y + ")");
        if(current.GetComponent<cube_information>().parent != null){
            print_path(current.GetComponent<cube_information>().parent);
            current.GetComponent<Renderer>().material.color = Color.blue;
        }
    }
   
    void print_path_list(GameObject current, ref List<GameObject> list){
        if(current.GetComponent<cube_information>().parent != null){
            print_path_list(current.GetComponent<cube_information>().parent, ref list);
            current.GetComponent<Renderer>().material.color = Color.blue;
            Debug.Log("(" + current.GetComponent<cube_information>().x + "," + current.GetComponent<cube_information>().y + ")");
        }
    }
    
    void instantiate_enemy(Vector3 position, List<GameObject> list){
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        enemy.transform.position = position;
        enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1, enemy.transform.position.z);
        enemy.AddComponent<Enemy_basic>();
        enemy.GetComponent<Renderer>().material.color = Color.red;
       // enemy.tag = "enemy";
        enemy.GetComponent<SphereCollider>().radius = 0.5f;
        enemies.Add(enemy);
        enemy.GetComponent<Enemy_basic>().path = list;
        enemy.GetComponent<Enemy_basic>().destination = map[exit.x][exit.y];
    }

    void instantiate_enemy_per_level( List<GameObject> path, int level){
         if(canvas.transform.GetChild(0).GetComponent<statistics>().level==1){
            for(int i = 0;i<level*10;i++){
            }
         }
    }
    void create_enemy_level1_test(){
       
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        enemy.AddComponent<Enemy_basic>();
        enemy.GetComponent<Enemy_basic>().set_enemy(100,4f,1, path, map[exit.x][exit.y], Color.red, map);
        enemies.Add(enemy);
    
    }


    void update_enemy_paths(){
        foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<Enemy_basic>().Update_path(map);
            }
    }
}
