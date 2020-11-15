using UnityEngine;

public class HandObject : MonoBehaviour
{
    private DataManager dataManager;
    private DiceObject[] dices;

    public  StageModel stage;

    public void Init()
    {
        dataManager = DataManager.Get();
        dices = FindObjectsOfType<DiceObject>();
    }

    public void Roll()
    {
        for (int i = 0; i < dices.Length; i++)
        {
            dices[i].Roll();
        }
    }
}