using Game.GlobalVariables;
using PathCreation;
using System.Collections;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Others
{
    public class ProgressBarControllerWithPath : ProgressBar
    {
        [Header("General Variables")]
        float firstDistanceValue;
        float currentDistanceValue;
        float distanceTravelled;
        float maxDistanceTravelled;

        [Header("References")]
        [SerializeField] Slider mainSlider;
        PathCreator pathCreatorRef;

        protected override void MB_Listen(bool status)
        {
            base.MB_Listen(status);

            if (status)
            {
                Manager.Subscribe(CustomManagerEvents.SendPathCreatorRef, FetchPathCreatorRef);
                Manager.Subscribe(CustomManagerEvents.UpdatePathedProgressBar, UpdateProgressBar);
            }
            else
            {
                Manager.Unsubscribe(CustomManagerEvents.SendPathCreatorRef, FetchPathCreatorRef);
                Manager.Unsubscribe(CustomManagerEvents.UpdatePathedProgressBar, UpdateProgressBar);
            }

        } // MB_Listen()

        void FetchPathCreatorRef(object[] a)
        {
            pathCreatorRef = (PathCreator)a[0];
            distanceTravelled = (float)a[1];
            maxDistanceTravelled = (float)a[2];

            mainSlider.value = 0f;
            firstDistanceValue = maxDistanceTravelled - distanceTravelled;

            if (pathCreatorRef == null)
            {
                Debug.Log("! Couldnt Fetch Path Creator !");
            }

        } // FetchPathCreatorRef()

        void UpdateProgressBar(object[] a)
        {
            float currDistTravelled = (float)a[0];

            currentDistanceValue = maxDistanceTravelled - currDistTravelled;
            mainSlider.value = (firstDistanceValue - currentDistanceValue) / firstDistanceValue;

        } // UpdatePlayerProgressBar()

    } // class
} // namespace

