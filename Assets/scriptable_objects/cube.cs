using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "cube")]
public class cube : ScriptableObject
{
    public int cube_value;//1=entrance, 2=exit, 3=path, 4=wall, 5=path_taken
}