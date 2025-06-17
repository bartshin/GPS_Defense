using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainSceneUI: MonoBehaviour {

  public const string Tag = "UI";
  public event Action OnFireButtonPressed;
  public event Action OnFireButtonReleased;
  public Vector2 JoystickInput => joystick.Input;

  VisualElement root;
  Label goldLabel; 
  Joystick joystick;

  void Awake() {
    this.root = this.GetComponent<UIDocument>().rootVisualElement;
    this.root.AddToClassList("container");
  }

  void OnEnable() {
  }

  void OnDisable() {
  }

  // Start is called before the first frame update
  void Start() {
    this.CreateUI();
  }

  void CreateUI() {
    this.CreateFireButton();
    this.CreateScoreText();
    this.CreateJoystick();
  }

  void CreateFireButton() {
    Button fireButton = new Button();
    fireButton.AddToClassList("fireButton");
    fireButton.text = "FIRE";
    fireButton.RegisterCallback<PointerDownEvent>(evt => {
      if (this.OnFireButtonPressed != null)
        this.OnFireButtonPressed.Invoke();
      }, TrickleDown.TrickleDown);
    fireButton.RegisterCallback<PointerUpEvent>(evt => {
        if (this.OnFireButtonReleased != null)  
        this.OnFireButtonReleased.Invoke();
        });
    this.root.Add(fireButton);
  }

  void CreateScoreText() {
    this.goldLabel = new Label();
    this.goldLabel.name = "gold-label";
    this.root.Add(goldLabel);
  }

  void CreateJoystick() {
    this.joystick = new Joystick();
    this.root.Add(this.joystick);
  }

}
