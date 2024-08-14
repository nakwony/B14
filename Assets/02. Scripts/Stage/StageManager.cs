using System;
using TMPro;
using UnityEngine;

public class StageManager : SingletonDestroyable<StageManager>
{
    public event Action<int, int> onStageChanged;

    [SerializeField]
    private TextMeshProUGUI _stageTmp;

    public StageDataSO StageDataSO;

    [SerializeField]
    private MonsterSpawner _monsterSpawner;
    
    protected override void Awake()
    {
        base.Awake();
        onStageChanged += UpdateStageDisplay;
    }
    
    public static void StageReset()
    {
        Instance.StageDataSO.StagePage = 0;
        Instance.ChangeStage(Instance.StageDataSO.Stage,
            Instance.StageDataSO.StagePage);
    }

    private void Start()
    {
        UpdateStageDisplay(Instance.StageDataSO.Stage, Instance.StageDataSO.StagePage);
        StartCoroutine(_monsterSpawner.CheckMonsters());
    }

    public void ChangeStage(int newStage, int newStagePage)
    {
        onStageChanged?.Invoke(newStage, newStagePage);
    }

    private void UpdateStageDisplay(int newStage, int newStagePage)
    {
        _stageTmp.text = $"스테이지 {newStage}-{newStagePage}";
    }
}