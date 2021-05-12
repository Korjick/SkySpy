using Assets.Scripts.Game;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float lookSensitivity = 1f;

    [Tooltip("Additional sensitivity multiplier for WebGL")]
    public float webglLookSensitivityMultiplier = 0.25f;

    [Tooltip("Limit to consider an input when using a trigger on a controller")]
    public float triggerAxisThreshold = 0.4f;

    [Tooltip("Used to flip the vertical input axis")]
    public bool invertYAxis = false;

    bool _mFireInputWasHeld;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        _mFireInputWasHeld = GetFireInputHeld();
    }

    public bool CanProcessInput()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public Vector3 GetMoveInput()
    {
        if (CanProcessInput())
        {
            Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.KAxisNameHorizontal), 0f,
                Input.GetAxisRaw(GameConstants.KAxisNameVertical));
            move = Vector3.ClampMagnitude(move, 1);

            return move;
        }

        return Vector3.zero;
    }

    public float GetRawAxis(string axisName)
    {
        return Input.GetAxisRaw(axisName);
    }

    public float GetLookInputsHorizontal()
    {
        return GetMouseLookAxis(GameConstants.KMouseAxisNameHorizontal);
    }

    public float GetLookInputsVertical()
    {
        return GetMouseLookAxis(GameConstants.KMouseAxisNameVertical);
    }

    public bool GetJumpInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown(GameConstants.KButtonNameJump);
        }

        return false;
    }

    public bool GetJumpInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton(GameConstants.KButtonNameJump);
        }

        return false;
    }

    public bool GetFireInputDown()
    {
        return GetFireInputHeld() && !_mFireInputWasHeld;
    }

    public bool GetFireInputReleased()
    {
        return !GetFireInputHeld() && _mFireInputWasHeld;
    }

    public bool GetFireInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton(GameConstants.KButtonNameFire);
        }

        return false;
    }

    public bool GetAimInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton(GameConstants.KButtonNameAim);
        }

        return false;
    }

    public bool GetSprintInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton(GameConstants.KButtonNameSprint);
        }

        return false;
    }

    public bool GetCrouchInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown(GameConstants.KButtonNameCrouch);
        }

        return false;
    }

    public bool GetCrouchInputReleased()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonUp(GameConstants.KButtonNameCrouch);
        }

        return false;
    }

    public int GetSwitchWeaponInput()
    {
        if (CanProcessInput())
        {
            string axisName = GameConstants.KButtonNameSwitchWeapon;

            if (Input.GetAxis(axisName) > 0f)
                return -1;
            else if (Input.GetAxis(axisName) < 0f)
                return 1;
            else if (Input.GetAxis(GameConstants.KButtonNameNextWeapon) > 0f)
                return -1;
            else if (Input.GetAxis(GameConstants.KButtonNameNextWeapon) < 0f)
                return 1;
        }

        return 0;
    }

    public int GetSelectWeaponInput()
    {
        if (CanProcessInput())
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                return 1;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                return 2;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                return 3;
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                return 4;
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                return 5;
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                return 6;
            else
                return 0;
        }

        return 0;
    }

    private float GetMouseLookAxis(string mouseInputName)
    {
        if (CanProcessInput())
        {
            float i = Input.GetAxisRaw(mouseInputName);
            if (invertYAxis && mouseInputName == GameConstants.KMouseAxisNameVertical)
                i *= -1f;

            i *= lookSensitivity * 0.01f;

            return i;
        }

        return 0f;
    }
}