using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
        This is am Implementation of dijkstra's algorithm in C# 
        This takes int a 2D array "int[,] matrix" which should be
        an adjacnecy matrix for the graph.
        The "int source", and "int sink" are the start and finish nodes
        for the algorihtm to find the shortest path between
    */
    public int[] dijkstraShortestPath(int[,] matrix, int source, int sink) {
        int graphSize = matrix.GetLength(0); //Size of graph

        int[] vertexPredecessors = new int[graphSize]; //node predecessors
        bool[] checkedSet = new bool[graphSize]; //node checked flag
        int[] distanceValues = new int[graphSize]; //nodes distance from source node

        //Setting all distance values to infinity
        for(int i = 0; i < matrix.GetLength(0); i++) {
            distanceValues[i] = System.Int32.MaxValue;
            checkedSet[i] = false;
        }
        distanceValues[source] = 0; //Source is 0 from its self


        //You run algorithm one less then the size of the graph
        //If you run again and it returns true then you have a negative closed loop in the graph
        //Which is impossible to have in our situation so no need to check for 
        for(int i = 0; i < graphSize-1; i++) {
            //Get the lowest value in the "distanceValues" that "checkSet" flag is set to false
            int minIndex = minDistance(distanceValues, checkedSet);

            checkedSet[minIndex] = true; //Setting checked on vertex to "true"

            for(int j = 0; j < graphSize; j++) {
                //This If statement is checking for 3 things
                //1 - If "j" has not been checked yet
                //2 - If there is an edge between verticies at "minIndex" and "j"
                //3 - If The distance from minIndex + the edge from minIndex vertex to j is less than the current calculated distance to j
                //If all of these pass. Then we put the distance value of "j" as === distance to "minIndex" + edge of "minIndex" and "j"
                //We also keep track of the node we took to get to the shortest path to vertext "j" so we store "minIndex" in the "vertexPredecessors" array at index "j"
                if(!checkedSet[j] && matrix[minIndex, j] != 0 &&
                    distanceValues[minIndex] != System.Int32.MaxValue &&
                    distanceValues[minIndex] + matrix[minIndex, j] < distanceValues[j]) {
                        distanceValues[j] = distanceValues[minIndex] + matrix[minIndex, j];
                        vertexPredecessors[j] = minIndex;
                }
            }
        }

        /*Now that the algorithm is complete we must build the path starting from the 
            Sink and working our way to the source Using the "vertexPredessesors array*/
        List<int> pathList = new List<int>();
        int indexPtr = sink; //Points to the current vertex 
        while(indexPtr != source) {
            pathList.Insert(0, indexPtr);
            indexPtr = vertexPredecessors[indexPtr];
        }
        pathList.Insert(0, source); //Adding the source to the path


        return pathList.ToArray();
    }
    
    //Method gets the smallest distance in the distances array that hasn't been checked yet
    private int minDistance(int[] distances, bool[] checkedSet) {
        int minValue = System.Int32.MaxValue;
        int minIndex = 1;

        //Simple search for smallest number in distances array
        for(int i = 0; i < distances.Length; i++) {
            if(!checkedSet[i] && distances[i] <= minValue) { //Checks if lower && if it hasn't been checked yet
                minValue = distances[i];
                minIndex = i;
            }
        }
        return minIndex;
    }
}
