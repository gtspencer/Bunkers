using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainDeformer : VRInteractableItem
{
    private DynamicWorld world;
    public Material laserPointerMat;
    public Vector3 shootOrigin;
    public Vector3 shootDirection;
    public Vector3 laserPointerOrigin;
    public float lineWidth = 0.01f;

    //flags passed into DynamicWorld to communicate state of terrain deformer
    //flags are dynamically passed every fixed update and it's active state is checked EVERY UPDATE
    private bool flatFlag;
    private bool craterFlag;
    private bool addFlag;
    private bool subtractFlag;
    //location to initiate removal
    private Vector3 location;
    private LineRenderer laserPointerLine;

    //these change based on input to get checked every update for a state change
    private bool beginAdd;
    private bool beginSubtract;
    private bool beginFlat;
    private bool beginCrater;

    // Use this for initialization
    void Start()
    {
        world = GameObject.FindGameObjectWithTag("Ground").GetComponent<DynamicWorld>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //input to make ground flat
        if (beginAdd)
        {
            addFlag = true;
        }
        //input to make ground crater
        else if (beginSubtract)
        {
            subtractFlag = true;
        }
        //input to add mesh to ground
        else if (beginFlat)
        {
            flatFlag = true;
        }
        //input to remove mesh from ground
        else if (beginCrater)
        {
            craterFlag = true;
        }
        else
        {
            addFlag = false;
            subtractFlag = false;
            flatFlag = false;
            craterFlag = false;
        }
        world.toggleAdd(addFlag, location);
        world.toggleSubtract(subtractFlag, location);
        world.toggleFlat(flatFlag, location);
        world.toggleCrater(craterFlag, location);
    }

    override public void ActionPressed()
    {
        beginCrater = true;
    }
    override public void ActionRelease()
    {
        beginCrater = false;
    }

    override public void Action2Pressed()
    {
        beginSubtract = true;
    }
    override public void Action2Release()
    {
        beginSubtract = false;
    }

    override public void addPressed()
    {
        beginAdd = true;
    }
    override public void addReleased()
    {
        beginAdd = false;
    }

    override public void flattenPressed()
    {
        beginFlat = true;
    }
    override public void flattenReleased()
    {
        beginFlat = false;
    }

    override public bool Pickup(VRInteractor hand)
    {
        return base.Pickup(hand);
    }

    override public void Drop(SteamVR_TrackedObject trackedObj)
    {
        base.Drop(trackedObj);
    }

    override public void Drop()
    {
        base.Drop();
    }

    override public void EnableHover()
    {
        base.EnableHover();
    }

    override public void DisableHover()
    {
        base.DisableHover();
    }

    void laserPointer()
    {
        if (laserPointerMat != null)
        {
            Ray ray = new Ray(item.TransformPoint(shootOrigin), item.TransformDirection(shootDirection));
            RaycastHit hit;
            bool hitSomething = false;
            hitSomething = Physics.Raycast(ray, out hit, 100);

            if (laserPointerLine == null)
            {
                laserPointerLine = gameObject.AddComponent<LineRenderer>();
                laserPointerLine.material = laserPointerMat;
                laserPointerLine.SetWidth(lineWidth, lineWidth);
                laserPointerLine.SetVertexCount(2);
            }
            Vector3 endPoint = Vector3.zero;
            if (hitSomething)
            {
                endPoint = hit.point;
            }
            else
            {
                endPoint = ray.origin + (ray.direction * 100);
            }
            laserPointerLine.SetPosition(0, item.TransformPoint(laserPointerOrigin));
            laserPointerLine.SetPosition(1, endPoint);
            location = endPoint;
        }
    }
    public void toggleAdd(bool set)
    {
        if (set)
        {
            beginAdd = true;
        }
        else
        {
            beginAdd = false;
        }
    }

    public void toggleSubtract(bool set)
    {
        if (set)
        {
            beginSubtract = true;
        }
        else
        {
            beginSubtract = false;
        }
    }

    public void toggleCrater(bool set)
    {
        if (set)
        {
            beginCrater = true;
        }
        else
        {
            beginCrater = false;
        }
    }

    public void toggleFlat(bool set)
    {
        if (set)
        {
            beginFlat = true;
        }
        else
        {
            beginFlat = false;
        }
    }
}
