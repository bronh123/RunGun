using UnityEngine;

public class MovePlayerBehindCamera : MonoBehaviour
{
   // Reference to the desired camera position
   [SerializeField] Transform camPosition;
   [SerializeField] Transform orientation;
   [SerializeField] float positionOffset = .25f;

   void Update()
   {
       // Update the camera's position every frame to match the target position
       transform.position = camPosition.position - orientation.forward * positionOffset;
       transform.rotation = orientation.rotation;
   }
}

