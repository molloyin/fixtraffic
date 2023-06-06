using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class TrafficLightsTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void GroupWaypoints()
    {
        GameObject obj = new GameObject("Traffic Controller Test");
        TrafficController controller = obj.AddComponent<TrafficController>();
        obj.transform.parent = null;
        
        var wp1 = controller.AddWaypoint().GetComponent<Waypoint>(); 
        var wp2 = controller.AddWaypoint(wp1).GetComponent<Waypoint>(); 
        var wp3 = controller.AddWaypoint(wp1).GetComponent<Waypoint>();

        Selection.objects = new Object[] {wp1.gameObject, wp2.gameObject, wp3.gameObject};
        var lights = controller.AddTrafficLights().GetComponent<TrafficLights>();
        
        Assert.IsTrue(lights.transform.childCount == 3);
        
        for (int i = 0; i < lights.transform.childCount; i++)
        {
            var child = lights.transform.GetChild(i);
            var wp = child.GetComponent<Waypoint>();
            Assert.IsTrue(wp == wp1 || wp == wp2 || wp == wp3);
        }
    }
}
