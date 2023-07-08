using UnityEngine;
using Unity.MLAgents;

namespace Unity.MLAgentsExamples
{
    /// <summary>
    /// This class contains logic for locomotion agents with joints which might make contact with the ground.
    /// By attaching this as a component to those joints, their contact with the ground can be used as either
    /// an observation for that agent, and/or a means of punishing the agent for making undesirable contact.
    /// </summary>
    /// 


    public interface IReward
    {
        public void reward();
    }
    [DisallowMultipleComponent]
    public class GroundContact : MonoBehaviour
    {
        public Agent agent;
        public IReward sa;
        [Header("Ground Check")] public bool agentDoneOnGroundContact; // Whether to reset agent on ground contact.
        public bool penalizeGroundContact; // Whether to penalize on contact.
        public float groundContactPenalty; // Penalty amount (ex: -1).
        public bool touchingGround;
        public const string k_Ground = "ground"; // Tag of ground object.
        public bool quadraticFallOf;
        public bool detached;
        public bool triggersExorcist;
        /// <summary>
        /// Check for collision with ground, and optionally penalize agent.
        /// </summary>
        void OnCollisionEnter(Collision col)
        {
            if (col.transform.CompareTag(k_Ground) && !detached)
            {
                touchingGround = true;
                if (triggersExorcist)
                {
                    agent
                }

                if (penalizeGroundContact)
                {
                    if (quadraticFallOf)
                    {
                        agent.SetReward( (1 - (Mathf.Pow((agent.StepCount/(1.0f*agent.MaxStep)),2))) * groundContactPenalty);
                    }
                    else
                    {
                        agent.SetReward(groundContactPenalty);
                    }
                    
                }

                if (agentDoneOnGroundContact )
                {
                    sa = (IReward) agent;
                    state = ((IState) agent).GetState();
                    if(state == EnemyState.training)
                    {
                        sa.reward();
                        agent.EndEpisode();
                    }
         
                }
            }
        }
        EnemyState state;
        /// <summary>
        /// Check for end of ground collision and reset flag appropriately.
        /// </summary>
        void OnCollisionExit(Collision other)
        {
            if (other.transform.CompareTag(k_Ground) && !detached)
            {
                touchingGround = false;
            }
        }
        
        public bool stdAgentDoneOnGroundContact;
        public void Awake()
        {
            agent = GetComponentInParent<Agent>();
            stdAgentDoneOnGroundContact = agentDoneOnGroundContact;
        }
        public void reset()
        {
            agentDoneOnGroundContact = stdAgentDoneOnGroundContact;
            touchingGround = false;
            detached = false;
        }
    }


}
