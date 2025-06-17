using UnityEngine;
using UnityEngine.UIElements;

public class TitleSceneUI : MonoBehaviour{

  VisualElement root;

  void Awake() {
    this.root = this.GetComponent<UIDocument>().rootVisualElement;
    this.root.name = "title-scene-container";
    root.style.height = Length.Percent(100);
    root.style.width = Length.Percent(100);
  }

  void Start() {
    this.CreateUI();
  }

  public void CreateUI() {
    var titleLabel = new Label();
    titleLabel.text = "저세상 디펜스";
    titleLabel.name = "titleLabel";
    this.root.Add(titleLabel);

    Button startButton = new Button();
    startButton.name = "startButton";
    startButton.RegisterCallback<ClickEvent>(this.OnClickStartButton);
    Label startButtonLabel = new Label("Start Game");
    startButton.Add(startButtonLabel);
    this.root.Add(startButton);
  }

  void OnClickStartButton(ClickEvent e) {
    GameManager.Shared.OnClickStart();
  }
}
