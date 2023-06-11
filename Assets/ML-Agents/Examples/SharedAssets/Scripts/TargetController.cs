using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using UnityEngine.Events;


    /// <summary>
    /// Utility class to allow target placement and collision detection with an agent
    /// Add this script to the target you want the agent to touch.
    /// Callbacks will be triggered any time the target is touched with a collider tagged as 'tagToDetect'
    /// </summary>
    /// 
    public interface Iid
    {
        public int GetID();
    }
    public enum RespawnMode
    {
        Box2D,
        Radius,
        Grid

    }

    public class TargetController : MonoBehaviour
    {
    public GetRandomPositionOnSurface getRandom;
    //Target has to be under the map to work properly and on the same level as the Mlagents Agent
    public Transform rootMap;
        public RespawnMode respawnMode;
        public Iid a; 
        [Header("Collider Tag To Detect")]
        public string tagToDetect = "agent"; //collider tag to detect 

        [Header("Target Placement")]
        public float spawnRadius; //The radius in which a target can be randomly spawned.
        public bool respawnIfTouched; //Should the target respawn to a different position when touched

        [Header("Target Fell Protection")]
        public bool respawnIfFallsOffPlatform = true; //If the target falls off the platform, reset the position.
        public float fallDistance = 5; //distance below the starting height that will trigger a respawn 


        private Vector3 m_startingPos; //the starting position of the target
        private Agent m_agentTouching; //the agent currently touching the target

        [System.Serializable]
        public class TriggerEvent : UnityEvent<Collider>
        {
        }

        [Header("Trigger Callbacks")]
        public TriggerEvent onTriggerEnterEvent = new TriggerEvent();
        public TriggerEvent onTriggerStayEvent = new TriggerEvent();
        public TriggerEvent onTriggerExitEvent = new TriggerEvent();

        [System.Serializable]
        public class CollisionEvent : UnityEvent<Collision>
        {
        }

        [Header("Collision Callbacks")]
        public CollisionEvent onCollisionEnterEvent = new CollisionEvent();
        public CollisionEvent onCollisionStayEvent = new CollisionEvent();
        public CollisionEvent onCollisionExitEvent = new CollisionEvent();

        
        public Mesh mesh;
        public void Awake()
        {
            //meshFilter = GetComponent<MeshFilter>();
            //mesh = meshFilter.mesh;
        }
        // Start is called before the first frame update
        void OnEnable()
        {
            m_startingPos = transform.localPosition;
            if (respawnIfTouched)
            {
                MoveTargetToRandomPosition();
            }
        }

        void Update()
        {
            if (respawnIfFallsOffPlatform)
            {
                if (transform.localPosition.y < m_startingPos.y - fallDistance)
                {
                    Debug.Log($"{transform.name} Fell Off Platform");
                    MoveTargetToRandomPosition();
                }
            }
        }
    
        /// <summary>
        /// Moves target to a random position within specified radius.
        /// </summary>
        public void MoveTargetToRandomPosition()
        {
            Vector3 newTargetPos;
            switch (respawnMode)
            {
                
                case (RespawnMode.Radius):

                    newTargetPos = m_startingPos + (Random.insideUnitSphere * spawnRadius);
                    newTargetPos.y = m_startingPos.y;
                    transform.localPosition = newTargetPos;
                    break;
                case (RespawnMode.Box2D):
                    Vector2 r = new Vector2(spawnRadius * (Random.value - 0.5f) * 2, spawnRadius * (Random.value - 0.5f) * 2);
                    newTargetPos = m_startingPos + new Vector3(r.x,0,r.y);
                    
                    transform.localPosition = newTargetPos;
                    break;

                case (RespawnMode.Grid):
                   
                    transform.position = getRandom.randomPosOnGrid(1);
                    break;


            }
        }
        private void OnCollisionEnter(Collision col)
        {
            if (col.transform.CompareTag(tagToDetect))
            {
                if(col.gameObject.GetComponent<IsBodyPart>().id == a.GetID())

                {
                    onCollisionEnterEvent.Invoke(col);
                    if (respawnIfTouched)
                    {
                        MoveTargetToRandomPosition();
                    }
                }
                
            }
        }

        private void OnCollisionStay(Collision col)
        {
            if (col.transform.CompareTag(tagToDetect))
            {
                if (col.gameObject.GetComponent<IsBodyPart>().id == a.GetID())
                {
                    onCollisionStayEvent.Invoke(col);
                }
            }
        }

        private void OnCollisionExit(Collision col)
        {
            if (col.transform.CompareTag(tagToDetect))
            {
                if (col.gameObject.GetComponent<IsBodyPart>().id == a.GetID())
                {
                    onCollisionExitEvent.Invoke(col);
                }
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag(tagToDetect))
            {
                if (col.gameObject.GetComponent<IsBodyPart>().id == a.GetID())
                {
                    onTriggerEnterEvent.Invoke(col);
                }
            }
        }

        private void OnTriggerStay(Collider col)
        {
            if (col.CompareTag(tagToDetect))
            {
                if (col.gameObject.GetComponent<IsBodyPart>().id == a.GetID())
                {
                    onTriggerStayEvent.Invoke(col);
                }
            }
        }

        private void OnTriggerExit(Collider col)
        {
            if (col.CompareTag(tagToDetect))
            {
                if (col.gameObject.GetComponent<IsBodyPart>().id == a.GetID())
                {
                    onTriggerExitEvent.Invoke(col);
                }
            }
        }
    }
