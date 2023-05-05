// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class TrafficLightManager : MonoBehaviour
// {
//     [SerializeField] private GraphCreator graphCreator;
//     [SerializeField] private float greenLightDuration = 10.0f; 
//     [SerializeField] private float yellowLightDuration = 2.0f;
//     [SerializeField] private float redLightDuration = 10.0f; 
//
//     private int[] currentLights; 
//     private float[] lightTimers; 
//
//     // Start is called before the first frame update
//     void Start()
//     {
//         // initialize the current lights to green
//         this.currentLights = new int[this.graphCreator.GetWaypoints().Length];
//         for(int i = 0; i < this.currentLights.Length; i++)
//         {
//             this.currentLights[i] = 1; // 2 is yellow, 1 is green, 0 is red
//         }
//
//         // initialize the light timers to the green light duration
//         this.lightTimers = new float[this.currentLights.Length];
//         for(int i = 0; i < this.lightTimers.Length; i++)
//         {
//             this.lightTimers[i] = this.greenLightDuration;
//         }
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         for(int i = 0; i < this.lightTimers.Length; i++)
//         {
//             this.lightTimers[i] -= Time.deltaTime;
//             if(this.lightTimers[i] < 0)
//             {
//                 // light switching
//                 if(this.currentLights[i] == 1) 
//                 {
//                     this.currentLights[i] = 2;
//                     this.lightTimers[i] = this.yellowLightDuration;
//                 }
//                 else if(this.currentLights[i] == 2) 
//                 {
//                     this.currentLights[i] = 0;
//                     this.lightTimers[i] = this.redLightDuration;
//                 }
//                 else 
//                 {
//                     this.currentLights[i] = 1;
//                     this.lightTimers[i] = this.greenLightDuration;
//                 }
//             }
//         }
//
//         //Update the traffic lights in the scene
//         Waypoint[] waypoints = this.graphCreator.GetWaypoints();
//         for(int i = 0; i < waypoints.Length; i++)
//         {
//             if(waypoints[i].hasTrafficLight) //Check if the intersection has a traffic light
//             {
//                 if(this.currentLights[i] == 1) //Green
//                 {
//                     waypoints[i].trafficLight.TurnGreen();
//                 }
//                 else if(this.currentLights[i] == 2) //Yellow
//                 {
//                     waypoints[i].trafficLight.TurnYellow();
//                 }
//                 else //Red
//                 {
//                     waypoints[i].trafficLight.TurnRed();
//                 }
//             }
//         }
//     }
// }