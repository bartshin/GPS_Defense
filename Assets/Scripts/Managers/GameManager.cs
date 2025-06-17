using System;
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
  public MainBuilding MainBuilding;
  public MainSceneUI MainSceneUI;
  public ObservableValue<int> Gold { get; set; }
  public Vector2 JoysticValue => this.MainSceneUI != null ? this.MainSceneUI.JoystickInput: Vector2.zero;

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
    this.Gold = new (500);
    this.State = new (GameState.BeforeStart);
    this.SelectedMapPath = Application.dataPath + "/Data/map/gyungil.xml";
    this.SelectedMetadataPath = Application.dataPath + "/Data/meta/gyungil.xml";
  }

  public void GameOver()
  {
    SceneManager.LoadScene("GameOverScene");
  }

  public void Restart()
  {
    this.MainSceneUI = null;
    this.State.Value = GameState.BeforeStart;
    this.Gold.Value = 500;
    SceneManager.LoadScene("TitleScene");
  }

  public void OnClickStart()
  {
    if (this.State.Value == GameState.BeforeStart) {
      this.loadingUI.Show(); 
      this.State.Value = GameState.Loading;
      SceneManager.LoadScene("MainScene");
    }
  }

  public void StartGame()
  {
    var factories = GameObject.FindGameObjectsWithTag("Factory");
    foreach (var factory in factories) {
      var monsterFactory = factory.GetComponent<MonsterFactory>();
      if (monsterFactory != null) {
        monsterFactory.AttackTarget = this.MainBuilding.GetComponent<BaseDamagable>();
        monsterFactory.EnemySpawnRange = (20, 50);
        monsterFactory.StartSpawn();
        continue;
      }
      var towerFactory = factory.GetComponent<TowerFactory>();
      if (towerFactory != null) {
        this.MainSceneUI.SelectedTower.OnChanged += towerFactory.OnSelectTower;
      }
    }
    CameraManager.Shared.Focus(this.MainBuilding.transform);
  }

  public void OnFinishLoading()
  {
    this.State.Value = GameState.BeforeStart;
    this.loadingUI.Hide();
    AudioManager.Shared.ChangeBgm();
  }
}
