using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHpView : VisualElement
{
  static readonly Color HP_COLOR = new Color(234f/255f, 45f/255f, 20f/255f);
  /****************** USS Selector ***************************/
  const string CONTAINER = "player-hp-view-container";
  const string BAR_CONTAINER = "bar-container";
  const string BAR_LABEL = "bar-label";
  const string BAR = "bar";
  const string BAR_BACKGROUND = "bar-background";
  const string BAR_FILL = "bar-fill";
  const string BAR_MASK = "bar-mask";
  /******************** Constants ***************************/

  public VisualElement HpBarHandle { get; private set; } 

  public PlayerHpView()
  {
    this.name = PlayerHpView.CONTAINER;
    this.CreateUI();
  }

  void CreateUI()
  {
    VisualElement hpBar = this.CreateBar(
      labelText: "HP",
      out VisualElement handle,
      color: HP_COLOR
      );
    this.HpBarHandle = handle;
    this.Add(hpBar);
  }

  public void SetHiddenTo(VisualElement element, bool hidden)
  {
    if (hidden) {
      element.AddToClassList("hidden");
    }
    else {
      element.RemoveFromClassList("hidden");
    }
  }

  public void SetValue(VisualElement handle, float percentage)
  {
    handle.style.translate = new StyleTranslate(
      new Translate(new Length(percentage * 100f, LengthUnit.Percent),
        new Length())
    );
  }

  VisualElement CreateBar(in string labelText, out VisualElement handle, Color color )
  {
    var container = new VisualElement();
    container.AddToClassList(PlayerHpView.BAR_CONTAINER);

    if (labelText != null) {
      var label = new Label(labelText);
      label.AddToClassList(PlayerHpView.BAR_LABEL);
      label.style.color = color;
      container.Add(label);
    }

    var hpBar = new VisualElement();
    hpBar.AddToClassList(PlayerHpView.BAR);
    this.SetBorderColor(hpBar, color);
    container.Add(hpBar);

    var background = new VisualElement();
    background.AddToClassList(PlayerHpView.BAR_BACKGROUND);
    hpBar.Add(background);

    var fill = new VisualElement();
    fill.AddToClassList(PlayerHpView.BAR_FILL);
    fill.style.unityBackgroundImageTintColor = color;
    background.Add(fill);

    var mask = new VisualElement();
    mask.AddToClassList(PlayerHpView.BAR_MASK);
    mask.style.translate = new StyleTranslate(
      new Translate(new Length(100f, LengthUnit.Percent), new Length())
      );
    background.Add(mask);
    handle = mask;

    return (container);
  }

  void SetBorderColor(VisualElement element, Color color) 
  {
    element.style.borderTopColor = color;
    element.style.borderBottomColor = color;
    element.style.borderLeftColor = color;
    element.style.borderRightColor = color;
  }
}
