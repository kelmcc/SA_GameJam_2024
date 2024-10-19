using UnityEngine;

namespace Framework
{

    public static class JointExtensions
    {
        public static Vector3 GetWorldSpaceAnchor(this ConfigurableJoint joint)
        {
            return joint.transform.TransformPoint(joint.anchor);
        }

        public static void SetWorldSpaceAnchor(this ConfigurableJoint joint, Vector3 worldPoint)
        {
            joint.anchor = joint.transform.InverseTransformPoint(worldPoint);
        }

        /// <summary>
        /// Sets a joint's targetRotation to match a given local rotation.
        /// The joint transform's local rotation must be cached on Start and passed into this method.
        /// </summary>
        public static void SetTargetRotationLocal(this ConfigurableJoint joint, Quaternion targetLocalRotation, Quaternion startLocalRotation)
        {
            if (joint.configuredInWorldSpace)
            {
                Debug.LogError("SetTargetRotationLocal should not be used with joints that are configured in world space. For world space joints, use SetTargetRotation.", joint);
            }

            SetTargetRotationInternal(joint, targetLocalRotation, startLocalRotation, Space.Self);
        }

        /// <summary>
        /// Sets a joint's targetRotation to match a given world rotation.
        /// The joint transform's world rotation must be cached on Start and passed into this method.
        /// </summary>
        public static void SetTargetRotation(this ConfigurableJoint joint, Quaternion targetWorldRotation, Quaternion startWorldRotation)
        {
            if (!joint.configuredInWorldSpace)
            {
                Debug.LogError("SetTargetRotation must be used with joints that are configured in world space. For local space joints, use SetTargetRotationLocal.", joint);
            }

            SetTargetRotationInternal(joint, targetWorldRotation, startWorldRotation, Space.World);
        }

        static void SetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
        {
            // Calculate the rotation expressed by the joint's axis and secondary axis
            var right = joint.axis;
            var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
            var up = Vector3.Cross(forward, right).normalized;
            Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

            // Transform into world space
            Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

            // Counter-rotate and apply the new local rotation.
            // Joint space is the inverse of world space, so we need to invert our value
            if (space == Space.World)
            {
                resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
            }
            else
            {
                resultRotation *= Quaternion.Inverse(targetRotation) * startRotation;
            }

            // Transform back into joint space
            resultRotation *= worldToJointSpace;

            // Set target rotation to our newly calculated rotation
            joint.targetRotation = resultRotation;
        }

        /// <summary>
        /// Adjust ConfigurableJoint settings to closely match CharacterJoint behaviour
        /// </summary>
        public static void SetupAsCharacterJoint(this ConfigurableJoint joint)
        {
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            joint.angularXMotion = ConfigurableJointMotion.Limited;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularZMotion = ConfigurableJointMotion.Limited;
            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;

            joint.rotationDriveMode = RotationDriveMode.Slerp;
            var slerpDrive = joint.slerpDrive;

            slerpDrive.maximumForce = Mathf.Infinity;
            joint.slerpDrive = slerpDrive;
        }

        public static void SetupAngularJoint(this ConfigurableJoint joint, Rigidbody connectedAnchor, Vector3 primaryAxis, Vector3 secondaryAxis, Vector3 anchor, float lowXLimit, float highXLimit, float yLimit, float zLimit)
        {
            SoftJointLimit GetLimit(float angle)
            {
                return new SoftJointLimit()
                {
                    bounciness = 0,
                    contactDistance = 0,
                    limit = angle
                };
            }

            joint.axis = primaryAxis;
            joint.secondaryAxis = secondaryAxis;

            joint.anchor = anchor;
            joint.connectedBody = connectedAnchor;
            joint.autoConfigureConnectedAnchor = true;

            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Limited;
            joint.angularYMotion = ConfigurableJointMotion.Limited;
            joint.angularZMotion = ConfigurableJointMotion.Limited;

            joint.lowAngularXLimit = GetLimit(lowXLimit);
            joint.highAngularXLimit = GetLimit(highXLimit);

            joint.angularYLimit = GetLimit(yLimit);
            joint.angularZLimit = GetLimit(zLimit);
        }
    }

}