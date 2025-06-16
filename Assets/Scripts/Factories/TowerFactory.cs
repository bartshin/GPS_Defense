using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unit;

public class TowerFactory : MonoBehaviour
{
  [SerializeField] 
  LayerMask floorLayer;
  int cameraRaycastLayer;
  [ShowInInspector]
  public List<TowerResource> TowerResources;
  public int SelectedPowerIndex;

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
    var hitPosition = this.CameraRaycast(position);
    if (hitPosition != null) {
      var tower = this.CreateTower(
        this.TowerResources[0],
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
      position.y + resource.TowerHeight * 0.5f,
      position.z
      );
    var tower = gameObject.AddComponent<Tower>();
    tower.ProjectileData = resource.Projectile;
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
