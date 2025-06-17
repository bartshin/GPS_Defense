using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unit;
using Architecture;

public class TowerFactory : MonoBehaviour
{
  [SerializeField] 
  LayerMask floorLayer;
  int cameraRaycastLayer;
  [ShowInInspector]
  public List<TowerResource> TowerResources;
  public TowerResource towerTobuild;
  [ShowInInspector]
  int towerPrice = 30;

  public void OnSelectTower(TowerResource tower)
  {
    this.towerTobuild = tower;
  }

  void Awake()
  {
    this.cameraRaycastLayer = ~(1 << this.floorLayer.value);
  }

  void OnEnable()
  {
    UserInputManager.Shared.OnCursorPressed += this.OnCursorPressed;
  }

  void OnDisable()
  {
    UserInputManager.Shared.OnCursorPressed -= this.OnCursorPressed;
  }

  void OnCursorPressed(Vector2 position)
  {
    if (this.towerTobuild == null) {
      return;
    }
    var currentGold = GameManager.Shared.Gold.Value;
    if (GameManager.Shared.Gold.Value < this.towerPrice) {
      return;
    }
    GameManager.Shared.Gold.Value = currentGold - this.towerPrice;
    var hitPosition = this.CameraRaycast(position);
    if (hitPosition != null) {
      var tower = this.CreateTower(
        this.towerTobuild,
        hitPosition.Value
        );
      tower.Activate();
    }
  }

  Tower CreateTower(TowerResource resource, Vector3 position)
  {
    var gameObject = Instantiate(resource.Prefab);
    gameObject.transform.position = new Vector3(
      position.x,
      position.y,
      position.z
      );
    var tower = gameObject.AddComponent<Tower>();
    tower.ProjectileStat = resource.Projectile;
    tower.DefaultState = resource.DefaultState;
    tower.Stat = resource.TowerStat;
    tower.Init();
    return (tower);
  }

  Nullable<Vector3> CameraRaycast(Vector2 screenPos)
  {
    Ray ray = Camera.main.ScreenPointToRay(screenPos);
    if (Physics.Raycast(
      ray,
      maxDistance: 1000f,
      hitInfo: out RaycastHit hitInfo,
      layerMask: this.cameraRaycastLayer
      )) {
      return (hitInfo.point); 
    }
    return (null);
  }
}
