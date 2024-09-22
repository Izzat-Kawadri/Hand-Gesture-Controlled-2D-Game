using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// this comes from the Unity Standard Assets
//namespace UnityStandardAssets._2D
//{
public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
     
        Vector3 currentOffSet;
    Vector3 targetOffSet;
         Transform myTransform;
    public AnimationCurve reframeCurve;
    public int reFaremeTime;


        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

		// private variables
        float m_OffsetZ;
        Vector3 m_LastTargetPosition;
        Vector3 m_CurrentVelocity;
        Vector3 m_LookAheadPos;

        // Use this for initialization
        private void Start()
        {

      

            target = GameObject.FindGameObjectWithTag("Player").transform;
            myTransform = target.transform;

        m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;

			// if target not set, then set it to the player
			//if (target==null) {
            

       // }

			if (target==null)
				Debug.LogError("Target not set on Camera2DFollow.");
        myTransform = target.transform;
        targetOffSet = myTransform.position - target.position;

    }

        // Update is called once per frame
		private void Update()
        {
			if (target == null)
				return;

            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
				m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = newPos;

            m_LastTargetPosition = target.position;

        myTransform.position = target.position + currentOffSet;
//        }
    }
    public void setTraget(Transform taarget)
    {
        currentOffSet = myTransform.position - taarget.position;
        target = taarget;
        StopAllCoroutines();
        StartCoroutine(reframeCurantine());
    }
    IEnumerator reframeCurantine()
    {
        Vector3 frameOffeSet = currentOffSet;
        float elapsed = 0f;
        float progress = 0f;
        while (progress < 1f)
        {
            currentOffSet = Vector3.LerpUnclamped(frameOffeSet, targetOffSet, reframeCurve.Evaluate(progress));
            yield return null;
            elapsed += Time.deltaTime;
            progress = elapsed / reFaremeTime;
        }
        currentOffSet = targetOffSet;
        }
    }

