using UnityEngine;

public class GroupLights : MonoBehaviour
{
    public int greenDuration = 5;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Waypoint>() != null)
                Gizmos.DrawSphere(child.transform.position, 1f);
        }
    }
}