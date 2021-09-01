using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassthroughBrush : MonoBehaviour
{
    public OVRInput.Controller controllerHand = OVRInput.Controller.None;
    public GameObject lineSegmentPrefab;
    public GameObject lineContainer;
    public bool forceActive = true;
    private LineRenderer currentLineSegment = null;
    private List<Vector3> inkPositions = new List<Vector3>();
    private float minInkDist = 0.01f;
    private float strokeWidth = 0.1f;
    private float strokeLength = 0.0f;

    public enum BrushState
    {
        Idle,
        Inking
    };

    private BrushState brushStatus = BrushState.Idle;

    private void OnDisable()
    {
        brushStatus = BrushState.Idle;
    }

    private void LateUpdate()
    {
        // face camera
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        if (!(controllerHand == OVRInput.Controller.LTouch || controllerHand == OVRInput.Controller.RTouch))
        {
            return;
        }

        Vector3 tipPosition = transform.position;
        switch (brushStatus)
        {
            case BrushState.Idle:
                if (OVRInput.GetUp(OVRInput.Button.One, controllerHand))
                {
                    UndoInkLine();
                }
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerHand))
                {
                    StartLine(tipPosition);
                    brushStatus = BrushState.Inking;
                }
                break;
            case BrushState.Inking:
                // every frame, draw line
                UpdateLine(tipPosition);
                if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controllerHand))
                {
                    brushStatus = BrushState.Idle;
                }
                break;
        }
    }

    private void StartLine(Vector3 inkPos)
    {
        GameObject newLine = Instantiate(lineSegmentPrefab, inkPos, Quaternion.identity);
        currentLineSegment = newLine.GetComponent<LineRenderer>();
        currentLineSegment.positionCount = 1;
        currentLineSegment.SetPosition(0, inkPos);
        strokeWidth = currentLineSegment.startWidth;
        strokeLength = 0.0f;
        inkPositions.Clear();
        inkPositions.Add(inkPos);
        newLine.transform.parent = lineContainer.transform;
    }

    private void UpdateLine(Vector3 inkPos)
    {
        float segmentLength = (inkPos - inkPositions[inkPositions.Count - 1]).magnitude;
        if (segmentLength >= minInkDist)
        {
            inkPositions.Add(inkPos);
            currentLineSegment.positionCount = inkPositions.Count;
            currentLineSegment.SetPositions(inkPositions.ToArray());
            strokeLength += segmentLength;
            // passing the line length to the shader ensures that the tail/end fades are consistent width
            currentLineSegment.material.SetFloat("_LineLength", strokeLength / strokeWidth);
        }
    }

    public void ClearLines()
    {
        for (int i = 0; i < lineContainer.transform.childCount; i++)
        {
            Destroy(lineContainer.transform.GetChild(i).gameObject);
        }
    }

    public void UndoInkLine()
    {
        if (lineContainer.transform.childCount >= 1)
        {
            Destroy(lineContainer.transform.GetChild(lineContainer.transform.childCount - 1).gameObject);
        }
    }
}
