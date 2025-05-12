using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -7);
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float pitchMin;
    [SerializeField] private float pitchMax;

    private float yaw = 0f;
    private float pitch = 15f;

    public List<Transform> cinematicTransform;
    private Transform currentCinematic;

    private void Awake()
    {
        Instance = this;
    }
    void LateUpdate()
    {
        if (GameManager.Instance.isInCinematic && currentCinematic != null)
        {
            transform.position = currentCinematic.position;
            transform.rotation = currentCinematic.rotation;
            return;
        }

        if (target == null) return;

        if (Cursor.lockState == CursorLockMode.Locked && !Cursor.visible && !GameManager.Instance.isOnTutorial)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            yaw += mouseX;
            pitch -= mouseY;
            if(!GameManager.Instance.Player.movement.isGrounded)
                pitchMax = 30;
            else
                pitchMax = 10;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);

    }
    public void EnterCinematic(int index)
    {
        GameManager.Instance.isInCinematic = true;
        currentCinematic = cinematicTransform[index];
    }

    public void ExitCinematic()
    {
        GameManager.Instance.isInCinematic = false;
        currentCinematic = null;
    }
}
