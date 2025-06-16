using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Architecture;

public class UserInputManager : SingletonBehaviour<UserInputManager>
{
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  public static void BeforeSceneLoad()  
  {
    UserInputManager.CreateInstance(); 
  }

  public abstract class OperationBase {
    public abstract void Update();
  }

  public class Operation<T>: OperationBase where T: struct
  {
    InputAction Action;
    public bool IsNeedUpdate;
    public T inputValue { get; private set; }
    public Action OnTriggered;
    public bool HasRegistered { get; private set; }

    public override void Update()
    {
      this.inputValue = this.Action.ReadValue<T>();
      this.HasRegistered  = this.Action.IsPressed();
    }

    public T Pull()
    {
      this.inputValue = this.Action.ReadValue<T>();
      return (this.inputValue);
    }

    public void OnPerformed(InputAction.CallbackContext context)
    {
      if (!this.HasRegistered && this.OnTriggered != null) {
        this.OnTriggered.Invoke(); 
      }
    }

    public Operation(string action, bool isNeedUpdate)
    {
      this.Action = InputSystem.actions.FindAction(action);
      this.Action.performed += this.OnPerformed;
      this.IsNeedUpdate = isNeedUpdate;
      this.HasRegistered = false;      
    }
  }

  public ObservableValue<Nullable<Vector2>> PrimarySelectedScreenPosition { get; private set; }
  public Vector2 PointerDelta { get; private set; }
  public Operation<float> MainInteract { get; private set; }
  public Operation<Vector2> CursorPosition { get; private set; }
  public List<OperationBase> OperationsNeedUpdate;
  public Action<Vector2> OnCursorPressed;

  protected override void Awake()
  {
    base.Awake();
    this.OperationsNeedUpdate = new ();
    this.MainInteract = new Operation<float>(
      action: "MainInteract",
      isNeedUpdate: true);
    this.CursorPosition = new Operation<Vector2>(
      action: "CursorPosition",
      isNeedUpdate: false
      );
    this.OperationsNeedUpdate.Add(this.MainInteract);
    this.PrimarySelectedScreenPosition = new (null);
  }

  void OnEnable()
  {
    this.MainInteract.OnTriggered += this.OnMainInteract;
  }

  void Update()
  {
    foreach (var operation in this.OperationsNeedUpdate) {
      operation.Update();
    }
  }

  void OnMainInteract()
  {
    if (this.OnCursorPressed != null) {
      var position = this.CursorPosition.Pull();
      this.OnCursorPressed.Invoke(position);
    }
    
  }

  void OnDisable()
  {
    this.MainInteract.OnTriggered -= this.OnMainInteract;
  }

  void OnDestory()
  {
    this.OnDestroyed();
  }
}

