using Game.GlobalVariables;
using System.Collections;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;

namespace PathCreation.Examples
{
    public class PathFollowerController : Actor<LevelManager>
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float pathFollowSpeed = 5f;
        [HideInInspector] public float distanceTravelled;

        [Header("Custom Variables")]
        bool canFollowPath = false;
        float maxDistanceTravelled;
        bool isReachedFinish = false;

        protected override void MB_Listen(bool status)
        {
            base.MB_Listen(status);

            if (status)
            {
                Manager.Subscribe(CustomManagerEvents.PlayerCanFollowPath, SetFollowablePath);
            }
            else
            {
                Manager.Unsubscribe(CustomManagerEvents.PlayerCanFollowPath, SetFollowablePath);
            }

        } // MB_Listen()

        protected override void MB_Start()
        {
            //base.MB_Start();

            if (pathCreator != null)
            {
                pathCreator.pathUpdated += OnPathChanged;
            }

            distanceTravelled = 10f;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

            maxDistanceTravelled = pathCreator.path.length;

            Push(CustomManagerEvents.SendPathCreatorRef, pathCreator, distanceTravelled, maxDistanceTravelled);

        } // MB_Start()

        protected override void MB_Update()
        {
            base.MB_Update();

            if (pathCreator != null)
            {
                if (canFollowPath)
                {
                    distanceTravelled += pathFollowSpeed * Time.deltaTime;
                    transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                    transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

                    Push(CustomManagerEvents.UpdatePathedProgressBar, distanceTravelled);
                }

                if (distanceTravelled >= (maxDistanceTravelled - 3f) && !isReachedFinish)
                {
                    canFollowPath = false;
                    isReachedFinish = true;
                    Push(CustomManagerEvents.PlayerReachedEndOfPath);
                    //Bu eventý dinleyerek yolun sonuna ulaþtýðý durumu elde edebilirsin.
                }
            }

        } // MB_Update()

        void SetFollowablePath(object[] a)
        {
            canFollowPath = (bool) a[0];

        } // SetFollowablePath()

        void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);

        } // OnPathChanged()

    } // class
} // namespace