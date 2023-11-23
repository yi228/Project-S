using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SingleGameManager : MonoBehaviour
{
    public static SingleGameManager instance;
    private SingleMyPlayerController _player;

    public SingleMyPlayerController Player { get { return _player; } set { _player = value; } }
    [SerializeField] private GameObject[] boss;
    private bool _bossWarned;
    public bool bossSpawned;
    public bool bossKilled;
    private bool _stageClear;
    public static bool isGameOver = false;
    public Transform[] spawnPos;

    public GameObject monHpBarParent;

    //임시 몬스터 수(보스 제외)
    private int[] _monsterNum = { 10, 10, 15, 15 };

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        InitGame();
    }
    void Update()
    {
        if (CheckNoMonster() && !_bossWarned)
            BossSpawnReady();
    }
    public void InitGame()
    {
        _player.UseTeleport = false;
        bossSpawned = false;
        _player.KillCount = 0;
        bossKilled = false;
        _stageClear = false;
        _bossWarned = false;
    }
    private bool CheckNoMonster()
    {
        int _killCnt = _player.KillCount;

        if (_killCnt >= _monsterNum[Stage.currentStage])
            return true;
        else
            return false;
    }
    private void BossSpawnReady()
    {
        _bossWarned = true;
        GameObject _warning = Managers.Resource.Instantiate("SinglePlay/UI/UI_Warning");
        if (Managers.Sound.SoundOn == true)
            Managers.Sound.Play("Effect/BossWarning");
        Invoke("BossSpawn", 2f);
        Destroy(_warning, 2.1f);
    }
    private void BossSpawn()
    {
        bossSpawned = true;
        Debug.Log("보스 스폰");
        boss[Stage.currentStage].SetActive(true);
    }
    public void StageClear()
    {
        if (!_stageClear)
        {
            _stageClear = true;
            Managers.Resource.Instantiate("SinglePlay/UI/UI_StageClearPopUp");
        }
    }
    public void GameOver()
    {
        if (isGameOver == false)
        {
            boss[Stage.currentStage].SetActive(false);
            Managers.Resource.Instantiate("SinglePlay/UI/UI_GameOverPopUp");
            isGameOver = true;
        }
    }
}
