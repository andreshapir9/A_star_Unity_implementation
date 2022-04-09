using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_basic : MonoBehaviour
{
    public void follow_path(List<GameObject> path){
        //traverse path backwards
        //use move towards to move towards the next node
        //once reached, remove to next node from path
        //repeat
        for(int i = path.Count-1; i>=0; i--){
            while(this.transform.position != path[i].transform.position){
                this.transform.position = Vector3.MoveTowards(this.transform.position, path[0].transform.position, 0.1f);
                //debug the distance moved
                Debug.Log(Vector3.Distance(this.transform.position, path[0].transform.position));
            }
        }

    }  
}
