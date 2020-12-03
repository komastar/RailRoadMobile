using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandObject : MonoBehaviour, IGameActor
{
    public int Id { get; set; }

    public GameObject dicePanel;
    public StageModel stage;
    public DiceObject dicePrefab;

    public DiceObject Dice;

    public Action onChangeHand;

    public List<DiceObject> dices;

#if UNITY_EDITOR
    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 200, 150, 150), "Roll"))
    //    {
    //        Roll();
    //    }
    //}
#endif

    public void Init(int id = 0)
    {
        dices = new List<DiceObject>();
    }

    public void Ready()
    {
        MakeDices();
    }

    private void MakeDices()
    {
        ClearDices();
        Log.Debug($"Make Dices : {stage.Dice.Length}");
        for (int i = 0; i < stage.Dice.Length; i++)
        {
            var dice = Instantiate(dicePrefab, dicePanel.transform);
            dice.Init(stage.Dice[i]);
            dice.onClick += OnClickDice;
            dices.Add(dice);
        }
    }

    private void ClearDices()
    {
        for (int i = 0; i < dices.Count; i++)
        {
            Destroy(dices[i].gameObject);
        }
        dices.Clear();
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
        if (!ReferenceEquals(null, Dice))
        {
            Dice.OnDeselect();
        }
        Dice = dice;
        onChangeHand?.Invoke();
    }

    public void DisposeNode()
    {
        Dice.TurnOff();
        Dice = null;
    }

    public int GetDiceCount()
    {
        return dices.FindAll(d => d.diceButton.interactable == true).Count;
    }

    public int GetDice()
    {
        return ReferenceEquals(null, Dice) ? -1 : Dice.DiceId;
    }

    public void Cancel()
    {
        for (int i = 0; i < dices.Count; i++)
        {
            dices[i].TurnOn();
        }
    }

    public void Return(NodeObject node)
    {
        var find = dices.Find(n => n.DiceId == node.Id);
        find.TurnOn();
    }
}