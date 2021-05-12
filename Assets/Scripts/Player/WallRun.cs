using UnityEngine;
using System.Linq;
using Assets.Scripts.Game;
using UnityEngine.Rendering;

[RequireComponent (typeof(PlayerCharacterController))]
public class WallRun : MonoBehaviour
{
    public float wallMaxDistance = 1;
    public float wallSpeedMultiplier = 1.2f;
    public float minimumHeight = 1.2f;
    public float maxAngleRoll = 20;
    [Range(0.0f, 1.0f)]
    public float normalizedAngleThreshold = 0.1f;
    
    public float jumpDuration = 1;
    public float wallBouncing = 3;
    public float cameraTransitionDuration = 1;

    public float wallGravityDownForce = 20f;

    public bool useSprint;
    
    [Space]
    public Volume wallRunVolume;

    private PlayerCharacterController _playerCharacterController;
    private PlayerInputHandler _inputHandler;

    private Vector3[] _directions;
    private RaycastHit[] _hits;

    private bool _isWallRunning;
    private Vector3 _lastWallPosition;
    private Vector3 _lastWallNormal;
    private float _elapsedTimeSinceJump;
    private float _elapsedTimeSinceWallAttach;
    private float _elapsedTimeSinceWallDetatch;
    private bool _jumping;
    private float _lastVolumeValue;
    private bool IsPlayergrounded => _playerCharacterController.IsGrounded;

    public bool IsWallRunning() => _isWallRunning;

    private bool CanWallRun()
    {
        float verticalAxis = _inputHandler.GetRawAxis(GameConstants.KAxisNameVertical);
        bool isSprinting = _inputHandler.GetSprintInputHeld();
        isSprinting = !useSprint || isSprinting;
        
        return !IsPlayergrounded && verticalAxis > 0 && VerticalCheck() && isSprinting;
    }

    private bool VerticalCheck()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumHeight);
    }


    private void Start()
    {
        _playerCharacterController = GetComponent<PlayerCharacterController>();
        _inputHandler = GetComponent<PlayerInputHandler>();

         _directions = new Vector3[]{ 
            Vector3.right, 
            Vector3.right + Vector3.forward,
            Vector3.forward, 
            Vector3.left + Vector3.forward, 
            Vector3.left
        };

        if(wallRunVolume != null)
        {
            SetVolumeWeight(0);
        }
    }


    private void LateUpdate()
    {  
        _isWallRunning = false;

        if(_inputHandler.GetJumpInputDown())
        {
            _jumping = true;
        }

        if(CanAttach())
        {
            _hits = new RaycastHit[_directions.Length];

            for(int i=0; i<_directions.Length; i++)
            {
                Vector3 dir = transform.TransformDirection(_directions[i]);
                Physics.Raycast(transform.position, dir, out _hits[i], wallMaxDistance);
                if(_hits[i].collider != null)
                {
                    Debug.DrawRay(transform.position, dir * _hits[i].distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position, dir * wallMaxDistance, Color.red);
                }
            }

            if(CanWallRun())
            {   
                _hits = _hits.ToList().Where(h => h.collider != null).OrderBy(h => h.distance).ToArray();
                if(_hits.Length > 0)
                {
                    OnWall(_hits[0]);
                    _lastWallPosition = _hits[0].point;
                    _lastWallNormal = _hits[0].normal;
                }
            }
        }

        if(_isWallRunning)
        {
            _elapsedTimeSinceWallDetatch = 0;
            if(_elapsedTimeSinceWallAttach == 0 && wallRunVolume != null)
            {
                _lastVolumeValue = wallRunVolume.weight;
            }
            _elapsedTimeSinceWallAttach += Time.deltaTime;
            _playerCharacterController.CharacterVelocity += Vector3.down * wallGravityDownForce * Time.deltaTime;
        }
        else
        {   
            _elapsedTimeSinceWallAttach = 0;
            if(_elapsedTimeSinceWallDetatch == 0 && wallRunVolume != null)
            {
                _lastVolumeValue = wallRunVolume.weight;
            }
            _elapsedTimeSinceWallDetatch += Time.deltaTime;
        }

        if(wallRunVolume != null)
        {
            HandleVolume();
        }
    }

    private bool CanAttach()
    {
        if(_jumping)
        {
            _elapsedTimeSinceJump += Time.deltaTime;
            if(_elapsedTimeSinceJump > jumpDuration)
            {
                _elapsedTimeSinceJump = 0;
                _jumping = false;
            }
            return false;
        }
        
        return true;
    }

    private void OnWall(RaycastHit hit){
        float d = Vector3.Dot(hit.normal, Vector3.up);
        if(d >= -normalizedAngleThreshold && d <= normalizedAngleThreshold)
        {
            float vertical = Input.GetAxisRaw(GameConstants.KAxisNameVertical);
            Vector3 alongWall = transform.TransformDirection(Vector3.forward);

            Debug.DrawRay(transform.position, alongWall.normalized * 10, Color.green);
            Debug.DrawRay(transform.position, _lastWallNormal * 10, Color.magenta);

            _playerCharacterController.CharacterVelocity = alongWall * vertical * wallSpeedMultiplier;
            _isWallRunning = true;
        }
    }

    private float CalculateSide()
    {
        if(_isWallRunning)
        {
            Vector3 heading = _lastWallPosition - transform.position;
            Vector3 perp = Vector3.Cross(transform.forward, heading);
            float dir = Vector3.Dot(perp, transform.up);
            return dir;
        }
        return 0;
    }

    public float GetCameraRoll()
    {
        float dir = CalculateSide();
        float cameraAngle = _playerCharacterController.playerCamera.transform.eulerAngles.z;
        float targetAngle = 0;
        if(dir != 0)
        {
            targetAngle = Mathf.Sign(dir) * maxAngleRoll;
        }
        return Mathf.LerpAngle(cameraAngle, targetAngle, Mathf.Max(_elapsedTimeSinceWallAttach, _elapsedTimeSinceWallDetatch) / cameraTransitionDuration);
    } 

    public Vector3 GetWallJumpDirection()
    {
        if(_isWallRunning)
        {
            return _lastWallNormal * wallBouncing + Vector3.up;
        }
        return Vector3.zero;
    } 

    private void HandleVolume()
    {
        float w = 0;
        if(_isWallRunning)
        {
            w = Mathf.Lerp(_lastVolumeValue, 1, _elapsedTimeSinceWallAttach / cameraTransitionDuration);
        }
        else
        {
            w = Mathf.Lerp(_lastVolumeValue, 0, _elapsedTimeSinceWallDetatch / cameraTransitionDuration);
        }

        SetVolumeWeight(w);
    }

    private void SetVolumeWeight(float weight)
    {
        wallRunVolume.weight = weight;
    }
}
