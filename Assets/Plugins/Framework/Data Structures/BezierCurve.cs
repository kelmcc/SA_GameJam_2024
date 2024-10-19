using UnityEngine;

//Interpolation between 2 points with a Bezier Curve (cubic spline)
namespace Framework
{
    public class BezierCurve
    {

        public Vector3 StartPoint;
        public Vector3 StartControlPoint;
        public Vector3 EndControlPoint;
        public Vector3 EndPoint;

        public BezierCurve(Vector3 startPoint, Vector3 startControlPoint, Vector3 endPoint, Vector3 endControlPoint)
        {
            StartPoint = startPoint;
            StartControlPoint = startControlPoint;
            EndPoint = endPoint;
            EndControlPoint = endControlPoint;
        }

        public BezierCurve(Vector3 startPoint, Vector3 endPoint, Vector3 normal, float perpendicularOffset, float parallelOffset)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;

            Vector3 direction = startPoint.To(endPoint).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, normal);

            StartControlPoint = startPoint + perpendicular * perpendicularOffset + direction * parallelOffset;
            EndControlPoint = endPoint - perpendicular * perpendicularOffset - direction * parallelOffset;
        }

#if UNITY_EDITOR
        public void DrawGizmo()
        {
            DrawGizmo(Color.white);
        }

        public void DrawGizmo(Color colour)
        {
            UnityEditor.Handles.DrawBezier(StartPoint, EndPoint, StartControlPoint, EndControlPoint, colour, null, 1.5f);
        }

        public void DrawGizmo(Color colour, Color handleColour)
        {
            UnityEditor.Handles.DrawBezier(StartPoint, EndPoint, StartControlPoint, EndControlPoint, colour, null, 1.5f);

            DebugUtils.DrawLine(StartPoint, StartControlPoint, handleColour);
            DebugUtils.DrawLine(EndPoint, EndControlPoint, handleColour);
        }
#endif

        //The De Casteljau's Algorithm
        public Vector3 GetPosition(float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;

            //Layer 1
            Vector3 Q = oneMinusT * StartPoint + t * StartControlPoint;
            Vector3 R = oneMinusT * StartControlPoint + t * EndControlPoint;
            Vector3 S = oneMinusT * EndControlPoint + t * EndPoint;

            //Layer 2
            Vector3 P = oneMinusT * Q + t * R;
            Vector3 T = oneMinusT * R + t * S;

            //Final interpolated position
            Vector3 U = oneMinusT * P + t * T;

            return U;
        }

        Vector3 GetFirstDerivative(float t)
        {
            Vector3 C1 = (EndPoint - (3f * EndControlPoint) + (3f * StartControlPoint) - StartPoint);
            Vector3 C2 = ((3f * EndControlPoint) - (6f * StartControlPoint) + (3f * StartPoint));
            Vector3 C3 = ((3f * StartControlPoint) - (3f * StartPoint));

            return ((3f * C1 * t * t) + (2f * C2 * t) + C3);
        }

        Vector3 GetSecondDerivative(float t)
        {
            return (6 * t * (StartControlPoint + 3 * (EndControlPoint - EndPoint) - StartPoint) + 6 * (StartPoint - 2 * EndControlPoint + EndPoint));
        }

        float GetArcLengthIntegrand(float t)
        {
            //The derivative of cubic De Casteljau's Algorithm
            Vector3 dU = t * t * (-3f * (StartPoint - 3f * (StartControlPoint - EndControlPoint) - EndPoint));

            dU += t * (6f * (StartPoint - 2f * StartControlPoint + EndControlPoint));
            dU += -3f * (StartPoint - StartControlPoint);

            return dU.magnitude;
        }

        public float EstimateLength(int numIntervals = 10, bool highAccuracy = true)
        {
            return EstimateLength(0f, 1f, numIntervals, highAccuracy);
        }


        float EstimateLength(float tStart, float tEnd, int numIntervals = 10, bool highAccuracy = true)
        {
            //Get the length of the curve between two t values with Simpson's rule
            if (highAccuracy)
            {
                //Tthe resolution has to be even
                if (numIntervals % 2 == 1)
                {
                    numIntervals++;
                }

                //Now we need to divide the curve into sections
                float delta = (tEnd - tStart) / numIntervals;

                //The main loop to calculate the length

                //Everything multiplied by 1
                float endPoints = GetArcLengthIntegrand(tStart) + GetArcLengthIntegrand(tEnd);

                //Everything multiplied by 4
                float x4 = 0f;
                for (int i = 1; i < numIntervals; i += 2)
                {
                    float t = tStart + delta * i;

                    x4 += GetArcLengthIntegrand(t);
                }

                //Everything multiplied by 2
                float x2 = 0f;
                for (int i = 2; i < numIntervals; i += 2)
                {
                    float t = tStart + delta * i;

                    x2 += GetArcLengthIntegrand(t);
                }

                //The final length
                float length = (delta / 3f) * (endPoints + 4f * x4 + 2f * x2);

                return length;
            }
            else
            {
                //Get the length of the curve with a naive method where we divide the
                //curve into straight lines and then measure the length of each line

                //Divide the curve into sections
                float delta = (tEnd - tStart) / numIntervals;

                //The start position of the curve
                Vector3 lastPos = GetPosition(tStart);

                //Init length
                float length = 0f;

                //Move along the curve
                for (int i = 1; i <= numIntervals; i++)
                {
                    //Calculate the t value at this section
                    float t = tStart + delta * i;

                    //Find the coordinates at this t
                    Vector3 pos = GetPosition(t);

                    //Add the section to the total length
                    length += Vector3.Magnitude(pos - lastPos);

                    //Save the latest pos for next loop
                    lastPos = pos;
                }

                return length;
            }
        }

        public float GetTimeAtDistance(float distance)
        {
            return GetTimeAtDistance(distance, EstimateLength());
        }

        public float GetTimeAtDistance(float distance, float totalLength)
        {
            //Need a start value to make the method start
            //Should obviously be between 0 and 1
            //We can say that a good starting point is the percentage of distance traveled
            //If this start value is not working you can use the Bisection Method to find a start value
            //https://en.wikipedia.org/wiki/Bisection_method
            float t = distance / totalLength;

            //Need an error so we know when to stop the iteration
            float error = 0.001f;

            //We also need to avoid infinite loops
            int iterations = 0;

            while (true)
            {
                //Newton's method
                float tNext = t - ((EstimateLength(0f, t) - distance) / GetArcLengthIntegrand(t));

                //Have we reached the desired accuracy?
                if (Mathf.Abs(tNext - t) < error)
                {
                    break;
                }

                t = tNext;

                iterations += 1;

                if (iterations > 1000)
                {
                    break;
                }
            }

            return t;
        }

        public Line3[] DivideCurveIntoEqualSegments(int numSegments)
        {

            //Find the total length of the curve
            float totalLength = EstimateLength();

            //What's the length of one section?
            float sectionLength = totalLength / numSegments;

            //Init the variables we need in the loop
            float currentDistance = 0f + sectionLength;

            //The curve's start position
            Vector3 lastPos = StartPoint;

            Line3[] segments = new Line3[numSegments];


            for (int i = 1; i <= numSegments; i++)
            {
                //Use Newton–Raphsons method to find the t value from the start of the curve 
                //to the end of the distance we have
                float t = GetTimeAtDistance(currentDistance, totalLength);

                //Get the coordinate on the Bezier curve at this t value
                Vector3 pos = GetPosition(t);

                segments[i - 1] = new Line3(lastPos, pos);

                //Save the last position
                lastPos = pos;
                //Add to the distance traveled on the line so far
                currentDistance += sectionLength;
            }

            return segments;
        }

        public Vector3 GetDirection(float t)
        {
            return GetFirstDerivative(Mathf.Clamp01(t)).normalized;
        }

        //  public Vector3 GetVelocity (float t) {
        //       return transform.TransformPoint(
        //           Bezier.GetFirstDerivative(points[0], points[1], points[2], points[3], t)) - transform.position;
        //   }

        /*
    public InterpolationTransform GetTransform(float t)
    {
        
           MyVector3 interpolation_posA_handlePos = BezierLinear(posA, handlePos, t);
            MyVector3 interpolation_handlePos_posB = BezierLinear(handlePos, posB, t);

            MyVector3 finalInterpolation = BezierLinear(interpolation_posA_handlePos, interpolation_handlePos_posB, t);
         
         



        //Same as when we calculate t
        MyVector3 interpolation_1_2 = _Interpolation.BezierQuadratic(posA, handleB, handleA, t);
        MyVector3 interpolation_2_3 = _Interpolation.BezierQuadratic(posA, posB, handleB, t);

        MyVector3 finalInterpolation = _Interpolation.BezierLinear(interpolation_1_2, interpolation_2_3, t);

        //This direction is always tangent to the curve
        MyVector3 forwardDir = MyVector3.Normalize(interpolation_2_3 - interpolation_1_2);

        //A simple way to get the other directions is to use LookRotation with just forward dir as parameter
        //Then the up direction will always be the world up direction, and it calculates the right direction 
        Quaternion orientation = Quaternion.LookRotation(forwardDir.ToVector3());


        InterpolationTransform trans = new InterpolationTransform(finalInterpolation, orientation);

        return trans;
    }


    public Vector3 GetNormal(float t)
    {
        var tangent = GetTangent(t);
        var point = GetPoint(t);
        var accel = GetAcceleration(t);
        // we need to lerp up base don the control points
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = _points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        //var rotaSource = GetControlPointRotation(i);
        //var rotaDest = GetControlPointRotation(i + 2);
        //var lerp = Mathf.Lerp(rotaSource, rotaDest,t);      
        //var normalRotation = Quaternion.AngleAxis(lerp,tangent);

        var binormal = Vector3.Cross(tangent, accel).normalized;

        var normalOr = Vector3.Cross(tangent, binormal).normalized;


        Debug.DrawLine(point, point + accel * 5, Color.blue);
        Debug.DrawLine(point, point + binormal * 5,Color.black);
        Debug.DrawLine(point, point + normalOr * 5, Color.yellow);
        Debug.DrawLine(point, point + tangent * 5, Color.magenta);

        if (Vector3.Dot(tangent, accel) > 0)
            return Vector3.up;

        return normalOr;
        //return (normalRotation*up).normalized;

    }


 public Vector3 GetAcceleration(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = _points.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }

            return Bezier.GetSecondDerivative(_points[i], _points[i + 1], _points[i + 2],
                _points[i + 3], t).normalized;

        }



public Vector3 GetTangent(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = _points.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return
                Bezier.GetFirstDerivative(_points[i], _points[i + 1], _points[i + 2],
                    _points[i + 3], t).normalized;
        }

*/
    }
}