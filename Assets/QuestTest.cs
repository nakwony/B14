using UnityEngine;

public class QuestTest : SingletonDestroyable<QuestTest>
{
    [SerializeField]
    private Quest _quest;

    [SerializeField]
    private Category _category;

    [SerializeField]
    private TaskTarget _target;

    private void Start()
    {
        var questSystem = QuestSystem.Instance;

        questSystem.onQuestRegistered += quest =>
        {
            print($"New Quest:{quest.CodeName} Registered");
            print($"Active Quests Count:{questSystem.ActiveQuests.Count}");
        };

        questSystem.onQuestCompleted += quest =>
        {
            print($"Quest:{quest.CodeName} Completed");
            print($"Completed Quests Count:{questSystem.CompletedQuests.Count}");
        };

        var newQuest = questSystem.Register(_quest);
        newQuest.onTaskSuccessChanged += (quest, task, currentSuccess, prevSuccess) =>
        {
            print($"Quest:{quest.CodeName}, Task:{task.CodeName}, CurrentSuccess:{currentSuccess}");
        };
    }

    public void CountOneQuestSuccess()
    {
        QuestSystem.Instance.ReceiveReport(_category, _target, 1);
    }
}