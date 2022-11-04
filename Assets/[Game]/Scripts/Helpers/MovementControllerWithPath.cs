using Game.GlobalVariables;
using Sirenix.OdinInspector;
using System;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;

public class MovementControllerWithPath : Actor<LevelManager>
{
    bool canMoveForward;
    bool canMoveHorizontal;

    #region Movement Settings
    [TitleGroup("MovementSettings")]
    [EnumToggleButtons] public MovementTypes SelectedMovementType;

    [HorizontalGroup("MovementSettings/Split")]
    [TabGroup("MovementSettings/Split/Parameters", "Slider")]
    public SliderProperties _SliderProperties;

    [TabGroup("MovementSettings/Split/Parameters", "Joystick")]
    public JoystickProperties _JoystickProperties;

    float firstClick = 0;
    #endregion

    [Title("Movement Position Settings")]
    [SerializeField] private float _LocalHorizontalMaxMinValue = 3.5f;

    [Title("Movement Rotation Settings")]
    [SerializeField] private bool UseModelRotate = true;
    [SerializeField] private float _ModelRotationSpeed = 5f;
    [SerializeField] private float _ModelRotatePowerValue = 1f;
    [SerializeField] private float _ModelRotateMinMaxValue = 1f;

    [SerializeField] Transform model_Transform;

    #region Properties
    [Serializable]
    public class SliderProperties
    {
        public float _VerticalSpeed = 5;
        public float _HorizontalSpeed = 1;

        [HideInInspector] public float _DeltaHorizontalValue;
        [HideInInspector] public float _ForwardValue;
    }

    [Serializable]
    public class JoystickProperties
    {
        public JoystickType joystickType;
        public bool IsShowJoystick;

        [HideInInspector] public VariableJoystick variableJoystick;

        [Header("Opsiyonel")]
        public float _VerticalSpeed = 5;
        public float _HorizontalSpeed = 10;

        [HideInInspector] public float _DeltaHorizontalValue;
        [HideInInspector] public float _ForwardValue;
    }
    #endregion

    public enum MovementTypes
    {
        Slider, Joystick
    }

    protected override void MB_Listen(bool status)
    {
        if (status)
        {
            GameManager.Instance.Subscribe(CustomManagerEvents.SendJoystick, FetchJoystick);
            GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Start, GameStarted);
            Manager.Subscribe(CustomManagerEvents.PlayerReachedEndOfPath, ReachedEndOfPath);
        }
        else
        {
            GameManager.Instance.Unsubscribe(CustomManagerEvents.SendJoystick, FetchJoystick);
            GameManager.Instance.Unsubscribe(ManagerEvents.GameStatus_Start, GameStarted);
            Manager.Unsubscribe(CustomManagerEvents.PlayerReachedEndOfPath, ReachedEndOfPath);
        }
    }

    void GameStarted(object[] a)
    {
        SetCanMoveForward(true);
        canMoveHorizontal = true;

    } // GameStarted()

    void SetCanMoveForward(bool _canMoveForward)
    {
        canMoveForward = _canMoveForward;
        Push(CustomManagerEvents.PlayerCanFollowPath, _canMoveForward);

    } // SetCanMoveForward()

    void ReachedEndOfPath(object[] a)
    {
        canMoveForward = false;
        canMoveHorizontal = false;

    } // ReachedEndOfPath()

    private void FetchJoystick(object[] args)
    {
        if (SelectedMovementType != MovementTypes.Joystick) return;

        _JoystickProperties.variableJoystick = (VariableJoystick)args[0];
        _JoystickProperties.variableJoystick.joystickType = _JoystickProperties.joystickType;
        _JoystickProperties.variableJoystick.IsShowJoystick = _JoystickProperties.IsShowJoystick;
    }

    protected override void MB_Update()
    {
        if (GameManager.Instance.IsGameOver || !GameManager.Instance.IsGameStarted) return;

        _SliderProperties._ForwardValue = 0;
        _JoystickProperties._ForwardValue = 0;

        /*if (!canMoveForward)
        {
            firstClick = Input.mousePosition.x;

            return;
        }*/

        switch (SelectedMovementType)
        {
            case MovementTypes.Slider:
                #region Slider
                _SliderProperties._ForwardValue = _SliderProperties._VerticalSpeed * Time.deltaTime;

                if (Input.GetMouseButtonDown(0))
                {
                    _SliderProperties._DeltaHorizontalValue = 0;

                    if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                    {
                        firstClick = Input.mousePosition.x;
                    }
                    else
                    {
                        firstClick = (Input.GetTouch(0).position.x + Screen.width);
                    }
                }

                if (Input.GetMouseButton(0))
                {
                    if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                    {
                        float delta = (Input.mousePosition.x - firstClick);

                        _SliderProperties._DeltaHorizontalValue = (delta * _SliderProperties._HorizontalSpeed) * Time.deltaTime;
                        firstClick = Input.mousePosition.x;
                    }
                    else
                    {
                        float delta = ((Input.GetTouch(0).position.x + Screen.width) - firstClick);

                        _SliderProperties._DeltaHorizontalValue = (delta * _SliderProperties._HorizontalSpeed) * Time.deltaTime;
                        firstClick = (Input.GetTouch(0).position.x + Screen.width);
                    }
                }
                else
                {
                    _SliderProperties._DeltaHorizontalValue = 0;
                }

                #endregion
                break;
            case MovementTypes.Joystick:
                #region Joystick

                _JoystickProperties._DeltaHorizontalValue = (_JoystickProperties.variableJoystick.Direction.x * _JoystickProperties._HorizontalSpeed) * Time.deltaTime;
                _JoystickProperties._ForwardValue = _JoystickProperties._VerticalSpeed * Time.deltaTime;

                //  _JoystickProperties.variableJoystick istediðiniz veriyi burdan alabilirsiniz. Mini game playerinizin içinde..
                #endregion
                break;
            default:
                break;
        }

        #region Movement

        Quaternion currentRot = model_Transform.rotation;
        float newRotY = 0;

        Vector3 newDirection = Vector3.zero;
        Vector3 currentPos = Vector3.zero;

        switch (SelectedMovementType)
        {
            case MovementTypes.Slider:
                #region Rotation
                if (UseModelRotate && canMoveHorizontal)
                {
                    currentRot.y = newRotY;
                    newRotY = Mathf.Clamp(currentRot.y + (_SliderProperties._DeltaHorizontalValue * _ModelRotatePowerValue), -_ModelRotateMinMaxValue, _ModelRotateMinMaxValue);
                    currentRot.y = newRotY;
                    model_Transform.localRotation = Quaternion.Lerp(model_Transform.localRotation, currentRot, Time.deltaTime * _ModelRotationSpeed);
                }
                #endregion

                #region Position
                if (canMoveHorizontal)
                {
                    newDirection.x = _SliderProperties._DeltaHorizontalValue;
                }
                newDirection.y = 0f;

                transform.localPosition += newDirection;
                transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -_LocalHorizontalMaxMinValue, _LocalHorizontalMaxMinValue), transform.localPosition.y, 0f);
                #endregion

                break;
            case MovementTypes.Joystick:
                #region Rotation
                if (UseModelRotate && canMoveHorizontal)
                {
                    currentRot.y = newRotY;
                    newRotY = Mathf.Clamp(currentRot.y + (_JoystickProperties._DeltaHorizontalValue * _ModelRotatePowerValue), -_ModelRotateMinMaxValue, _ModelRotateMinMaxValue);
                    currentRot.y = newRotY;
                    model_Transform.localRotation = Quaternion.Lerp(model_Transform.localRotation, currentRot, Time.deltaTime * _ModelRotationSpeed);

                }
                #endregion

                #region Position
                if (canMoveHorizontal)
                {
                    newDirection.x = _JoystickProperties._DeltaHorizontalValue;
                }
                newDirection.y = 0f;

                transform.localPosition += newDirection;
                transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -_LocalHorizontalMaxMinValue, _LocalHorizontalMaxMinValue), transform.localPosition.y, 0f);
                #endregion

                break;
            default:
                break;
        }
        #endregion

    } // MB_Update()


} // class
