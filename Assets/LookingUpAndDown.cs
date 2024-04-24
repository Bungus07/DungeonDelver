using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookingUpAndDown : MonoBehaviour
{
    public float Ysensitivity;
    public float MinYAngle;
    public float MaxYAngle;
    private float RotationY;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float MouseY = Input.GetAxis("Mouse Y");
        RotationY += MouseY * Ysensitivity;
        RotationY = Mathf.Clamp(RotationY, MinYAngle, MaxYAngle);
        transform.localEulerAngles = new Vector3(-RotationY, transform.localEulerAngles.y, 0);
    }
}
