using UnityEngine;
using Vuforia;
using EasyUI.Popup;

public class ModelTargetInstantiator : DefaultObserverEventHandler
{
    /*
    public GameObject FloatingPopUp;
    public GameObject ARCamera;

    private Transform FloatingPopUpTransform;
    private GameObject ModelTargetParent;

    protected override void OnTrackingFound()
    {
        Popup.Show("Tracking Found" , "Tracking Found", "Close", PopupColor.Blue);

        FloatingPopUp.transform.SetParent(transform);
        FloatingPopUpTransform = FloatingPopUp.transform;
        ModelTargetParent = transform.parent.gameObject;

        base.OnTrackingFound();
    }

    protected override void OnTrackingLost()
    {
        Popup.Show("Tracking Lost" , "Tracking Lost", "Close", PopupColor.Blue);

        FloatingPopUp.transform.SetParent(ARCamera.transform);
        FloatingPopUp.transform.position = ARCamera.transform.position + 2 * ARCamera.transform.forward;
        FloatingPopUp.SetActive(true);

        base.OnTrackingLost();
    }
    */
}