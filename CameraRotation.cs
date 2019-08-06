using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {

    public Camera mainCam;
    public Camera cam;
    public Transform target;
    public Transform origin;
    

    bool originMove = false;
    public float rotSpeed = 0.5f;

    [Range(1.0f, 65f)]
    public float cameraDistance;

    //There are two options. Our current prototype uses Ortho, but if we need perspective, it's here too
    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.

    public bool pause;

    public int switcharoo;

    void Start(){
        cam.transform.position = (cam.transform.position - target.position).normalized * cameraDistance + target.position;
        cam.enabled = false;
        mainCam.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate(){
        Debug.Log(originMove);
        Debug.Log(switcharoo);
        switch (switcharoo) {
            case 2:
                GestureMovement();
                break;
            case 1:
                ZoomOut();
                break;
            default:
                break;
        }

        if (!pause && originMove)
        {
            switcharoo = 1;
        }

        if (!pause && !originMove){

            switcharoo = 2;
        }

        if (pause){
            originMove = false;

            //This was part of a larger project, for the purpose of this script, I am leaving the next two lines in
            // So you can see what the pause button does

            //GameObject.Find("Canvas").GetComponentInChildren<JoystickControl>().enabled = true;
            //GameObject.Find("player").GetComponent<SkyWalker>().enabled = true;
        }

    }

    void GestureMovement() {

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            cam.transform.Translate(-touchDeltaPosition.x * rotSpeed, -touchDeltaPosition.y * rotSpeed, 0);

            // cam.transform.RotateAround(target.transform.position, target.transform.up, 20 * Time.deltaTime);            
            cam.transform.position = (cam.transform.position - target.position).normalized * cameraDistance + target.position;

        }


        if (Input.touchCount == 2)
        {

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            //calculating the distance between touches between each frame
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            //if this value is negative, then the fingers are moving apart and positive as they move together
            float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;

            if (cam.orthographic)
            {
                cam.orthographicSize += deltaMagnitudediff * orthoZoomSpeed;
                cam.orthographicSize = Mathf.Max(cam.orthographicSize, .5f);
            }
            else
            {
                cam.fieldOfView += deltaMagnitudediff * perspectiveZoomSpeed;
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 0.1f, 179.9f);
            }

        }

        cam.transform.LookAt(target);

    }

    //This is not neccessary, but for added looks when zooming away from a player, object, or target into the rotation mode
    void ZoomOut() {

        if (cam.enabled == true){
            cam.transform.position = Vector3.Lerp(cam.transform.position, origin.position, Time.deltaTime * 3f);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 5, Time.deltaTime * 3f);
            cam.transform.LookAt(target);

            StartCoroutine("runMoveOnce");

        }
    }

    //What happens when you click the Pause button
    public void Paused()
    {
        pause = !pause;
        mainCam.enabled = !mainCam.enabled;
        cam.enabled = !cam.enabled;
        originMove = !originMove;

    }

    //Not neccessary. Used to place time in between transitions
    IEnumerator runMoveOnce()
    {
        yield return new WaitForSeconds(1.5f);
        originMove = false;
    }

}
