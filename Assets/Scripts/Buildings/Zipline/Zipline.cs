using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
public class Zipline : Building
{
    public ZiplinePole Point1;
    public ZiplinePole Point2;
    public float SegmentLength;
    public float MidOffset = 0.1f;
    public CinemachineImpulseSource StartImpact;

    private LineRenderer _lineRenderer;

    private float _accurateLength = 0;
    public float Length
    {
        get
        {
            if (_accurateLength == 0)
            {
                _accurateLength = CalculateAccurateLength();
            }

            return _accurateLength;
        }
    }

    private LineRenderer LineRenderer
    {
        get
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }
            return _lineRenderer;
        }
    }

    private Vector3 GetMid(Vector3 p1, Vector3 p2)
    {
        Vector3 mid = (p1 + p2) / 2f;

        float midH = (Point1.LineConnector.position.y + Point2.LineConnector.position.y) / 2f;
        float minHeight = midH - MidOffset;
        mid = new Vector3(mid.x, minHeight, mid.z);

        return mid;
    }

    private void LateUpdate()
    {
        if (Point1.isActiveAndEnabled && Point2.isActiveAndEnabled)
        {
            LineRenderer.enabled = true;
            Vector3 p1 = Point1.LineConnector.position;
            Vector3 p2 = Point2.LineConnector.position;
            float length = Vector3.Distance(Point1.LineConnector.position, Point2.LineConnector.position);

            Vector3 mid = GetMid(p1, p2);

            List<Vector3> points = new List<Vector3>();
            if (SegmentLength == 0)
            {
                SegmentLength = 1f;
            }
            for (float d = 0; d < length; d += SegmentLength)
            {
                float t = d / length;
                points.Add(Vector3.Lerp(Vector3.Lerp(p1, mid, t), Vector3.Lerp(mid, p2, t), t));

            }
            points.Add(p2);
            LineRenderer.positionCount = points.Count;
            LineRenderer.SetPositions(points.ToArray());
        }
        else
        {
            LineRenderer.positionCount = 0;
            LineRenderer.enabled = false;
        }
    }

    private float CalculateAccurateLength()
    {
        Vector3 p1 = Point1.LineConnector.position;
        Vector3 p2 = Point2.LineConnector.position;
        float length = Vector3.Distance(Point1.LineConnector.position, Point2.LineConnector.position);

        Vector3 mid = GetMid(p1, p2);

        if (SegmentLength == 0)
        {
            SegmentLength = 1f;
        }

        Vector3 last = p1;
        float accurateLength = 0;
        for (float d = 0; d < length; d += SegmentLength)
        {
            float t = d / length;
            Vector3 p = Vector3.Lerp(Vector3.Lerp(p1, mid, t), Vector3.Lerp(mid, p2, t), t);
            accurateLength += Vector3.Distance(last, p);
            last = p;

        }
        accurateLength += Vector3.Distance(last, p2);

        return accurateLength;
    }

    public void StartZip(ZiplinePole ziplinePole, Player player)
    {
        StartImpact.GenerateImpulse();
        if (ziplinePole == Point1)
        {
            player.Zip(Point1, Point2, this);
        }
        else if (ziplinePole == Point2)
        {
            player.Zip(Point2, Point1, this);
        }
    }

    public override void SetInteractable()
    {
        Point1.ActivateUse();
        Point2.ActivateUse();
    }

    public override void SetPassive()
    {
        Point1.DisableUse();
        Point2.DisableUse();
    }

    protected override void TakeDamage(float damage, Vector3 damageSourcePosition)
    {
        //todo?
    }

    public Vector3 GetPosition(ZiplinePole startZip, float zipPosition)
    {
        if (SegmentLength == 0)
        {
            SegmentLength = 1f;
        }

        Vector3 p1 = Point1.LineConnector.position;
        Vector3 p2 = Point2.LineConnector.position;
        Vector3 mid = GetMid(p1, p2);

        if (startZip == Point2)
        {
            (p1, p2) = (p2, p1);
        }

        float t = zipPosition / Length;
        return Vector3.Lerp(Vector3.Lerp(p1, mid, t), Vector3.Lerp(mid, p2, t), t);

    }

    public Vector3 GetTangent(ZiplinePole startZip, float zipPosition)
    {
        if (SegmentLength == 0)
        {
            SegmentLength = 1f;
        }

        Vector3 p1 = Point1.LineConnector.position;
        Vector3 p2 = Point2.LineConnector.position;
        Vector3 mid = GetMid(p1, p2);

        if (startZip == Point2)
        {
            (p1, p2) = (p2, p1);
        }

        float t = zipPosition / Length;

        Vector3 s1 = Vector3.Lerp(Vector3.Lerp(p1, mid, t), Vector3.Lerp(mid, p2, t), t);
        Vector3 s2 = Vector3.zero;
        float epsilon = 0.05f;
        if (t >= 1 - epsilon)
        {
            float t0 = t - epsilon;
            s2 = Vector3.Lerp(Vector3.Lerp(p1, mid, t0), Vector3.Lerp(mid, p2, t0), t0);

            return (s1 - s2).normalized;
        }
        else
        {
            float t1 = t - epsilon;
            s2 = Vector3.Lerp(Vector3.Lerp(p1, mid, t1), Vector3.Lerp(mid, p2, t1), t1);
            return (s2 - s1).normalized;
        }
    }
}
