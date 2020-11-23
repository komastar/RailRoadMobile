using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandObject : MonoBehaviour, IGameActor
{
    public int Id { get; set; }

    public StageModel stage;
    public DiceObject dicePrefab;

    public DiceObject Dice;

    private List<DiceObject> dices;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 50, 50), "Roll"))
        {
            Roll();
        }
    }

    public void Init(int id = 0)
    {
        dices = new List<DiceObject>();
    }

    private void MakeDices()
    {
        Debug.Log($"Make Dices : {stage.Dice.Length}");
        for (int i = 0; i < stage.Dice.Length; i++)
        {
            var dice = Instantiate(dicePrefab, transform);
            dice.Init(stage.Dice[i]);
            dice.onClick += OnClickDice;
            dices.Add(dice);
        }
    }

    private void ClearDices()
    {
        for (int i = 0; i < dices.Count; i++)
        {
            DestroyImmediate(dices[i].gameObject);
        }
        dices.Clear();
    }

    public void Roll()
    {
        ClearDices();
        MakeDices();
        for (int i = 0; i < dices.Count; i++)
        {
            dices[i].Roll();
        }
    }

    public void OnClickDice(DiceObject dice)
    {
        Dice = dice;
    }

    public void DisposeNode()
    {
        dices.Remove(Dice);
        Destroy(Dice.gameObject);
        Dice = null;
    }

    public int GetDiceCount()
    {
        return dices.Count;
    }

    public int GetDice()
    {
        return ReferenceEquals(null, Dice) ? -1 : Dice.DiceId;
    }
}