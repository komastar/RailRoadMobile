using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandObject : MonoBehaviour, IGameActor
{
    public int Id { get; set; }

    public ContentSizeFitter sizeFitter;
    public GridLayoutGroup gridGroup;

    public GameObject dicePanel;
    public StageModel stage;
    public DiceObject dicePrefab;

    public DiceObject Dice;

    public Action onChangeHand;
    public Action<IActor> onClickObject;

    public List<DiceObject> dices;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Roll();
        }
    }

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
            dice.onClickObject += onClickObject;
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

    public void Roll(List<int> diceList)
    {
        for (int i = 0; i < dices.Count; i++)
        {
            dices[i].Roll(diceList[i]);
        }
    }

    public void OnClickDice(DiceObject dice)
    {
        Dice = dice;
        onChangeHand?.Invoke();
    }

    public void DisposeNode()
    {
        Dice.TurnOff();
    }

    public int GetDiceCount()
    {
        return dices.FindAll(d => d.diceButton.interactable == true).Count;
    }

    public DiceObject GetDice()
    {
        return Dice;
    }

    public void ResetHand()
    {
        if (!ReferenceEquals(null, Dice))
        {
            Dice = null;
        }
    }

    public void Cancel()
    {
        for (int i = 0; i < dices.Count; i++)
        {
            dices[i].TurnOn();
        }

        Dice = null;
    }

    public void Return(NodeObject node)
    {
        var find = dices.Find(n => n.DiceId == node.RouteId);
        if (!ReferenceEquals(null, find))
        {
            find.TurnOn();
        }
    }
}