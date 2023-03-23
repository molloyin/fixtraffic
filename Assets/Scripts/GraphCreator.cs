using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphCreator : MonoBehaviour
{
    //Fields
    [SerializeField] private Waypoint[] waypoints; //This is the way points on the graph
    private int[,] matrix; //Adjacency Matrix


    //Methods
    
    // Start is called before the first frame update
    void Start()
    {
        //Make sure the waypoints are sorted by ID
        this.waypoints = this.SortWayPointsById(this.waypoints);

        //Now we want to pre our graph
        this.matrix = new int[this.waypoints.Length, this.waypoints.Length];

        //LOGIC for Creating graph
        /* - Loop through all Waypoints 0->N
         * - Loop though Waypoint I's successor array
         *      - Find dist between I and successor
         *      - Add dist to graph as type Integer
         */
        for (int i = 0; i < this.waypoints.Length; i++)
        {
            List<int> successors = waypoints[i].successors;
            foreach(int id in successors) //Looping through successors of waypoints[i]
            {
                if(id != this.waypoints[i].id)
                {
                    //Calculating distance between waypoint[i] and waypoint[id]
                    int dist = Mathf.RoundToInt(Vector3.Distance(this.waypoints[i].transform.position, this.waypoints[id].transform.position));

                    this.matrix[i, id] = dist; 
                }
            }
        }

        Debug.Log(this.GraphToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Used for sorting waypoints by there ID (LOW->HIGH)
    private Waypoint[] SortWayPointsById(Waypoint[] waypoints)
    {
        for(int i = 0; i < waypoints.Length-1; i++)
        {
            for(int j = 0; j < waypoints.Length-1; j++)
            {
                if(waypoints[j].id > waypoints[j+1].id)
                {
                    Waypoint temp = waypoints[j];
                    waypoints[j] = waypoints[j+1];
                    waypoints[j + 1] = temp;
                }
            }
        }
        return waypoints;
    }

    //Creates a string that can be printed to the console.
    private string GraphToString()
    {
        string tempString = "";
        int size = this.matrix.GetLength(0);

        for (int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                tempString += this.matrix[i, j].ToString();

                if(j < size-1)
                {
                    tempString += ", ";
                }
            }
            tempString += "\n";
        }

        return tempString;
    }
}
