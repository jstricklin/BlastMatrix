using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace SA
{
    public abstract class StateManager : MonoBehaviour
    {
        // public float health;
        public State currentState;
        public Transform currentTarget;
        public GameObject avoidTarget;
        public GameObject moveTarget;
        public GameObject deathFX;
        public GameObject _deathFX;
        // public UnitType unitType;
        public Transform lookTarget;
        public Transform lastTarget;
        public Transform attacker;
        public Transform homeBase;
        // public IResourceReceiver homeReceiver => homeBase.GetComponent<IResourceReceiver>();
        public int maxHealth = 100;
        public int currentHealth { get; set; }

        // public AIBehaviorMode behaviorMode;

        public enum BattleMode { Chasing, Attacking, Fleeing, Idle, Patrolling, FollowingOrders, ReturningToBase }
        public BattleMode battleMode = BattleMode.Idle;
        public BattleMode prevMode;

        // public float patrolTime = 10f;

        public bool canAvoid { get; set; }

        
        bool onPlanet = false;
        bool approachingPlanet = false;
        bool nearPlanet = false;

        // from test enemy script
        public float alarmResponseDist = 3000f;
        public float attackRange = 30f;
        public float _maxDist;
        public float repairDist = 15f;
        public float maxDist
        {
            get
            {
                switch (battleMode)
                {
                    case BattleMode.Chasing:
                        return attackRange;
                    case BattleMode.Fleeing:
                        return repairDist;
                    default:
                        return _maxDist;
                }
            }
        }
        public float lostEnemyDist => attackRange * attackRange * 2;
        public List<Transform> enemyTargets = new List<Transform>();
        public float currentDist;
        public Vector3 dir;
        // [HideInInspector]
        // public float delta;
        public Rigidbody myRb;
        public float dot;
        public float aimDot;
        public bool canMove { get; set; }
        public bool grounded { get; set; }
        [HideInInspector]
        public Transform mTransform;
        [HideInInspector]
        public BaseBot baseBot;
        // [HideInInspector]
        // public ResourceGatherer resourceGatherer;

         public void Start()
         {
            mTransform = this.transform;
            // resourceGatherer = mTransform.GetComponentInChildren<ResourceGatherer>();
            baseBot = mTransform.GetComponent<BaseBot>();
            if (baseBot == null) 
                baseBot = mTransform.GetComponentInChildren<BaseBot>();
            myRb = GetComponent<Rigidbody>();
         }
        public virtual void Update()
        {
            if (currentState != null)
            {
                currentState.Tick(this);
            }
        }
        public virtual void FixedUpdate()
        {
            if (currentState != null)
            {
                currentState.FixedTick(this);
            }
        }
    }
}
