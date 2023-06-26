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
    public float limbMaxHealth = 30;
    ConfigurableJoint joint;
    public List<IsBodyPart> children;
    public GroundContact groundContact;
    private void Start()
    {
        
           joint = GetComponent<ConfigurableJoint>();
        if (!joint && !isMain())
        {
            Debug.LogError("No Joint attached at gameobject of IsBodyPart");
        }
        groundContact = GetComponent<GroundContact>();
        ResetLimb();
        children = GetComponentsInChildren<IsBodyPart>().ToList();
        
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

    public void ResetLimb()
    {
        if (!isMain())
        {
            groundContact.detached = false;
            limbHealth = limbMaxHealth;
            slicedOff = false;
            detached = false;
            AttachJoint();
        }


    }
    public void SliceLimb()
    {
        if (!isMain())
        {
            slicedOff = true;
            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;
            DetechJointRecursive();
        }
    }
    void DetechJointRecursive()
    {
        
        DetechJoint();
        if(children != null)
        {
            foreach (var c in children)
            {
                c.DetechJointRecursive();
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
