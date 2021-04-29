using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour
{

    public GameObject placementIndicator, objectToPlace;
    
    private ARSessionOrigin _arSessionOrigin;
    private Pose _pose;

    private bool placementPoseIsValid = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _arSessionOrigin = FindObjectOfType<ARSessionOrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, _pose.position, _pose.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(_pose.position, _pose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hitsList = new List<ARRaycastHit>();
        var rayCastMgr = FindObjectOfType<ARRaycastManager>();
        rayCastMgr.Raycast(screenCenter, hitsList, TrackableType.Planes);

        placementPoseIsValid = hitsList.Count > 0;
        if (placementPoseIsValid)
        {
            _pose = hitsList[0].pose;

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z);
            _pose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
