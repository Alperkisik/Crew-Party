using Game.GlobalVariables;
using System.Collections;
using System.Collections.Generic;
using TriflesGames.Actors;
using UnityEngine;

namespace Game.Actors
{
    public class GameLevelActor : LevelActor
    {
        // Custom fields

        [SerializeField] private Transform _PlayerTransform;
        [SerializeField] private Transform _cameraAimTarget;
        [SerializeField] private Transform _FinishlineTransform;
        [SerializeField] private PathCreation.Examples.RoadMeshCreator roadMeshCreatorRef;

        public override void InitLevel()
        {
            Push(CustomManagerEvents.SendPlayerTransform, _PlayerTransform);
            Push(CustomManagerEvents.SendFinishlineTransform, _FinishlineTransform);
            Push(CustomManagerEvents.SendCameraTarget, _PlayerTransform, _cameraAimTarget);

            if (roadMeshCreatorRef != null)
            {
                roadMeshCreatorRef.UpdateMeshManuel();
            }
        }
    }
}