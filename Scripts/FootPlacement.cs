using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FootPlacement : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform RotatedParentObject;

    [SerializeField]
    private float rayDistance = 0.1f;
    [SerializeField]
    private float rayOffset = 0.3f;

    [SerializeField]
    private Transform RFootTarget;
    [SerializeField]
    private Transform LFootTarget;

    [SerializeField]
    private TwoBoneIKConstraint RFootRig;
    [SerializeField]
    private TwoBoneIKConstraint LFootRig;

    public LayerMask FootLayerMask;
    

    RaycastHit hit;
    Ray ray;

    /*private void OnAnimatorIK(int layerIndex)
    {
        RaycastHit hit;
        Ray ray;

        Quaternion RFootRot = animator.GetIKRotation(AvatarIKGoal.RightFoot);
        Debug.Log(RFootRot);

        ray = new Ray(RFootTarget.position + Vector3.up * rayOffset, Vector3.down);
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.DrawRay(ray.origin, ray.direction, Color.red);
            //RFootTarget.position = hit.point;
            //RFootTarget.up = hit.normal;
            //RFootTarget.rotation = RFootRot;
        }

        Quaternion LFootRot = animator.GetIKRotation(AvatarIKGoal.LeftFoot);

        ray = new Ray(LFootTarget.position + Vector3.up * rayOffset, Vector3.down);
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.DrawRay(ray.origin, ray.direction, Color.red);
            LFootTarget.position = hit.point;
            LFootTarget.up = hit.normal;
            LFootTarget.rotation = LFootRot;
        }
    }*/
    private void LateUpdate()
    {
        if (RotatedParentObject.GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {
            LFootRig.weight = Mathf.Lerp(LFootRig.weight, 0, Time.deltaTime * 7f);
            RFootRig.weight = Mathf.Lerp(RFootRig.weight, 0, Time.deltaTime * 7f);        }
        else
        {
            LFootRig.weight = Mathf.Lerp(LFootRig.weight, 1, Time.deltaTime * 7f);
            RFootRig.weight = Mathf.Lerp(RFootRig.weight, 1, Time.deltaTime * 7f);
        }

        ray = new Ray(RFootTarget.position + Vector3.up * rayOffset, Vector3.down);
        if (Physics.Raycast(ray, out hit, 5f, FootLayerMask))
        {
            //Debug.DrawRay(ray.origin, ray.direction, Color.red);
            RFootTarget.position = hit.point;
            Vector3 newUp = hit.normal;
            Vector3 oldForward = RotatedParentObject.forward;

            Vector3 newRight = Vector3.Cross(newUp, oldForward);
            Vector3 newForward = Vector3.Cross(newRight, newUp);
            RFootTarget.rotation = Quaternion.LookRotation(newForward, newUp);   
        }

        ray = new Ray(LFootTarget.position + Vector3.up * rayOffset, Vector3.down);
        if (Physics.Raycast(ray, out hit, 5f, FootLayerMask))
        {
            //Debug.DrawRay(ray.origin, ray.direction, Color.red);
            LFootTarget.position = hit.point;
            Vector3 newUp = hit.normal;
            Vector3 oldForward = RotatedParentObject.forward;

            Vector3 newRight = Vector3.Cross(newUp, oldForward);
            Vector3 newForward = Vector3.Cross(newRight, newUp);
            LFootTarget.rotation = Quaternion.LookRotation(newForward, newUp);
        }
    }
}
