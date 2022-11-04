using Game.GlobalVariables;
using System.Collections;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Others
{
    public class ProgressBarController : ProgressBar
    {
        /*
        Progress Bar'ı kullanabilmek için MB_Listen metodu içinde yer alan 2 event'ında LevelManager'dan üretilen bir actorden gönderilmesi gerekiyor.
        Bu eventların gönderilmesini "GameLevelActor" üzerinden yapabilirsin(örnek).
        "SendFinishlineTransform" eventına parametre olarak bölüm sonunun konumunu simgeleyen bir transform göndermen gerekiyor.
        "SendPlayerTransform" eventına parametre olarak player transformunu göndermen gerekiyor.
        */

        [Header("General Variables")]
        float firstDistanceValue; 
        float currentDistanceValue;
        float playerZ = 0f;

        [Header("References")]
        [SerializeField] Slider mainSlider;
        Transform playerTransform;
        Transform finishPointTransform;

        protected override void MB_Listen(bool status)
        {
            base.MB_Listen(status);

            if (status)
            {
                Manager.Subscribe(CustomManagerEvents.SendFinishlineTransform, FetchFinishTransform, Priority.High);
                Manager.Subscribe(CustomManagerEvents.SendPlayerTransform, FetchPlayerTransform);
            }
            else
            {
                Manager.Unsubscribe(CustomManagerEvents.SendFinishlineTransform, FetchFinishTransform, Priority.High);
                Manager.Unsubscribe(CustomManagerEvents.SendPlayerTransform, FetchPlayerTransform);
            }

        } // MB_Listen()

        protected override void MB_Update()
        {
            base.MB_Update();

            if (playerTransform != null)
            {
                UpdateProgressBar();
            }

        } // MB_Update()

        void FetchPlayerTransform(object[] a)
        {
            StartCoroutine(DelayedAttach((Transform)a[0]));

        } // InitBarValues()
        IEnumerator DelayedAttach(Transform _playerTransform)
        {
            yield return new WaitForEndOfFrame();

            if (finishPointTransform != null)
            {
                mainSlider.value = 0f;
                playerTransform = _playerTransform;
                playerZ = _playerTransform.position.z;

                firstDistanceValue = Mathf.Abs(finishPointTransform.position.z) - Mathf.Abs(playerZ);
            }
            else
            {
                Debug.Log("! Finish Transform Ref is Null !");
            }

        } // DelayedAttach()

        void FetchFinishTransform(object[] a)
        {
            finishPointTransform = (Transform)a[0];

            if (finishPointTransform == null)
            {
                Debug.Log("! Couldnt Fetch Finish Transform !");
            }

        } // AttachFinishlineTransform()

        void UpdateProgressBar()
        {
            if (finishPointTransform != null)
            {
                playerZ = playerTransform.position.z;
                currentDistanceValue = Mathf.Abs(finishPointTransform.position.z) - Mathf.Abs((float)playerZ);
                mainSlider.value = (firstDistanceValue - currentDistanceValue) / firstDistanceValue;
            }

        } // UpdatePlayerProgressBar()

    } // class
} // namespace

