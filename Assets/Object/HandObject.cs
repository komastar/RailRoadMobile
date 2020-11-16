using System.Collections.Generic;
using UnityEngine;

public class HandObject : MonoBehaviour, IGameActor
{
    public int Id { get; set; }

    public StageModel stage;
    public DiceObject dicePrefab;

    public DiceObject Dice;

    private List<DiceObject> dices;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Roll"))
        {
            Roll();
        }
    }

    public void Init(int id = 0)
    {
        dices = new List<DiceObject>();
        for (int i = 0; i < stage.Dice.Length; i++)
        {
            var dice = Instantiate(dicePrefab, transform);
            dice.Init(stage.Dice[i]);
            dice.onClick += OnClickDice;
            dices.Add(dice);
        }
    }

    public void Roll()
    {
        for (int i = 0; i < dices.Count; i++)
        {
            dices[i].Roll();
        }
    }

    public void OnClickDice(DiceObject dice)
    {
        Dice = dice;
    }
}