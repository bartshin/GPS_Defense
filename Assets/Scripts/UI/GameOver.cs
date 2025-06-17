using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
  float RemainTime;

  void Start()
  {
    this.RemainTime = 3f;
  }

  // Update is called once per frame
  void Update()
  {
    this.RemainTime -= Time.deltaTime;
    if (this.RemainTime < 0) {
      GameManager.Shared.Restart();
    }
  }
}
