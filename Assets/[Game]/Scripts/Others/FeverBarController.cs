using DG.Tweening;
using Game.GlobalVariables;
using Sirenix.OdinInspector;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;
using UnityEngine.UI;

public class FeverBarController : Actor<LevelManager>
{

    /*
    Fever Bar'ý kullanabilmek için MB_Listen metodu içinde yer alan 1 event'ýnda LevelManager'dan üretilen bir actorden gönderilmesi gerekiyor.       
    "CustomManagerEvents.FeverIncrease" eventi yollayarak barý doldurabilirsiniz.
    */


    [Title("Fever Setup")]
    [SerializeField] private float _FeverDuration;
    [SerializeField] private Image _FeverBar;
    [SerializeField] private GameObject _FeverParent;
    [SerializeField] private Image _FeverCanvasEffect;
    [SerializeField] private GameObject _FeverParticleEffect;
    [SerializeField] private Animation _FeverTextAnimation;
    [SerializeField]
    private float _FeverValue
    {
        get { return _feverValue; }
        set
        {
            _feverValue = Mathf.Clamp(value, 0, 100);
            _FeverBar.DOFillAmount(_feverValue * 0.01f, 1).OnComplete(() =>
            {
                if (_feverValue >= 100)
                {
                    StartFever();
                }
            });
        }
    }
    private float _feverValue;
    private bool isFeverActive;

    protected override void MB_Start()
    {
        _FeverParent.SetActive(true);
    }

    protected override void MB_Listen(bool status)
    {
        if (status)
        {
            LevelManager.Instance.Subscribe(CustomManagerEvents.FeverIncrease, FeverIncrease);
        }
        else
        {
            LevelManager.Instance.Unsubscribe(CustomManagerEvents.FeverIncrease, FeverIncrease);
        }
    }

    private void FeverIncrease(object[] args)
    {
        if (isFeverActive) return;

        _FeverValue += (float)args[0];
    }

    private void StartFever()
    {
        isFeverActive = true;
        Push(CustomManagerEvents.FeverState, true);

        _FeverTextAnimation.gameObject.SetActive(true);
        _FeverTextAnimation.Play();

        _FeverCanvasEffect.DOKill();
        _FeverCanvasEffect.DOFade(0.02f, 0);
        _FeverCanvasEffect.DOFade(0.04f, 0.5f).SetLoops(-1, LoopType.Yoyo);

        _FeverParticleEffect.SetActive(true);

        _FeverBar.DOFillAmount(0, _FeverDuration).SetEase(Ease.Unset).OnComplete(() =>
        {
            StopFever();
        });
    }

    private void StopFever()
    {
        _FeverCanvasEffect.DOKill();
        _FeverCanvasEffect.DOFade(0f, 0f);

        _FeverTextAnimation.Stop();
        _FeverTextAnimation.gameObject.SetActive(false);

        _FeverParticleEffect.SetActive(false);

        _FeverValue = 0;
        isFeverActive = false;
    }

    /*[Button("Fever Increase Test")]
    private void FeverIncrease(float value)
    {
        Push(CustomManagerEvents.FeverIncrease, value);
    }*/
}
