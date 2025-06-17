using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unit;

public class MonsterFactory : MonoBehaviour
{
  [ShowInInspector]
  public List<MonsterResource> monsterResources;
  System.Random rand = new ();
  [SerializeField] [Required(InfoMessageType.Warning)]
  public BaseDamagable AttackTarget;
  [SerializeField]
  public int SpawnCountAtOnce = 20;
  [SerializeField]
  int numberOfWayPoints;
  [SerializeField]
  int numberOfMonstersInPool;
  Dictionary<GameObject , MonoBehaviourPool<FieldUnit>> monsterPools;
  [ShowInInspector]
  public (float min, float max) EnemySpawnRange;
  WaitForSeconds SpawnDelay = new WaitForSeconds(0.5f);
  Coroutine SpawnRoutine;
  float GetRandomPercentage() => (float)this.rand.Next(0, 100) / 100f;

  void Awake()
  {
    this.monsterPools = new ();
    foreach (var monsterResource in this.monsterResources) {
      if (!this.monsterPools.ContainsKey(monsterResource.Prefab)) {
        this.monsterPools.Add(monsterResource.Prefab, this.CreatePool(monsterResource));
      }
    }
  }

  public void StartSpawn()
  {
    if (this.SpawnRoutine != null) {
      this.StopCoroutine(this.SpawnRoutine);
    }
    this.SpawnRoutine = this.StartCoroutine(
      this.CreateSpawnRoutine()
      );
  }

  IEnumerator CreateSpawnRoutine()
  {
    int count = 0;
    while (count < 20) {
      var resource = this.PickRandomMonster();
      this.SpawnMonster(resource, this.GetRandomPosition(this.AttackTarget.Position));
      count += 1;
      yield return (this.SpawnDelay);
    } 
    this.SpawnRoutine = null;
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
    var pool = this.monsterPools[monsterResource.Prefab];
    var monster = pool.Get();
    monster.Stat = monsterResource.MonsterStat;
    monster.DefaultState = monsterResource.DefaultState;
    monster.ChaseTarget = this.AttackTarget;
    monster.transform.position = position;
    monster.Reset();
    monster.Activate();
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
      this.EnemySpawnRange.min, this.EnemySpawnRange.max);
    var y = UnityEngine.Random.Range(0f, 0.5f);
    var z = UnityEngine.Random.Range(
      this.EnemySpawnRange.min, this.EnemySpawnRange.max);
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
