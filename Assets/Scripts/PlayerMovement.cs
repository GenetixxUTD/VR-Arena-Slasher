using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public XRNode inputController;
    public XROrigin playerRig;
    private Vector2 InputAxis;
    private CharacterController character;

    public XRNode smoothTurnController;
    private Vector2 SmoothTurnAxis;

    public float gravity = -9.81f;
    private float downwardSpeed;
    public LayerMask groundLayer;

    public GameObject TeleportationHandManager;
    public SnapTurnProviderBase SnapTurnProvider;

    public enum movementType
    {
        continuous,
        teleport
    }

    public movementType selectedMovement;

    public enum rotationType
    {
        snap,
        smooth
    }

    public rotationType selectedRotation;

    // Start is called before the first frame update
    void Start()
    {
        
        character = GetComponent<CharacterController>();
        playerRig = GetComponent<XROrigin>();
        fetchSettings();
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputController);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out InputAxis);

        device = InputDevices.GetDeviceAtXRNode(smoothTurnController);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out SmoothTurnAxis);
    }

    private void FixedUpdate()
    {
        colliderHeadsetPos();

        if (selectedMovement == movementType.continuous)
        {
            Quaternion headDir = Quaternion.Euler(0, playerRig.Camera.transform.eulerAngles.y, 0);
            Vector3 movementDirection = headDir * new Vector3(InputAxis.x, 0, InputAxis.y);
            character.Move(movementDirection * Time.fixedDeltaTime * movementSpeed);
            TeleportationHandManager.SetActive(false);
        }
        else
        {
            TeleportationHandManager.SetActive(true);
        }

        if(selectedRotation == rotationType.snap)
        {
            SnapTurnProvider.enabled = true;
        }
        else
        {
            SnapTurnProvider.enabled = false;
            
            if(SmoothTurnAxis.x >= .8f)
            {
                this.transform.RotateAround(this.transform.position, this.transform.up, 20f * .1f);
            }
            else if(SmoothTurnAxis.x <= -.8f)
            {
                this.transform.RotateAround(this.transform.position, this.transform.up, 20f * -.1f);
            }
        }

        if (isGrounded())
        {
            downwardSpeed = 0;
        }
        else
        {
            downwardSpeed += gravity * Time.fixedDeltaTime;
        }
        
        character.Move(Vector3.up * downwardSpeed * Time.fixedDeltaTime);
    }

    private bool isGrounded()
    {
        Vector3 sphereStart = transform.TransformPoint(character.center);
        float sphereRayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(sphereStart, character.radius, Vector3.down, out RaycastHit hit, sphereRayLength, groundLayer);
        return hasHit;
    }

    private void colliderHeadsetPos()
    {
        character.height = playerRig.CameraInOriginSpaceHeight;
        Vector3 centerHeight = transform.InverseTransformPoint(playerRig.Camera.transform.position);
        character.center = new Vector3(centerHeight.x, character.height / 2, centerHeight.z);
    }

    public void fetchSettings()
    {
        SnapTurnProvider.turnAmount = PlayerPrefs.GetInt("snapangle");
        switch (PlayerPrefs.GetInt("movementtype"))
        {
            case 0:
                selectedMovement = movementType.continuous;
                break;
            case 1:
                selectedMovement = movementType.teleport;
                break;
        }

        switch(PlayerPrefs.GetInt("rotationtype"))
        {
            case 0:
                selectedRotation = rotationType.snap; break;
            case 1:
                selectedRotation = rotationType.smooth; break;
        }
    }
}
