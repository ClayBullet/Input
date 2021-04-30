using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Fields

    [Header("SPEED MOVEMENT")]
    [Space]
    [SerializeField] protected float speedMovement;

    [Header("JUMP SYSTEM")]
    [Space]
    [SerializeField] protected float forceJumpDefault;
    [SerializeField] protected float maxDelayForStopIncreasingJump;
    [SerializeField] protected float increaserHelper;
    [SerializeField] protected float speedJump;
    [SerializeField] protected float delayForAscending;
    [SerializeField] protected Transform checkHeadJump;
    [SerializeField] protected float distanceRay;
    [SerializeField] protected float coyoteTime;
    [SerializeField] protected float adjustReduceSpeed;
    private bool _delayForCheckNewJumping;
    private bool _breakFallEffectBool;
    private bool _isFallingBool;
    private bool _moveToTheRightBool;
    private bool _isInFloorBool;

    [Header("DOUBLE JUMP")]
    [Space]
    [SerializeField] protected float forceJumpDouble;
    public bool doubleJumpBool;

    [Header("FALL SYSTEM")]
    [Space]
    [SerializeField] protected float speedFall;
    [SerializeField] protected Transform[] checkFeetFall;
    [SerializeField] protected float distanceRayInFall;
    [SerializeField] protected float timeFall;
    [SerializeField] protected float delayForLanding;
    public bool isReadyForJump;

    [SerializeField] protected LayerMask maskLayerRayCheck;


    private Rigidbody _rgbPlayer;

    [Header("DASH SYSTEM")]
    [Space]
    [SerializeField] private float forceDash;
    [SerializeField] private float speedDash;
    [SerializeField] private Transform forwardTransform;
    [SerializeField] private float delayForReactiveDashEffectBool;
    [SerializeField] private float maxDistanceToAvoidDash;
    [SerializeField] private LayerMask maskLayerForDetectDashing;
    private bool _isAvailableForDashingBool;
    private PlayerAnimation _animPlayer;

    [Header("MODEL CHARACTER")]
    [Space]
    [SerializeField] private Transform characterModel;

    [HideInInspector] public bool notMovementBool;
    [HideInInspector] public bool freezeGravityBool;

   // public Transform referencePoint;
    private float _originalSpeed;
    private bool _isPressingJumpButtonBool;
    private float _currentTimePressingJumpBtn;

    [Header("COLLISION LOCK")]
    [Space]
    [SerializeField] private Transform leftLockCollision;
    [SerializeField] private Transform rightLockCollision;
    [SerializeField] private LayerMask collisionLayerMask;
    private float _xAxisLock, _yAxisLock;

    public bool isTimeDeltaTimeBool;

    private float _reduceSpeedMovementInTheAir;

    private CameraFollow _followCamera;

    public bool isIgnoredCollisionSystemCreatedBool;

    #endregion

    #region Constructors
    #endregion

    #region Getters
    #endregion

    #region Setters
    #endregion


    #region Private_Methods
    private void Awake()
    {
        _rgbPlayer = GetComponentInChildren<Rigidbody>();
        _animPlayer = GetComponent<PlayerAnimation>();
        _followCamera = FindObjectOfType<CameraFollow>();
    }

    private void Start()
    {
        _isInFloorBool = true;
        isReadyForJump = true;
        _isAvailableForDashingBool = true;
        _originalSpeed = speedMovement;
        GenerateDefaultInputs(true);
    }

    public void GenerateDefaultInputs(bool isMovement)
    {
        if (isMovement)
        {
            GameManager.managerGame.managerInput.right_Axis_Move += MoveRight;
            GameManager.managerGame.managerInput.left_Axis_Move += MoveLeft;
            GameManager.managerGame.managerInput.up_Axis_Move += MoveUp;
            GameManager.managerGame.managerInput.down_Axis_Move += MoveDown;
            GameManager.managerGame.managerInput.DownPress_JumpButton += JumpCharacter;
            GameManager.managerGame.managerInput.DownPress_JumpButton += ActivateDelayJumnpingBoolean;
            GameManager.managerGame.managerInput.UpPress_JumpButton += DisableDelayJumpingBoolean;
         
        }
        else
        {
            GameManager.managerGame.managerInput.right_Axis_Move -= MoveRight;
            GameManager.managerGame.managerInput.left_Axis_Move -= MoveLeft;
            GameManager.managerGame.managerInput.up_Axis_Move -= MoveUp;
            GameManager.managerGame.managerInput.down_Axis_Move -= MoveDown;
            GameManager.managerGame.managerInput.DownPress_JumpButton -= JumpCharacter;
            GameManager.managerGame.managerInput.DownPress_JumpButton -= ActivateDelayJumnpingBoolean;
            GameManager.managerGame.managerInput.UpPress_JumpButton -= DisableDelayJumpingBoolean;
           
        }


    }

   

    private void MoveRight()
    {
        if (notMovementBool) return;

        transform.Translate(transform.right * Time.deltaTime * speedMovement, Space.World);
    

    }

    private void MoveLeft()
    {
        if (notMovementBool) return;

        transform.Translate(-transform.right * Time.deltaTime * speedMovement, Space.World);
      

    }

    private void MoveUp()
    {
        if (notMovementBool) return;

        transform.Translate(transform.forward * Time.deltaTime * speedMovement, Space.World);

    }

    private void MoveDown()
    {
        if (notMovementBool) return;

        transform.Translate(-transform.forward * Time.deltaTime * speedMovement, Space.World);

    }

    private void ActivateDelayJumnpingBoolean()
    {
        _isPressingJumpButtonBool = true;

    }

    private void DisableDelayJumpingBoolean()
    {
        _isPressingJumpButtonBool = false;
    }

    private void Update()
    {
        if (!lowerThanMyFeetsBool() && !_isFallingBool && isReadyForJump && _isInFloorBool && !notMovementBool)
        {
            StartCoroutine(FallDescending());

        }

        if (lowerThanMyFeetsBool() && !_isFallingBool && isReadyForJump && _isInFloorBool)
        {
            _currentTimePressingJumpBtn = 0;
        }


        if (_isInFloorBool)
            VerifyLanding();

        if(_animPlayer != null)
            _animPlayer.FloorAnimated(_isInFloorBool);
    }

    private void VerifyLanding()
    {
        if (_animPlayer != null && _animPlayer.recoverAnimationStatePlayerBool(AnimationManager.stateAnimationJumpDescending))
        {
            _animPlayer.LandingAnimated(true);

        }
        else
        {
            if(_animPlayer != null)
                _animPlayer.LandingAnimated(false);
        }
    }

  
    private void StayFreezeTheMovement()
    {
        _animPlayer.MovementPlayerAnimated(0f);
    }
    private void JumpCharacter()
    {
        if (notMovementBool) return;

        if (lowerThanMyFeetsBool() && isReadyForJump)
        {
            StartCoroutine(JumpAscending(Vector3.up, forceJumpDefault, speedJump));
        }
        else if (doubleJumpBool)
        {
            StartCoroutine(JumpAscending(Vector3.up, forceJumpDouble, speedJump));
            _animPlayer.DoubleJumpPlayerAnimated(true);

        }


    }


    protected IEnumerator JumpAscending(Vector3 vectorToGo, float force, float speed)
    {
        OriginalSpeed();


        _followCamera.specialSituationForChangeOffsetCameraBool = true;
        _reduceSpeedMovementInTheAir = _originalSpeed / adjustReduceSpeed;

        _animPlayer.JumpPlayerAnimated(true);
        _animPlayer.FallPlayerAnimated(false);

        _isInFloorBool = false;

        if (_delayForCheckNewJumping) yield break;
        _delayForCheckNewJumping = true;

        StartCoroutine(DelayForOverJumpAgain_Coroutine());

        if (!isReadyForJump)
        {
            doubleJumpBool = false;
            _animPlayer.DoubleJumpPlayerAnimated(true);
            _currentTimePressingJumpBtn = 0;
        }

        _breakFallEffectBool = true;


        isReadyForJump = false;

        _rgbPlayer.useGravity = false;


        float _currentTime = 0f;
        float _maxTime = .1f;
        for (float i = 0; i < force; i++)
        {
            yield return new WaitForEndOfFrame();



            if (overMyHeadMadeIStopBool())
            {
                break;
            }

            speedMovement = _reduceSpeedMovementInTheAir;

            yield return new WaitUntil(() => !freezeGravityBool);

            transform.Translate(vectorToGo * speed * Time.fixedDeltaTime, Space.World);


            if (_currentTime > _maxTime)
                force += IncreaserJumpForce();


            _currentTime += Time.fixedDeltaTime;
        }


        _breakFallEffectBool = false;


        StartCoroutine(FallDescending());
    }


    private float IncreaserJumpForce()
    {

        if (_isPressingJumpButtonBool && maxDelayForStopIncreasingJump > _currentTimePressingJumpBtn)
        {
            _currentTimePressingJumpBtn += Time.fixedDeltaTime;


            return increaserHelper;
        }

        return 0;
    }

    protected IEnumerator FallDescending()
    {
        _followCamera.specialSituationForChangeOffsetCameraBool = true;

        OriginalSpeed();
        _reduceSpeedMovementInTheAir = _originalSpeed / adjustReduceSpeed;

        if (!doubleJumpBool)
        {
            _animPlayer.DoubleJumpPlayerAnimated(false);
        }

        _isInFloorBool = false;

        _animPlayer.FallPlayerAnimated(true);

        _isFallingBool = true;
        isReadyForJump = false;

        _rgbPlayer.useGravity = false;
        yield return new WaitForSecondsRealtime(coyoteTime);
        while (!lowerThanMyFeetsBool())
        {

            speedMovement = _reduceSpeedMovementInTheAir;
            if (_breakFallEffectBool)
            {
                yield break;
            }
            yield return new WaitUntil(() => !freezeGravityBool);



            yield return new WaitForEndOfFrame();

            transform.Translate(Vector3.down * speedFall * Time.fixedDeltaTime, Space.World);

        }

        _rgbPlayer.useGravity = true;

        _animPlayer.JumpPlayerAnimated(false);
        _animPlayer.FallPlayerAnimated(false);

        _animPlayer.LandingAnimated(true);

        _animPlayer.DoubleJumpPlayerAnimated(false);

        yield return new WaitForSecondsRealtime(delayForLanding);

        _animPlayer.LandingAnimated(false);

        isReadyForJump = true;

        doubleJumpBool = true;
        _isFallingBool = false;

        _isInFloorBool = true;
        OriginalSpeed();
        _followCamera.specialSituationForChangeOffsetCameraBool = false;

    }

    private IEnumerator DelayForOverJumpAgain_Coroutine()
    {
        yield return new WaitForSeconds(.2f);
        _delayForCheckNewJumping = false;
    }


    private bool overMyHeadMadeIStopBool()
    {
        RaycastHit hit;
        
        
        if(Physics.Linecast(transform.position, checkHeadJump.transform.position, out hit, maskLayerRayCheck))
        {
            if (hit.collider != null)
                return true;


        }



        return false;
    }

    private bool lowerThanMyFeetsBool()
    {
        RaycastHit hit;
        int currentNumberOfCollisions = 0;
        for (int i = 0; i < checkFeetFall.Length; i++)
        {

        
            if (Physics.Linecast(transform.position, checkFeetFall[i].transform.position, out hit, maskLayerRayCheck))
            {
                if (hit.collider == null)
                {
                    currentNumberOfCollisions += 1;
                }

            }
        }

        if (currentNumberOfCollisions >= 3)
        {
            return false;

        }

        return true;
    }


    private void DashControl()
    {
       
        StartCoroutine(DashMovement_Coroutine(transform.forward, forceDash, speedDash));
          

    }

    public IEnumerator DashMovement_Coroutine(Vector3 vectorToGo, float force, float speed)
    {

        if (!_isAvailableForDashingBool) yield break;
        freezeGravityBool = true;
        notMovementBool = true;


        _animPlayer.DashPlayerAnimated(true);

        _isAvailableForDashingBool = false;

        StartCoroutine(DelayForOverDashAgain());

        for (float i = 0; i < force; i++)
        {
            if (!isTimeDeltaTimeBool)
                yield return new WaitForEndOfFrame();
            else
                yield return new WaitForSecondsRealtime(0.02f);

            if (stopDashForCollideWithSomethingBool(vectorToGo))
            {
                break;
            }


            transform.Translate(vectorToGo * speed * Time.fixedDeltaTime, Space.World);


        }

        _animPlayer.DashPlayerAnimated(false);
        freezeGravityBool = false;
        notMovementBool = false;

    }


    private IEnumerator DelayForOverDashAgain()
    {
        yield return new WaitForSeconds(delayForReactiveDashEffectBool);
        _isAvailableForDashingBool = true;
    }

    private bool stopDashForCollideWithSomethingBool(Vector3 dirToMovement)
    {
        RaycastHit hit;

        if(Physics.Linecast(transform.transform.position, forwardTransform.transform.position, out hit, maskLayerRayCheck))
        {
            if (hit.collider != null)
            {
                return true;

            }
        }
      

        return false;
    }

  
    #endregion


    #region Public_Methods
    public void IncreaseSpeed(float speed)
    {
        speedMovement += speed;
    }

    public void OriginalSpeed()
    {
        speedMovement = _originalSpeed;
    }
    public void RigidBodySimulated(bool isSimulated)
    {
        _rgbPlayer.useGravity = isSimulated;
    }

  
    #endregion

    #region Static_Methods
    #endregion
}
