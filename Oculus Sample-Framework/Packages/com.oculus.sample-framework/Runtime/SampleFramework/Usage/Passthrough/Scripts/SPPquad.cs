using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPPquad : MonoBehaviour
{
    private OVRPassthroughLayer passthroughLayer;
    public MeshFilter projectionObject;
    private OVRInput.Controller controllerHand;

    private void Start()
    {
        passthroughLayer = GetComponent<OVRPassthroughLayer>();
        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, false);
        if (GetComponent<GrabObject>())
        {
            GetComponent<GrabObject>().GrabbedObjectDelegate += Grab;
            GetComponent<GrabObject>().ReleasedObjectDelegate += Release;
        }
    }

    public void Grab(OVRInput.Controller grabHand)
    {
        passthroughLayer.RemoveSurfaceGeometry(projectionObject.gameObject);
        controllerHand = grabHand;
    }

    public void Release()
    {
        controllerHand = OVRInput.Controller.None;
        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, false);
    }
}
