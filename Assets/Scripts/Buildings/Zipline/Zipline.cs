using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
public class Zipline : MonoBehaviour
{
    public int PointsPlaced { get; private set; } = 0;
    public ZiplinePole Point1;
    public ZiplinePole Point2;
    public float SegmentLength;
    public float MidOffset = 0.1f;

    private LineRenderer _lineRenderer;

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

    private void Awake()
    {
        if (Application.isPlaying)
        {
            Point1.gameObject.SetActive(false);
            Point2.gameObject.SetActive(false);
        }
    }

    public void PlacePoint(Vector3 point)
    {
        switch (PointsPlaced)
        {
            case 0:
                Debug.Log("Point 1 Placed");
                Point1.gameObject.SetActive(true); 
                Point1.transform.position = point;
                break;
            case 1:
                Debug.Log("Point 2 Placed");
                Point2.gameObject.SetActive(true);
                Point2.transform.position = point;
                break;
            default:
                Debug.LogError("Placing point on already set up zipline!");
                break;
        }
        PointsPlaced++;
    }

    private void LateUpdate()
    {
        if (Point1.isActiveAndEnabled && Point2.isActiveAndEnabled)
        {
            LineRenderer.enabled = true;
            Vector3 p1 = Point1.LineConnector.position;
            Vector3 p2 = Point2.LineConnector.position;
            float length = Vector3.Distance(Point1.LineConnector.position, Point2.LineConnector.position);
            
            Vector3 mid = (p1 + p2) / 2f;

            float lowest = Mathf.Min(Point1.LineConnector.position.y, Point2.LineConnector.position.y);
            float minHeight = lowest - length / 4f;
            mid = new Vector3(mid.x,minHeight, mid.z);
            
            List<Vector3> points = new List<Vector3>();
            if(SegmentLength == 0)
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
}
