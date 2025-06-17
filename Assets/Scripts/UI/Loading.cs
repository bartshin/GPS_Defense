using UnityEngine;
using UnityEngine.UIElements;

public class LoadingUI: MonoBehaviour
{
  VisualElement root;
  [SerializeField]
  GameObject Spinner; 

  void Awake()
  {
    this.root = this.GetComponent<UIDocument>().rootVisualElement;
    this.root.name = "loading-ui-container";
    var label = new Label();
    label.text = "로딩중";
    label.name = "loading-label";
    this.root.Add(label);
    this.Hide();
  }

  public void Show() {      
    this.root.visible = true;
    this.Spinner.SetActive(true);
    this.root.BringToFront();
  }  

  public void Hide() {
    this.root.visible = false;
    this.Spinner.SetActive(false);
    this.root.SendToBack();   
  }    
}
