using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgentsExamples;
public class IsBodyPart : MonoBehaviour, ITakeDamage, ICanDie
{
    // Start is called before the first frame update
    public float strength = 40;
    public bool x = true, y = true, z = true;
    public int id;

    public bool slicedOff = false;
    public bool detached;
    public float limbHealth;
    public float limbMaxHealth = 15;
    ConfigurableJoint joint;
    public List<IsBodyPart> children;
    public GroundContact groundContact;
    public GameObject visual;
    public SkinnedMeshRenderer skinned;
    public BodyPartController partController;
    public bool sliceable=true;
    void addVisuals()
    {
        if (skinned)
        {

        }
    }
    private void Awake()
    {
        partController = GetComponentInParent<BodyPartController>();
           joint = GetComponent<ConfigurableJoint>();
        if (!joint && !isMain())
        {
            Debug.LogError("No Joint attached at gameobject of IsBodyPart");
        }
        groundContact = GetComponent<GroundContact>();
       
        children =transform.GetComponentsInChildren<IsBodyPart>().ToList();
        children.RemoveAt(0);
        if(joint)
        connected = joint.connectedBody;
       
    }
    public bool isMain()
    {
        if(!x && !y && !z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool TakeDamage(float d)
    {
        if (!detached)
        {
            limbHealth -= d;
            if (limbHealth <= 0)
            {
                Die();
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;

    }
    public void SetParent(Transform t)
    {
        transform.parent = t;
    }
    public void ResetLimb()
    {
        if(groundContact)
        groundContact.reset();
        if (sliceable)
        {
           
            limbHealth = limbMaxHealth;
            slicedOff = false;
            detached = false;
            AttachJoint();
            SwitchVisuals(false);
        }


    }
    public void SwitchVisuals(bool detached)
    {
        if (sliceable)
        {
            if (detached)
            {
                skinned.enabled = false;
                visual.SetActive(true);
            }
            else
            {
                skinned.enabled = true;
                visual.SetActive(false);
            }
        }
        
    }
    public Rigidbody connected;
    public void SliceLimb()
    {
        partController.SetLimbOff();
        if (sliceable)
        { 
            partController.CanTouchFloor(true);
            partController.SwitchModelToLimb();
            var g = GetComponentsInParent<IsBodyPart>();
            if (g.Length > 1 && g[1])
                g[1].SwitchVisuals(true);
             slicedOff = true;
            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;
            
            joint.connectedBody = partController.setToRB;
            DetachJointRecursive();
            SwitchVisuals(true);
            //SetParent(partController.root.parent);
            SetParent(null);
        }
    }
    void DetachJointRecursive()
    {
        
        DetechJoint();
        if(children != null)
        {
            foreach (var c in children)
            {
                c.DetachJointRecursive();
            }
        }

    }
    void DetechJoint()
    {
        groundContact.agentDoneOnGroundContact = false;
        detached = true;
        groundContact.detached = true;
    }
    void AttachJoint()
    {
        joint.connectedBody = connected;
        
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        
    }

    public bool Die()
    {
       SliceLimb();
        return true;
    }
}
