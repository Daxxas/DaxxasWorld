using UnityEngine;

public class MoveUtils
{
    private float m_SmoothedSpeed;

    private const float k_MinSmoothSpeed = 4.0f;
    private const float k_TargetCatchupTime = 0.1f;

    
    public static void SmoothMove(Transform moveTransform, Vector3 targetTransformPosition, Quaternion targetTransformRotation, float timeDelta, ref float closingSpeed, float maxAngularSpeed)
    {
        var posDiff = targetTransformPosition - moveTransform.position;
        var angleDiff = Quaternion.Angle(targetTransformRotation, moveTransform.rotation);
        float posDiffMag = posDiff.magnitude;

        if (posDiffMag > 0)
        {
            closingSpeed = Mathf.Max(closingSpeed, Mathf.Max(k_MinSmoothSpeed, posDiffMag / k_TargetCatchupTime));

            float maxMove = timeDelta * closingSpeed;
            float moveDist = Mathf.Min(maxMove, posDiffMag);
            posDiff *= (moveDist / posDiffMag);

            moveTransform.position += posDiff;

            if( moveDist == posDiffMag )
            {
                //we capped the move, meaning we exactly reached our target transform. Time to reset our velocity.
                closingSpeed = 0;
            }
        }
        else
        {
            closingSpeed = 0;
        }

        if (angleDiff > 0)
        {
            float maxAngleMove = timeDelta * maxAngularSpeed;
            float angleMove = Mathf.Min(maxAngleMove, angleDiff);
            float t = angleMove / angleDiff;
            moveTransform.rotation = Quaternion.Slerp(moveTransform.rotation, targetTransformRotation, t);
        }
    }

    
    public static void SmoothMove(Transform moveTransform, Transform targetTransform, float timeDelta, ref float closingSpeed, float maxAngularSpeed)
    {
        var posDiff = targetTransform.position - moveTransform.position;
        var angleDiff = Quaternion.Angle(targetTransform.transform.rotation, moveTransform.rotation);
        float posDiffMag = posDiff.magnitude;

        if (posDiffMag > 0)
        {
            closingSpeed = Mathf.Max(closingSpeed, Mathf.Max(k_MinSmoothSpeed, posDiffMag / k_TargetCatchupTime));

            float maxMove = timeDelta * closingSpeed;
            float moveDist = Mathf.Min(maxMove, posDiffMag);
            posDiff *= (moveDist / posDiffMag);

            moveTransform.position += posDiff;

            if( moveDist == posDiffMag )
            {
                //we capped the move, meaning we exactly reached our target transform. Time to reset our velocity.
                closingSpeed = 0;
            }
        }
        else
        {
            closingSpeed = 0;
        }

        if (angleDiff > 0)
        {
            float maxAngleMove = timeDelta * maxAngularSpeed;
            float angleMove = Mathf.Min(maxAngleMove, angleDiff);
            float t = angleMove / angleDiff;
            moveTransform.rotation = Quaternion.Slerp(moveTransform.rotation, targetTransform.rotation, t);
        }
    }
}
