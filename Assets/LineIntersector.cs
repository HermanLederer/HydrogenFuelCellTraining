using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineIntersector : MonoBehaviour
{
	private void OnDrawGizmos()
	{
        // Variables
        Vector3 rayPos = new Vector3(0f, 3f, 0f);
        Vector3 rayDirection = new Vector3(0f, 1f, 0f);
        Vector3 spherePos = new Vector3(0f, 10f, 0f);
        Vector3 sphereDirection = new Vector3(0f, -1f, 0f);
        float sphereAngle = 30f;
        float sphereRadius = 5f;

        // Solving
        float distToVolume = 0;
        float distThroughVolume = 0;

        Vector3 rayToSphereDirection = spherePos - rayPos;
        float tMiddle = Vector3.Dot(rayToSphereDirection, rayDirection);
        Vector3 posMiddle = rayPos + rayDirection * tMiddle;
        float distanceSphereToTMiddle = Vector3.Magnitude(spherePos - posMiddle);

        if (distanceSphereToTMiddle < sphereRadius)
        {
            float distancePosMiddleToSphereEdge = Mathf.Sqrt(sphereRadius * sphereRadius - distanceSphereToTMiddle * distanceSphereToTMiddle);
            float distancePosMiddleToCutEdge = 

            distToVolume = tMiddle - distancePosMiddleToSphereEdge;
            distThroughVolume = distancePosMiddleToSphereEdge * 2;
        }

        // Cone
        Quaternion quat;
        quat = Quaternion.AngleAxis(sphereAngle / 2, sphereDirection);
        Vector3 targetN = quat * sphereDirection;

        quat = Quaternion.AngleAxis(sphereAngle / 2, sphereDirection);
        Vector3 targetE = quat * sphereDirection;

        quat = Quaternion.AngleAxis(-sphereAngle / 2, sphereDirection);
        Vector3 targetS = quat * sphereDirection;

        quat = Quaternion.AngleAxis(-sphereAngle / 2, sphereDirection);
        Vector3 targetW = quat * sphereDirection;

        Gizmos.DrawRay(spherePos, targetN * sphereRadius);
        Gizmos.DrawRay(spherePos, targetE * sphereRadius);
        Gizmos.DrawRay(spherePos, targetS * sphereRadius);
        Gizmos.DrawRay(spherePos, targetW * sphereRadius);

        // Drawing
        Gizmos.DrawWireSphere(spherePos, sphereRadius);

		Gizmos.DrawLine(rayPos, rayPos + rayDirection * distToVolume);
		Gizmos.DrawWireSphere(rayPos + rayDirection * distToVolume, 0.1f);
		Gizmos.DrawLine(rayPos + rayDirection * distToVolume, rayPos + rayDirection * (distToVolume + distThroughVolume));
		Gizmos.DrawWireSphere(rayPos + rayDirection * (distToVolume + distThroughVolume), 0.1f);
	}
}
