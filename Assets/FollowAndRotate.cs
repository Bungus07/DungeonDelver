using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowAndRotate : MonoBehaviour
{
    public GameObject Target;
    public Vector3 Offset;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Target.transform.position + Offset;
        if (Target.GetComponent<Person2>().IsRolling == false)
        {
            gameObject.transform.eulerAngles = new Vector3(0, Target.transform.eulerAngles.y, 0);
        }
    }
}
