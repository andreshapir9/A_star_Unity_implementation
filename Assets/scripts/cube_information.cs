using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class cube_information : MonoBehaviour
{
    public int value//1=entrance, 2=exit, 3=path, 4=wall, 5=path_taken
    {get; set;}
    //coordinates;
    public int x,y;
    //vector of game objects neighbors
    public List<GameObject> neighbors = new List<GameObject>();

    public bool visited = false;
    public int distance;
    public GameObject parent;

    public float GCost;
    public float HCost;
    public float FCost;

    //function to print neighbors of each cube
    public void print_neighbors()
    {
        foreach(GameObject cube in neighbors)
        {
            Debug.Log(cube.GetComponent<cube_information>().value);
        }
    }
    public void CalculateFCost()
    {
        FCost = GCost + HCost;
    }
    //function reset cost
    public void reset_cost()
    {
        GCost = 0;
        HCost = 0;
        CalculateFCost();
    }
    //function to reset all variables
    public void reset_all()
    {
        visited = false;
        distance = 0;
        parent = null;
        reset_cost();
        neighbors.Clear();
        
    }

     
}