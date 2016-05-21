using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class WandController : MonoBehaviour
{
    public GameObject Sphere;

    private EVRButtonId _gripButton = EVRButtonId.k_EButton_Grip;
    private EVRButtonId _triggerButton = EVRButtonId.k_EButton_SteamVR_Trigger;
    private EVRButtonId _padPutton = EVRButtonId.k_EButton_SteamVR_Touchpad;

    private SteamVR_TrackedObject _trackedObj;

    private SteamVR_Controller.Device Controller
    {
        get
        {
            return SteamVR_Controller.Input((int)_trackedObj.index);
        }
    }

    private HashSet<InteractableItem> _objectsHoveringOver = new HashSet<InteractableItem>();
    private InteractableItem _closestItem;
    private InteractableItem _interactingItem;

    public bool _isSolid = true;
    public bool _isDeleting = false;
    public Color BaseColor;
    public Color ActiveColor;
    public Color DeleteColor;

    // Use this for initialization
    private void Start()
    {
        _trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        if (Controller.GetPress(_padPutton))
        {
            foreach (var obj in _objectsHoveringOver)
                Object.Destroy(obj.gameObject);

            _objectsHoveringOver.Clear();
            _isDeleting = true;
        } else
        {
            _isDeleting = false;
        }

        if (Controller.GetPressDown(_triggerButton))
        {
            var minDist = float.MaxValue;
            var dist = 0.0f;
            _closestItem = null;

            foreach (var item in _objectsHoveringOver)
            {
                dist = (item.transform.position - transform.position).sqrMagnitude;

                if (dist < minDist)
                {
                    minDist = dist;
                    _closestItem = item;
                }
            }

            _interactingItem = _closestItem;
            if (_interactingItem != null)
            {
                if (_interactingItem.IsInteracting())
                    _interactingItem.EndInteraction(this);

                _interactingItem.BeginInteraction(this);
            }
        }

        if (Controller.GetPressUp(_triggerButton))
        {
            if (_interactingItem != null)
            {
                _interactingItem.EndInteraction(this);
                _interactingItem = null;
            }
        }

        if (Controller.GetPressUp(_gripButton))
        {
            _isSolid = !_isSolid;

            Sphere.GetComponent<Collider>().enabled = _isSolid;
        }



        Sphere.GetComponent<Renderer>().material.color = GetCurrentColor();
    }

    private Color GetCurrentColor()
    {
        if (_isDeleting)
            return DeleteColor;

        Color color;
        if (_interactingItem != null)
            color = ActiveColor;
        else
            color = BaseColor;
        color.a = _isSolid ? 1.0f : 0.5f;

        return color;
    }

    private void OnTriggerEnter(Collider collider)
    {
        var collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem != null)
            _objectsHoveringOver.Add(collidedItem);
    }

    private void OnTriggerExit(Collider collider)
    {
        var collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem != null)
            _objectsHoveringOver.Remove(collidedItem);
    }
}