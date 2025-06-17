using System;
using UnityEngine;
using Architecture;
using Sirenix.OdinInspector;

public class AudioManager : SingletonBehaviour<AudioManager>
{
  [SerializeField] [ShowInInspector]
  [InlineEditor(InlineEditorModes.SmallPreview)]
  AudioClip[] bgmList;
  [SerializeField]
  [ValueDropdown("bgmList")]
  [InlineButton("PlayBgm")]
  AudioClip currentBgm => this.bgmList[this.currentBgmIndex];
  int currentBgmIndex = 0;
  [ShowInInspector]
  float BgmVolume 
  { 
    get => this.bgmVolume;
    set {
      this.bgmVolume = value;
      this.SetBgmVolume(value);
    }
  }
  float bgmVolume = 0.3f;
  float sfxVolume = 0.5f;
  SfxController bgmCongtroller;

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  new public static void CreateInstance()  
  {
    GameObject prefab = Resources.Load<GameObject>("AudioManager");
    var gameObject = Instantiate(prefab);
    DontDestroyOnLoad(gameObject);
  }

  public void ChangeBgm()
  {
    this.currentBgmIndex = (this.currentBgmIndex + 1) % this.bgmList.Length;
    this.PlayBgm();
  }

  const int DEFAULT_SFX_POOL_SIZE = 30;
  [SerializeField]
  GameObject sfxControllerPrefab;
  ObjectPool<SfxController> sfxPool;

  public SfxController GetSfxController()
  {
    return (this.sfxPool.Get());
  }

  public void PlaySfx(AudioClip clip, Nullable<Vector3> position = null, Nullable<float> volume = null)
  {
    var sfx = this.GetSfxController();
    sfx.SetVolume(volume ?? this.sfxVolume);
    sfx.transform.position = position ?? Camera.main.transform.position;
    sfx.PlaySound(clip);
  }

  protected override void Awake()
  {
    base.Awake();
    this.sfxPool = new MonoBehaviourPool<SfxController>(
        poolSize: DEFAULT_SFX_POOL_SIZE,
        prefab: this.sfxControllerPrefab);
    this.bgmCongtroller = this.InitBgmController(); 
    this.PlayBgm();
  }

  SfxController InitBgmController()
  {
    var sfx = this.GetSfxController();
    sfx.SetLoop(true);
    sfx.SetVolume(this.bgmVolume);
    sfx.transform.SetParent(this.transform);
    return (sfx);
  }

  void PlayBgm()
  {
    this.bgmCongtroller.Stop();
    this.bgmCongtroller.PlaySound(this.currentBgm);
  }

  void SetBgmVolume(float volume)
  {
    this.bgmCongtroller.SetVolume(volume);
  }
}

