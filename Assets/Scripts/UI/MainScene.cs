using System;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Architecture;
using Unit;

public class MainSceneUI: MonoBehaviour {

  public const string Tag = "UI";
  public event System.Action OnFireButtonPressed;
  public event System.Action OnFireButtonReleased;
  public Vector2 JoystickInput => joystick.Input;
  public GreetingUI greetingUI;
  public ObservableValue<TowerResource> SelectedTower;
  TowerResource RotateTower;
  (Button button, TowerResource tower) CurrentSelectedTower
  {
    get => this.currentSelectedTower;
    set => this.OnChangeSelectedTower(value.button, value.tower);
  }
  (Button button, TowerResource) currentSelectedTower;

  VisualElement root;
  Label goldLabel; 
  Joystick joystick;
  PlayerHpView hpView;

  void Awake() {
    this.RotateTower = Resources.Load<TowerResource>("RotateTowerResourceLv1");
    this.root = this.GetComponent<UIDocument>().rootVisualElement;
    this.root.AddToClassList("container");
    this.greetingUI = new GreetingUI();
    this.root.Add(this.greetingUI);
    this.hpView = new PlayerHpView();
    this.root.Add(this.hpView);
    this.greetingUI.Hide();
    this.greetingUI.OnDismiss += this.OnGreetingDismiss;
    this.SelectedTower = new (null);
    this.CurrentSelectedTower = (null, null);
    this.CreateUI();
    this.Hide();
  }

  public void OnGreetingDismiss()
  {
    GameManager.Shared.StartGame();
    this.Show();
  }

  public void Show() {      
    this.root.visible = true;
    this.root.BringToFront();
    if (GameManager.Shared.MainBuilding != null) {
      var playerHp = GameManager.Shared.MainBuilding.Damagable.Hp;
      playerHp.OnChanged += this.OnHpChanged; 
      this.OnHpChanged(playerHp.Value);
    }
  }  

  public void OnHpChanged((int current, int max) hp)
  {
    this.hpView.SetValue(this.hpView.HpBarHandle, (float)hp.current / (float)hp.max);
  }

  public void Hide() {
    this.root.visible = false;
    this.root.SendToBack();   
  }    

  public void OnGoldChanged(int gold)
  {
    this.goldLabel.text = $"Gold: {gold}";
  }

  void OnEnable() {
    GameManager.Shared.MainSceneUI = this; 
    GameManager.Shared.Gold.OnChanged += this.OnGoldChanged;
  }

  void OnDisable() {
    GameManager.Shared.MainSceneUI = null;
    GameManager.Shared.Gold.OnChanged -= this.OnGoldChanged;
  }

  // Start is called before the first frame update
  void Start() {
    this.greetingUI.Show();
  }

  void OnChangeSelectedTower(Button button, TowerResource tower) {
    if (button != null) {
      if (this.currentSelectedTower.button == button) {
        this.ClearSelectedTower();
      }
      else {
        this.SetSelectedTower(button, tower);
      }
    }
    else {
      this.ClearSelectedTower();
    }

  }

  void ClearSelectedTower()
  {
    if (this.currentSelectedTower.button != null) {
      this.currentSelectedTower.button.RemoveFromClassList("tower-button-active");
    }
    this.currentSelectedTower = (null, null);
    if (this.SelectedTower != null) {
      this.SelectedTower.Value = null;
    }
  }

  void SetSelectedTower(Button button, TowerResource tower)
  {
    this.currentSelectedTower = (button, tower);
    this.currentSelectedTower.button.AddToClassList("tower-button-active");
    this.SelectedTower.Value = tower;
  }


  void CreateUI() {
    this.CreateTowerButtons();
    this.CreateScoreText();
    this.CreateJoystick();
    this.CreateZoomButton();
  }

  private void CreateZoomButton()
  {
    var container = new VisualElement();
    container.name = "zoom-container";
    var zoomInButton = new Button();
    zoomInButton.AddToClassList("zoom-button");
    zoomInButton.text = "+";
    zoomInButton.RegisterCallback<ClickEvent>(e => {
      CameraManager.Shared.Zoom(20f);
      });
    var zoomOutButton = new Button();
    zoomOutButton.text = "-";
    zoomOutButton.AddToClassList("zoom-button");
    zoomOutButton.RegisterCallback<ClickEvent>(e => {
      CameraManager.Shared.Zoom(-20f);
      });
    container.Add(zoomInButton);
    container.Add(zoomOutButton);
    this.root.Add(container);
  }

  void CreateTowerButtons() {
    Button rotateTowerButton = new Button();
    rotateTowerButton.AddToClassList("tower-button");
    rotateTowerButton.name = "rotate-tower-button";
    rotateTowerButton.RegisterCallback<ClickEvent>(evt => {
      this.CurrentSelectedTower = (rotateTowerButton, this.RotateTower);
      });
    this.root.Add(rotateTowerButton);
  }

  void CreateScoreText() {
    this.goldLabel = new Label();
    this.goldLabel.name = "gold-label";
    this.goldLabel.text = $"Gold: {GameManager.Shared.Gold.Value}";
    this.root.Add(goldLabel);
  }

  void CreateJoystick() {
    this.joystick = new Joystick();
    this.root.Add(this.joystick);
  }
}
