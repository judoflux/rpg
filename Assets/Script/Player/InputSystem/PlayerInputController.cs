using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private Player player => GetComponent<Player>();
    private PlayerInputAction playerInputActions;
    private PlayerData playerData => player.playerData;
    public Vector2 RawMovementInput;
    public int norInputX;
    public int NormInputX => norInputX; // 提供只读访问
    public int norInputY;
    public bool runInput { get; private set; }
    public bool runJumpInput { get; private set; }
    public bool sprintJumpInput { get; private set; }
    public bool sprintInput { get; private set; }
    public bool grabInput { get; private set; } = false;
    public bool jumpInputStop { get; private set; }
    public bool isTouchingWall; // 是否正在触碰墙的标志位
    public bool isCrouchInput { get; private set; }
    public bool isClimbLedgeDown { get; private set; }
    public bool isClimbLedgeUp { get; private set; }
    [SerializeField] private float runJumpInputHoldTime = 0.2f;
    [SerializeField] private float sprintJumpInputHoldTime = 0.2f;
    private float runJumpInputStartTime;
    private float sprintJumpInputStartTime;
    private bool isRun;
    private bool isIdle;
    private bool isSprint;
    private bool isCrouch;
    public Vector3 moveDirection;
    private bool isTouchingLedgeDown;
    private bool isTouchingCeiling;

    private void Awake()
    {
        playerInputActions = new PlayerInputAction();
    }

    private void OnEnable()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Move.performed += OnMovement;
            playerInputActions.Player.Move.canceled += OnMovement;
            //playerInputActions.Player.WallGrab.performed += OnGrabInputPerformed; // 绑定事件
            //playerInputActions.Player.WallGrab.Enable();
            playerInputActions.Player.Move.Enable();
        }
    }

    private void OnDisable()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Move.performed -= OnMovement;
            playerInputActions.Player.Move.canceled -= OnMovement;
            //playerInputActions.Player.WallGrab.performed -= OnGrabInputPerformed;
            //// 绑定事件
            //playerInputActions.Player.WallGrab.Disable();
            playerInputActions.Player.Move.Disable();
        }
    }

    private void Update()
    {
        isRun = playerData.isRun;
        isIdle = playerData.isIdle;
        isSprint = playerData.isSprint;
        isCrouch = playerData.isCrouch;
        isTouchingLedgeDown = player.LedgeDownTriggerDetection.isTouchingLedgeDown;
        isTouchingCeiling = player.CheckIfTouchingCeiling();
        if (player.isBusy)
        {
            Debug.Log("Player is busy from input");
            return;
        }
        if (norInputX != 0 && isSprint)
        {

            CheckSprintJumpInputHoldTime();
        }



        if (norInputX != 0 || norInputX == 0)
        {
            CheckRunJumpInputHoldTime();
        }


        if (!player.isTouchingWall && grabInput)
        {
            grabInput = false; // 玩家离开墙壁，重置抓取状态
            Debug.Log("Player left the wall, grabInput reset to false.");
            ReleaseInput(); // 调用释放逻辑
        }




    }

    public void OnMovement(InputAction.CallbackContext context)
    {

        RawMovementInput = context.ReadValue<Vector2>();
        if (Mathf.Abs(RawMovementInput.x) > 0.5f)
        {
            norInputX = Mathf.RoundToInt((RawMovementInput * Vector2.right).normalized.x);
        }
        else
        {
            norInputX = 0;
        }

        if (Mathf.Abs(RawMovementInput.y) > 0.5f)
        {
            norInputY = Mathf.RoundToInt((RawMovementInput * Vector2.up).normalized.y);
        }
        else
        {
            norInputY = 0;
        }
        // 示例：根据输入控制玩家位置移动
        if (context.performed)
        {
            runInput = true; // Set runInput to true when movement input is pressed
        }
        else if (context.canceled)
        {
            runInput = false; // Reset runInput when the movement input is released
        }

    }

    public void OnSprintInput(InputAction.CallbackContext context)
    {
        if (context.performed && playerData.isRun)
        {

            sprintInput = true;

        }

        if (context.canceled)
        {
            UseSprintInput();
        }
    }

    public void OnCouchInput(InputAction.CallbackContext context)
    {

        if (context.started && !isTouchingLedgeDown && playerData.isGroundedState && !isTouchingCeiling)
        {
            Debug.LogWarning("Crouch Input Triggered");
            isCrouchInput = !isCrouchInput;
        }
    }
    public void OnClimbLedgeDownInput(InputAction.CallbackContext context)
    {
        if (context.started && isTouchingLedgeDown)
        {
            Debug.LogWarning("Climb Ledge Down Input Triggered");
            isClimbLedgeDown = true;
        }
        if (context.canceled)
            isClimbLedgeDown = false;
    }
    public void OnLedgeClimbUpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.LogWarning("Ledge Climb Up Input Triggered");
            isClimbLedgeUp = true;
        }
        if (context.canceled)
            isClimbLedgeUp = false;
    }

    public void OnGrabInputPerformed(InputAction.CallbackContext context)
    {
        // 检查玩家是否接触到墙壁
        if (!player.isTouchingWall)
        {
            Debug.Log("Cannot grab! Player is not touching the wall.");
            return; // 如果没有接触墙壁，则直接返回
        }

        // 只有在按键触发（context.started）时切换 grabInput 的状态
        if (context.started)
        {
            grabInput = !grabInput; // 切换状态

            if (grabInput)
            {
                // grabInput 为 true 时执行
                OnGrabInput();
            }
            else
            {
                // grabInput 为 false 时执行
                ReleaseInput();
            }
        }

    }

    private void OnGrabInput()
    {
        grabInput = true;
        // Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        // Cursor.visible = false; // 隐藏鼠标光标
        Debug.Log("Input grabbed");
    }

    private void ReleaseInput()
    {
        grabInput = false;
        // Cursor.lockState = CursorLockMode.None; // 释放鼠标锁定
        // Cursor.visible = true; // 显示鼠标光标
        Debug.Log("Input released");
    }

    public void OnRunJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {

            runJumpInput = true;
            jumpInputStop = false;
            runJumpInputStartTime = Time.time;
        }

        if (context.canceled)
        {
            jumpInputStop = true;
        }


    }

    public void OnSprintJumpInput(InputAction.CallbackContext context)
    {
        if (context.started && playerData.isSprint)
        {
            Debug.Log("Sprint Jump Input");

            sprintJumpInput = true;
            sprintJumpInputStartTime = Time.time;
        }


    }

    public void UseRunJumpInput()
    {

        runJumpInput = false;

    }


    public void UseSprintJumpInput()
    {
        Debug.Log("input false");
        sprintJumpInput = false;

    }


    public void UseSprintInput()
    {
        sprintInput = false;
        Debug.Log("UseSprintInput");
    }

    public void UseCrouchInput()
    {
        isCrouchInput = false;
    }
    public void UseLedgeClimbDownInput()
    {
        isClimbLedgeDown = false;
    }
    public void CancelAllJumpInput()
    {
        UseRunJumpInput();
        UseSprintJumpInput();
    }

    private void CheckRunJumpInputHoldTime()
    {
        if (Time.time >= runJumpInputStartTime + runJumpInputHoldTime)
        {
            float time = runJumpInputStartTime + runJumpInputHoldTime;
            runJumpInput = false;

        }
    }


    private void CheckSprintJumpInputHoldTime()
    {
        if (Time.time >= sprintJumpInputStartTime + sprintJumpInputHoldTime)
        {
            float time = sprintJumpInputStartTime + sprintJumpInputHoldTime;
            sprintJumpInput = false;

        }
    }
}

//using System;
//using System.Collections.Generic;
//using TMPro.Examples;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using Yushan.Enums;

//[System.Serializable]
//public class InputBinding
//{
//    public string actionName;
//    public InputActionReference actionReference;
//    public string defaultBindingPath;
//}

//public class PlayerInputController : MonoBehaviour
//{
//    private Player player => GetComponent<Player>();
//    private PlayerInputAction playerInputActions;
//    private UI ui;
//    private PlayerData playerData => player.playerData;
//    private Yushan.Enums.EquipmentType CurrentEquipment => Yushan.Enums.EquipmentType.None; // 這裡可以根據實際裝備系統進行調整
//    // 移動輸入
//    public Vector2 RawMovementInput { get; private set; }
//    public int NormInputX { get; private set; }
//    public int NormInputY { get; private set; }

//    // 狀態輸入
//    public bool RunInput { get; private set; }
//    public bool RunJumpInput { get; private set; }
//    public bool SprintJumpInput { get; private set; }
//    public bool SprintInput { get; private set; }
//    public bool GrabInput { get; private set; }
//    public bool JumpInputStop { get; private set; }
//    public bool IsCrouchInput { get; private set; }
//    public bool IsClimbLedgeDown { get; private set; }
//    public bool IsClimbLedgeUp { get; private set; }
//    public bool DashInput { get; private set; }
//    public bool DashBackInput { get; private set; }
//    public bool LockOnInput { get; private set; }
//    public bool InteractInput { get; private set; }

//    // 自動互動標記
//    public bool ShouldAutoGrabLadder { get; set; }
//    public bool ShouldAutoWallSlide { get; set; }

//    // 鎖定系統
//    public GameObject LockedTarget { get; private set; }
//    public Vector2 LockDirection => LockedTarget ?
//        (LockedTarget.transform.position - transform.position).normalized :
//        Vector2.zero;

//    // 輸入時間控制
//    [SerializeField] private float runJumpInputHoldTime = 0.2f;
//    [SerializeField] private float sprintJumpInputHoldTime = 0.2f;
//    [SerializeField] private float doubleTapTimeThreshold = 0.3f;
//    private float runJumpInputStartTime;
//    private float sprintJumpInputStartTime;
//    private float lastDashTapTime;
//    private float lastBackDashTapTime;
//    private int consecutiveDashTaps;
//    private int consecutiveBackDashTaps;



//    public bool EquipmentActive { get; private set; }

//    // 可自訂輸入
//    public List<InputBinding> customBindings = new List<InputBinding>();

//    private void Awake()
//    {
//        playerInputActions = new PlayerInputAction();
//        SetupCustomInputs();
//    }

//    private void SetupCustomInputs()
//    {
//        // 初始化自訂輸入
//        customBindings.Add(new InputBinding
//        {
//            actionName = "Dash",
//            defaultBindingPath = "<Keyboard>/d"
//        });

//        customBindings.Add(new InputBinding
//        {
//            actionName = "DashBack",
//            defaultBindingPath = "<Keyboard>/a"
//        });

//        customBindings.Add(new InputBinding
//        {
//            actionName = "LockOn",
//            defaultBindingPath = "<Mouse>/rightButton"
//        });

//        customBindings.Add(new InputBinding
//        {
//            actionName = "Equipment",
//            defaultBindingPath = "<Keyboard>/q"
//        });

//        // 加載玩家保存的按鍵設置
//        LoadCustomBindings();
//    }

//    private void OnEnable()
//    {
//        if (playerInputActions != null)
//        {
//            // 基本移動
//            playerInputActions.Player.Move.performed += OnMovement;
//            playerInputActions.Player.Move.canceled += OnMovement;
//            playerInputActions.Player.Move.Enable();

//            // 跳躍
//            playerInputActions.Player.Jump.performed += OnRunJumpInput;
//            playerInputActions.Player.Jump.canceled += OnRunJumpInput;
//            playerInputActions.Player.Jump.Enable();

//            // 衝刺
//            playerInputActions.Player.Sprint.performed += OnSprintInput;
//            playerInputActions.Player.Sprint.canceled += OnSprintInput;
//            playerInputActions.Player.Sprint.Enable();

//            // 蹲下 (改為按住觸發)
//            playerInputActions.Player.Crouch.performed += OnCrouchInput;
//            playerInputActions.Player.Crouch.canceled += OnCrouchInput;
//            playerInputActions.Player.Crouch.Enable();

//            // 互動
//            playerInputActions.Player.InteractAndEvents.performed += OnInteractInput;
//            playerInputActions.Player.InteractAndEvents.Enable();

//            // 裝備使用
//            playerInputActions.Player.SwitchWeapons.performed += OnEquipmentInput;
//            playerInputActions.Player.SwitchWeapons.Enable();

//            // 鎖定目標
//            playerInputActions.Player.Toggle.performed += OnLockOnInput;
//            playerInputActions.Player.Toggle.Enable();
//        }
//    }

//    private void OnDisable()
//    {
//        if (playerInputActions != null)
//        {
//            playerInputActions.Player.Move.performed -= OnMovement;
//            playerInputActions.Player.Move.canceled -= OnMovement;
//            playerInputActions.Player.Jump.performed -= OnRunJumpInput;
//            playerInputActions.Player.Jump.canceled -= OnRunJumpInput;
//            playerInputActions.Player.Sprint.performed -= OnSprintInput;
//            playerInputActions.Player.Sprint.canceled -= OnSprintInput;
//            playerInputActions.Player.Crouch.performed -= OnCrouchInput;
//            playerInputActions.Player.Crouch.canceled -= OnCrouchInput;
//            playerInputActions.Player.InteractAndEvents.performed -= OnInteractInput;
//            playerInputActions.Player.SwitchWeapons.performed -= OnEquipmentInput;
//            playerInputActions.Player.Toggle.performed -= OnLockOnInput;
//        }
//    }

//    private void Update()
//    {
//        if (player.isBusy) return;

//        // 自動互動檢測
//        AutoInteractionCheck();

//        // 雙擊檢測
//        CheckDoubleTaps();

//        // 輸入時間限制檢查
//        CheckInputHoldTimes();
//    }

//    private void AutoInteractionCheck()
//    {
//        // 自動抓梯子
//        ShouldAutoGrabLadder = player.IsLadderDetected() && (player.stateMachine.currentState != player.wallGrabState);


//        //// 自動牆壁滑行
//        //ShouldAutoWallSlide = player.WallDetection.IsTouchingWall &&
//        //                     !player.GroundDetection.IsGrounded &&
//        //                     player.Rigidbody.velocity.y < 0;
//    }

//    private void CheckDoubleTaps()
//    {
//        // 雙擊前衝檢測
//        if (Time.time - lastDashTapTime > doubleTapTimeThreshold)
//        {
//            consecutiveDashTaps = 0;
//        }

//        // 雙擊後閃檢測
//        if (Time.time - lastBackDashTapTime > doubleTapTimeThreshold)
//        {
//            consecutiveBackDashTaps = 0;
//        }
//    }

//    public void OnMovement(InputAction.CallbackContext context)
//    {
//        RawMovementInput = context.ReadValue<Vector2>();

//        // 標準化X輸入
//        NormInputX = Mathf.Abs(RawMovementInput.x) > 0.5f ?
//            (int)Mathf.Sign(RawMovementInput.x) : 0;

//        // 標準化Y輸入
//        NormInputY = Mathf.Abs(RawMovementInput.y) > 0.5f ?
//            (int)Mathf.Sign(RawMovementInput.y) : 0;

//        // 雙擊檢測 (衝刺)
//        if (NormInputX > 0 && context.performed)
//        {
//            consecutiveDashTaps++;
//            if (consecutiveDashTaps == 2)
//            {
//                DashInput = true;
//                consecutiveDashTaps = 0;
//            }
//            lastDashTapTime = Time.time;
//        }

//        // 雙擊檢測 (後閃)
//        if (NormInputX < 0 && context.performed)
//        {
//            consecutiveBackDashTaps++;
//            if (consecutiveBackDashTaps == 2)
//            {
//                DashBackInput = true;
//                consecutiveBackDashTaps = 0;
//            }
//            lastBackDashTapTime = Time.time;
//        }

//        // 奔跑輸入
//        RunInput = context.performed && RawMovementInput.magnitude > 0.1f;
//    }

//    public void OnRunJumpInput(InputAction.CallbackContext context)
//    {
//        if (context.performed)
//        {
//            RunJumpInput = true;
//            JumpInputStop = false;
//            runJumpInputStartTime = Time.time;
//        }
//        else if (context.canceled)
//        {
//            JumpInputStop = true;
//        }
//    }

//    public void OnSprintInput(InputAction.CallbackContext context)
//    {
//        if (context.performed)
//        {
//            SprintInput = true;
//        }
//        else if (context.canceled)
//        {
//            SprintInput = false;
//        }
//    }

//    // 蹲下改為按住觸發 (符合大多數遊戲設計)
//    public void OnCrouchInput(InputAction.CallbackContext context)
//    {
//        if (context.performed)
//        {
//            IsCrouchInput = true;
//        }
//        else if (context.canceled)
//        {
//            IsCrouchInput = false;
//        }
//    }

//    public void OnInteractInput(InputAction.CallbackContext context)
//    {
//        if (context.performed)
//        {
//            InteractInput = true;
//        }
//    }

//    public void OnEquipmentInput(InputAction.CallbackContext context)
//    {
//        if (context.performed)
//        {
//            // 切換裝備激活狀態
//            EquipmentActive = !EquipmentActive;

//            // 裝備特殊效果
//            if (EquipmentActive)
//            {
//                ActivateEquipment();
//            }
//            else
//            {
//                DeactivateEquipment();
//            }
//        }
//    }

//    private void ActivateEquipment()
//    {
//        switch (CurrentEquipment)
//        {
//            case EquipmentType.Armor:
//                switch (player.EquippedArmor)
//                {
//                    case Armor.Helmet:
//                        // Apply helmet effect
//                        break;
//                    case Armor.Chestplate:
//                        // Apply chestplate effect
//                        break;
//                    // ... other cases ...
//                    default:
//                        break;
//                }
//                break;
//                // ... other equipment types ...
//        }
//    }

//    private void DeactivateEquipment()
//    {
//        // 裝備停用邏輯
//    }

//    public void OnLockOnInput(InputAction.CallbackContext context)
//    {
//        if (context.performed)
//        {
//            LockOnInput = true;
//            ToggleLockOn();
//        }
//    }

//    private void ToggleLockOn()
//    {
//        if (LockedTarget == null)
//        {
//            // 尋找最近的敵人
//            LockedTarget = FindNearestTarget();

//            if (LockedTarget != null)
//            {
//                // 啟用鎖定視角
//                //CameraController.Instance.ZoomOut(2f);
//                //CameraController.Instance.FollowTarget(LockedTarget.transform, 0.3f);
//            }
//        }
//        else
//        {
//            // 解鎖
//            LockedTarget = null;
//            //CameraController.Instance.ZoomIn();
//            //CameraController.Instance.ResetFollow();
//        }
//    }

//    private GameObject FindNearestTarget()
//    {
//        // 實際遊戲中應該使用ObjectPool或敵人管理器
//        GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
//        GameObject nearest = null;
//        float minDistance = Mathf.Infinity;

//        foreach (var target in targets)
//        {
//            float distance = Vector2.Distance(transform.position, target.transform.position);
//            if (distance < minDistance)
//            {
//                minDistance = distance;
//                nearest = target;
//            }
//        }
//        return nearest;
//    }

//    // 衝刺狀態中調整方向
//    public Vector2 GetDashDirection()
//    {
//        Vector2 direction = Vector2.right * NormInputX;

//        // 垂直微調 (衝刺中按上/下)
//        if (NormInputY != 0)
//        {
//            direction.y = NormInputY * 0.3f; // 小幅度調整
//        }

//        // 鎖定系統方向覆蓋
//        if (LockedTarget != null)
//        {
//            direction = LockDirection;
//        }

//        return direction.normalized;
//    }

//    // 輸入重置方法
//    public void ResetDashInput() => DashInput = false;
//    public void ResetDashBackInput() => DashBackInput = false;
//    public void ResetInteractInput() => InteractInput = false;
//    public void ResetLockOnInput() => LockOnInput = false;
//    public void UseRunJumpInput() => RunJumpInput = false;
//    public void UseSprintJumpInput() => SprintJumpInput = false;
//    public void UseClimbLedgeDownInput() => IsClimbLedgeDown = false;
//    public void UseClimbLedgeUpInput() => IsClimbLedgeUp = false;
//    public void CancelAllJumpInput()
//    {
//        UseRunJumpInput();
//        UseSprintJumpInput();
//    }

//    private void CheckInputHoldTimes()
//    {
//        if (Time.time >= runJumpInputStartTime + runJumpInputHoldTime)
//        {
//            RunJumpInput = false;
//        }

//        if (Time.time >= sprintJumpInputStartTime + sprintJumpInputHoldTime)
//        {
//            SprintJumpInput = false;
//        }
//    }

//    // 按鍵重綁系統
//    public void RebindAction(string actionName, InputAction newAction)
//    {
//        var binding = customBindings.Find(b => b.actionName == actionName);
//        if (binding != null)
//        {
//            binding.actionReference.action.ApplyBindingOverride(newAction.bindings[0]);
//            SaveCustomBindings();
//        }
//    }

//    private void SaveCustomBindings()
//    {
//        foreach (var binding in customBindings)
//        {
//            string key = $"Binding_{binding.actionName}";
//            PlayerPrefs.SetString(key, binding.actionReference.action.bindings[0].overridePath);
//        }
//    }

//    private void LoadCustomBindings()
//    {
//        foreach (var binding in customBindings)
//        {
//            string key = $"Binding_{binding.actionName}";
//            if (PlayerPrefs.HasKey(key))
//            {
//                string savedBinding = PlayerPrefs.GetString(key);
//                binding.actionReference.action.ApplyBindingOverride(savedBinding);
//            }
//            else
//            {
//                binding.actionReference.action.ApplyBindingOverride(binding.defaultBindingPath);
//            }
//        }
//    }
//}