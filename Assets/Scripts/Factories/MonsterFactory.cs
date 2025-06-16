using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Architecture;
using Unit;

public class MonsterFactory : MonoBehaviour
{
  [ShowInInspector]
  public List<MonsterResource> monsterResources;
  System.Random rand = new ();
  [SerializeField] [Required(InfoMessageType.Warning)]
  public BaseDamagable AttackTarget;
  [SerializeField]
  int numberOfWayPoints;
  [SerializeField]
  int numberOfMonstersInPool;
  Dictionary<MonsterResource, MonoBehaviourPool<FieldUnit>> monsterPools;
  [ShowInInspector]
  (float min, float max) enemySpawnRange;
  float GetRandomPercentage() => (float)this.rand.Next(0, 100) / 100f;

  void Awake()
  {
    this.monsterPools = new ();
    foreach (var monsterResource in this.monsterResources) {
      this.monsterPools.Add(monsterResource, this.CreatePool(monsterResource));
    }
  }

  MonoBehaviourPool<FieldUnit> CreatePool(MonsterResource resource)
  {
    return new MonoBehaviourPool<FieldUnit>(
      poolSize: this.numberOfMonstersInPool,
      prefab: resource.Prefab,
      initPooledObject: (monster) => this.InitMonster(monster, resource));
  }

  void InitMonster(FieldUnit unit, MonsterResource monsterResource) {
    unit.Stat = monsterResource.MonsterStat;
    unit.DefaultState = monsterResource.DefaultState;
    unit.ChaseTarget = this.AttackTarget;
    unit.Init();
    this.SetWayPoints(unit);
  }

  [Button ("Spawn monster")]
  void SpawnMonster(MonsterResource monsterResource, Vector3 position)
  {
    var pool = this.monsterPools[monsterResource];
    var monster = pool.Get();
    monster.Reset();
    monster.transform.position = position;
    monster.Activate();
  }

  FieldUnit CreateMonster(MonsterResource monsterResource)
  {
    var gameObject = Instantiate(monsterResource.Prefab);
    var unit = gameObject.GetComponent<FieldUnit>();
    unit.Stat = monsterResource.MonsterStat;
    unit.DefaultState = monsterResource.DefaultState;
    unit.ChaseTarget = this.AttackTarget;
    unit.Init();
    this.SetWayPoints(unit);
    return (unit);
  }

  void SetWayPoints(FieldUnit unit)
  {
    for (int i = 0; i < this.numberOfWayPoints - 1; i++) {
      unit.AddWayPoint(this.GetRandomPosition(this.AttackTarget.transform.position)); 
    }
    unit.WayPoints.Add(this.AttackTarget.transform.position);
  }

  MonsterResource PickRandomMonster()
  {
    var percentage = this.GetRandomPercentage();
    foreach (var monster in this.monsterResources) {
      if (percentage < monster.SpawnChance) {
        return (monster);
      }
    }
    return (this.monsterResources[0]);
  }

  Vector3 GetRandomPosition(Vector3 center)
  {
    var x = UnityEngine.Random.Range(
      this.enemySpawnRange.min, this.enemySpawnRange.max);
    var y = UnityEngine.Random.Range(0f, 0.5f);
    var z = UnityEngine.Random.Range(
      this.enemySpawnRange.min, this.enemySpawnRange.max);
    return (new Vector3(
      this.GetRandomSignedValue(x),
      this.GetRandomSignedValue(y), 
      this.GetRandomSignedValue(z)));
  }

  float GetRandomSignedValue(float value) {
    if (this.rand.Next(0, 2) == 0) {
      return (-value);
    }
    return (value);
  }
}
