using System;
using UnityEngine.UIElements;

public class GreetingUI : VisualElement
{
  public Action OnDismiss;
  public GreetingUI()
  {
    this.name = "greeting-ui-container"; 
    this.CreateUI();
  }

  void CreateUI()
  {
    var title = new Label();
    title.AddToClassList("title");
    title.text = "환영합니다!\n 여러분이 알던 세계와 같은 장소 같지만 그렇지 않습니다";
    var description = new Label();
    description.text = "잠시 후면 여러 악마들이 당신을 향해 공격해 올 것입니다\n다행히 우리에겐 믿음직한 방어 건물을 지을 수 있습니다\n적들을 막아내서 이 세계에서 생존해 보세요!";
    description.AddToClassList("description");
    this.Add(title);
    this.Add(description);

    var dissmissButton = new Button();
    dissmissButton.name = "dismiss-button";
    var buttonLabel = new Label();
    buttonLabel.text = "확인";
    buttonLabel.name = "dismiss-button-label";
    dissmissButton.Add(buttonLabel);
    dissmissButton.RegisterCallback<ClickEvent>(this.OnClickConfirm);
    this.Add(dissmissButton);
  }

  void OnClickConfirm(ClickEvent click) {
    if (GameManager.Shared.State.Value == GameManager.GameState.Loading) {
      return ;
    }
    this.Hide();
    if (this.OnDismiss != null) {
      this.OnDismiss.Invoke();
    }
  }

  public void Show() {      
    this.visible = true;
    this.BringToFront();
  }  

  public void Hide() {
    this.visible = false;
    this.SendToBack();   
  }    
}
