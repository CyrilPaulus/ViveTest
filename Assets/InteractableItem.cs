using System.Collections;
using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    public Rigidbody _rigidBody;

    private bool _currentlyInteracting;
    private WandController _attachedWand;
    private Transform _interactionPoint;

    private float _velocityFactor = 20000f;
    private Vector3 _posDelta;

    private float _rotationalFactor = 400f;
    private Quaternion _rotationDelta;
    private float _angle;
    private Vector3 _axis;

    private Shader _originalShader;
    private Shader _wireframeShader;

    // Use this for initialization
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _interactionPoint = new GameObject().transform;
        _velocityFactor = _velocityFactor / _rigidBody.mass;
        _rotationalFactor = _rotationalFactor / _rigidBody.mass;


        _originalShader = GetComponent<Renderer>().material.shader;
        _wireframeShader = Shader.Find("Custom/Wireframe");
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (_attachedWand != null && _currentlyInteracting)
        {
            _posDelta = _attachedWand.transform.position - _interactionPoint.position;
            _rigidBody.velocity = _posDelta * _velocityFactor * Time.deltaTime;

            _rotationDelta = _attachedWand.transform.rotation * Quaternion.Inverse(_interactionPoint.rotation);
            _rotationDelta.ToAngleAxis(out _angle, out _axis);

            if (_angle > 180)
                _angle -= 360;

            _rigidBody.angularVelocity = (Time.deltaTime * _angle * _axis * _rotationalFactor);
        }
    }

    public void BeginInteraction(WandController wand)
    {
        _attachedWand = wand;
        _interactionPoint.position = wand.transform.position;
        _interactionPoint.rotation = wand.transform.rotation;
        _interactionPoint.SetParent(transform, true);

        _currentlyInteracting = true;
        GetComponent<Renderer>().material.shader = _wireframeShader;
        GetComponent<Renderer>().material.SetColor("_WireColor", new Color(0, 1, 0));
    }

    public void EndInteraction(WandController wand)
    {
        if (wand == _attachedWand)
        {
            _attachedWand = null;
            _currentlyInteracting = false;
            GetComponent<Renderer>().material.shader = _originalShader;
        }
    }

    public bool IsInteracting()
    {
        return _currentlyInteracting;
    }
}