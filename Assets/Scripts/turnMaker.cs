using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnMaker : MonoBehaviour
{
    //Fields
    [SerializeField] private Waypoint[] entryWaypoints;
    [SerializeField] private Transform intersectionMiddle;  
    [SerializeField] private GameObject turnAnchor;

    [SerializeField] private int curveNodeNum = 3;
    private Transform tempPosSource = null;
    private Transform tempPosSink = null;
    private Transform tempPosMid = null;
    

    [SerializeField] private Transform sourceToB;
    [SerializeField] private Transform bToSink;
    [SerializeField] private Transform curvePoint;
    
    [SerializeField] private GameObject turnObj;

    // Start is called before the first frame update
    void Start()
    {
        //On start we want to create the turns
        //Loop through the "entryWaypoints" and create the turns then save them to the appropreate "pathToNext"
        //for(int i = 0; i < entryWaypoints.Length; i++) {
        for(int i = 0; i < 1; i++) {
            List<Waypoint> successors = entryWaypoints[i].GetSuccessorsWaypoints();

            foreach(Waypoint sink in successors) {
               //Create curve from //source -> sink
               Waypoint source = entryWaypoints[i];

                Vector3 posB = new Vector3(source.transform.position.x,0,sink.transform.position.z);  //Pos for B
                Vector3 posD = new Vector3(sink.transform.position.x,0,source.transform.position.z);  //Pos for D
                Vector3 mid = new Vector3(intersectionMiddle.position.x, 0, intersectionMiddle.position.z); //Pos for Mid

                float bDist = Vector3.Distance(posB, mid);
                float dDist = Vector3.Distance(posD, mid);

                GameObject tempBTransform;
                if(bDist < dDist) {
                    tempBTransform = Instantiate(this.turnAnchor, posB, Quaternion.identity);
                } else {
                    tempBTransform = Instantiate(this.turnAnchor, posD, Quaternion.identity);
                }
                
                //Setting tranforms
                tempPosMid = tempBTransform.transform;
                tempPosSource = source.transform;
                tempPosSink = sink.transform;

                //Now we create the curve 
                float tickPercent = 1f/ (float)(curveNodeNum+1); //How are along the curve u move per tick
                float interpPoint = tickPercent; //Current point on curve

                for(int j = 0; j < curveNodeNum; j++) {
                    sourceToB.position = Vector3.Lerp(tempPosSource.position, tempPosMid.position, interpPoint);
                    bToSink.position = Vector3.Lerp(tempPosMid.position, tempPosSink.position, interpPoint);
                    curvePoint.position = Vector3.Lerp(sourceToB.position, bToSink.position, interpPoint);

                    //Spawning the object in and adding it to the LIST of turning objects
                    GameObject newTurnObj = Instantiate(turnObj, curvePoint.position, Quaternion.identity);


                    //INC turn point
                    interpPoint += tickPercent;
                }

           }
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
