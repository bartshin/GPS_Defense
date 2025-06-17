using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Architecture;
using Sirenix.OdinInspector;

public class GameManager : SingletonBehaviour<GameManager>
{
  public enum GameState 
  {
    BeforeStart,
    Loading,
    Playing
  }

  public ObservableValue<GameState> State { get; private set; }
  [SerializeField] [Required(InfoMessageType.Error)]
  LoadingUI loadingUI;
  public string SelectedMapPath { get; private set; }
  public string SelectedMetadataPath { get; private set; }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  new public static void CreateInstance()  
  {
    GameObject prefab = Resources.Load<GameObject>("GameManager");
    var gameObject = Instantiate(prefab);
    DontDestroyOnLoad(gameObject); 
  }

  protected override void Awake()
  {
    base.Awake();
    this.State = new (GameState.BeforeStart);
    this.SelectedMapPath = Application.dataPath + "/Data/map/gyungil.xml";
    this.SelectedMetadataPath = Application.dataPath + "/Data/meta/gyungil.xml";
  }

  public void GameOver()
  {
  }

  public void OnClickStart()
  {
    if (this.State.Value == GameState.BeforeStart) {
      this.State.Value = GameState.Loading;
      this.loadingUI.Show(); 
      SceneManager.LoadScene("MainScene");
    }
  }

  public void OnFinishLoading()
  {
    this.State.Value = GameState.Playing;
    this.loadingUI.Hide();
    AudioManager.Shared.ChangeBgm();
  }
}
