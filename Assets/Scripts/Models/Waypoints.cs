using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Waypoints : MonoBehaviour
{
    public string roomID;
    public List<Transform> waypoints;

    //update when some object change in the edit mode
    void Update()
    {
        if (!Application.isPlaying)
        {
            Transform[] temp = GetComponentsInChildren<Transform>();

            if (temp.Length > 0)
            {
                waypoints.Clear();
                int index = 0;

                foreach (Transform point in temp)
                {
                    if (point != transform)
                    {
                        index++;
                        point.name = "Point_" + index;
                        waypoints.Add(point);
                    }
                }
            }
        }
    }

    // void OnDrawGizmosSelected()
    // {
    //     if (waypoints.Count > 0)
    //     {
    //         //draw sphere for each point
    //         Gizmos.color = Color.green;
    //         foreach (Transform point in waypoints)
    //         {
    //             Gizmos.DrawSphere(point.position, 0.5f);
    //         }

    //         //draw lines
    //         Gizmos.color = Color.red;
    //         for (int i = 0; i < waypoints.Count - 1; i++)
    //         {
    //             Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
    //         }
    //     }
    // }
    
    void OnDrawGizmos()
    {
        if (waypoints.Count > 0)
        {
            //draw sphere for each point
            Gizmos.color = Color.green;
            foreach (Transform point in waypoints)
            {
                Gizmos.DrawSphere(point.position, 0.5f);
            }

            //draw lines
            Gizmos.color = Color.red;
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}
