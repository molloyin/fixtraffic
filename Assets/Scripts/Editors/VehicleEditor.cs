using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(Vehicle))]
    public class VehicleEditor : Editor
    {
        private Vehicle vehicle;

        private void OnEnable()
        {
            vehicle = (Vehicle) target;
        }

        private void OnDestroy()
        {
            if (Application.isEditor && vehicle == null)
                vehicle.controller.vehicles.Remove(vehicle);
        }
    }
}