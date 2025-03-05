using System.Collections.Generic;
using UnityEngine;

namespace AGDDPlatformer
{
    public class MovingPlatform : KinematicObject
    {
        [SerializeField] private float Speed;
        [SerializeField] private float tolerance = 0.1f;

        [SerializeField] private Transform currentTarget;
        [SerializeField] private GameObject TargetsContainer;
        [SerializeField] private List<Transform> targets = new List<Transform>();

        private void Awake()
        {
            PopulateTargets();
            gravityModifier = 0;
            currentTarget = targets[0];
        }

        void Update()
        {
            if (Vector2.Distance(transform.position, currentTarget.position) < tolerance)
            {
                IterateToNextTarget();
            }

            setVelocity();
        }

        void setVelocity()
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;
            velocity = direction * Speed;
        }

        void IterateToNextTarget()
        {
            int index = targets.IndexOf(currentTarget);
            if (index >= targets.Count - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            currentTarget = targets[index];
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AttatchTo(this);
            }
        }

        void OnCollisionExit2D(Collision2D other)
        {

            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Detatch();
            }
        }

        private void PopulateTargets()
        {
            targets.Clear();

            if (TargetsContainer == null)
            {
                Debug.LogError($"{name}: TargetsContainer is NOT assigned!");
                return;
            }

            Transform startPoint = null;
            Transform endPoint = null;
            List<Transform> middlePoints = new List<Transform>();

            foreach (Transform child in TargetsContainer.transform)
            {
                if (child.name == "StartPoint")
                {
                    startPoint = child;
                }
                else if (child.name == "EndPoint")
                {
                    endPoint = child;
                }
                else
                {
                    middlePoints.Add(child);
                }
            }

            middlePoints.Sort((a, b) => string.Compare(a.name, b.name));

            if (startPoint == null)
            {
                Debug.LogError($"{name}: StartPoint not found in TargetsContainer!");
                return;
            }

            if (endPoint == null)
            {
                Debug.LogError($"{name}: EndPoint not found in TargetsContainer!");
                return;
            }

            targets.Add(startPoint);
            targets.AddRange(middlePoints);
            targets.Add(endPoint);

            currentTarget = targets[0];
        }
        public void ResetPlatform()
        {
            currentTarget = targets[0];
            transform.position = targets[0].position;
            velocity = Vector2.zero;
        }

    }
}
