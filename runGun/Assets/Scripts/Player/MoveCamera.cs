using UnityEngine;

public class MoveCamera : MonoBehaviour
{
   // Reference to the desired camera position
   [SerializeField] Transform cameraPosition;

   void Update()
   {
       // Update the camera's position every frame to match the target position
       transform.position = cameraPosition.position;
   }
}