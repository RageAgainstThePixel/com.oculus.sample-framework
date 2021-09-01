using UnityEngine;

public class AugmentedObject : MonoBehaviour
{
    public OVRInput.Controller controllerHand = OVRInput.Controller.None;
    public Transform shadow;
    private bool groundShadow = false;

    private void Start()
    {
        if (GetComponent<GrabObject>())
        {
            GetComponent<GrabObject>().GrabbedObjectDelegate += Grab;
            GetComponent<GrabObject>().ReleasedObjectDelegate += Release;
        }
    }

    private void Update()
    {
        if (controllerHand != OVRInput.Controller.None)
        {
            if (OVRInput.GetUp(OVRInput.Button.One, controllerHand))
            {
                ToggleShadowType();
            }
        }

        if (shadow)
        {
            if (groundShadow)
            {
                shadow.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            else
            {
                shadow.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void Grab(OVRInput.Controller grabHand)
    {
        controllerHand = grabHand;
    }

    public void Release()
    {
        controllerHand = OVRInput.Controller.None;
    }

    private void ToggleShadowType()
    {
        groundShadow = !groundShadow;
    }
}
