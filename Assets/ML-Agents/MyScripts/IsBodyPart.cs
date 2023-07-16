using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgentsExamples;
public class IsBodyPart : MonoBehaviour, ITakeDamage, ICanDie, Iid
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
    public bool isTraining;
    void addVisuals()
    {
        if (skinned)
        {

        }
    }
    Collider c;
    IState state;
    private void Awake()
    {
        c = GetComponentInChildren<Collider>();
        partController = GetComponentInParent<BodyPartController>();
        state = GetComponentInParent<IState>();
           joint = GetComponent<ConfigurableJoint>();
        if (!joint && !isMain())
        {
            Debug.LogError("No Joint attached at gameobject of IsBodyPart");
        }
        groundContact = GetComponent<GroundContact>();
       
        children =transform.GetComponentsInChildren<IsBodyPart>().ToList();
        children.RemoveAt(0);
        if (joint)
        {
            connected = joint.connectedBody;
            jj = new JJ(joint.angularXMotion, joint.angularYMotion, joint.angularZMotion);
        }
       
    }
    CrossHairManager crossHairManager;
    private void Start()
    {
        if (state.GetState() == EnemyState.playing)
            crossHairManager = FindAnyObjectByType<CrossHairManager>();
        if (state.GetState() == EnemyState.training)
            isTraining = true;
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
            //activate to allow for tocuh floor 
            //partController.CanTouchFloor(true);
            
            partController.SwitchModelToLimb();
            var g = GetComponentsInParent<IsBodyPart>();
            if (g.Length > 1 && g[1])
                g[1].SwitchVisuals(true);
             slicedOff = true;
            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;

            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Free;

            joint.connectedBody = partController.setToRB;
            DetachJointRecursive();
            SwitchVisuals(true);
            //SetParent(partController.root.parent);
            SetParent(null);
            if (state.GetState() == EnemyState.playing)
            {
                crossHairManager.GetCrossHairByName("bone").startAnimation();
                crossHairManager.PlaySound("bone");
            }
                
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
    struct JJ
    {
        public ConfigurableJointMotion x;
        public ConfigurableJointMotion y;
        public ConfigurableJointMotion z;
        public JJ(ConfigurableJointMotion a, ConfigurableJointMotion b, ConfigurableJointMotion c)
        {
            x = a;
            y = b;
            z = c;
        }
    }
    JJ jj;
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

        joint.angularXMotion = jj.x;
        joint.angularYMotion = jj.y;
        joint.angularZMotion = jj.z;
    }

    public bool Die()
    {
       SliceLimb();
        return true;
    }
  
    private void OnCollisionStay(Collision collision)
    {
        if (false)
        {
            var s = collision.transform.GetComponentInParent<SpiderAgent>();
            Iid i = null;
            if (s)
            {
                i= (Iid)s;
            }

              
            if (i != null)
            {
                int o = GetID();
                int oo = i.GetID();
                if (o != oo)
                {
                    Collider col = collision.gameObject.GetComponentInChildren<Collider>();
                    if (col)
                        Physics.IgnoreCollision(c, col, true);
                    return;
                }
            }
            i = null;
            var t = collision.transform.GetComponent<TargetController>();
            if (t)
                i = (Iid)t;
            if (i != null)
            {
                if (i.GetID() != GetID())
                {
                    Collider col = collision.gameObject.GetComponent<Collider>();
                    if (col)
                        Physics.IgnoreCollision(c, col, true);
                }
            }
        }

    }

    public int GetID()
    {
        return id;
    }
}
