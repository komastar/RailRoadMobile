using Manager;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceObject : MonoBehaviour, IGameActor
{
    public Button diceButton;
    public Image buttonRenderer;
    public Image routeRenderer;

    private DataManager dataManager;

    public DiceModel diceData;
    public Action<DiceObject> onClick;
    public Action<IActor> onClickObject;

    [SerializeField]
    private int id;
    public int Id { get => id; set => id = value; }

    [SerializeField]
    private int diceId;
    public int DiceId { get => diceId; set => diceId = value; }

    public void Roll(int dice = -1)
    {
        if (-1 == dice)
        {
            DiceId = diceData.Routes[Random.Range(0, diceData.Routes.Length)];
        }
        else
        {
            DiceId = dice;
        }
        var routeData = dataManager.RouteData[DiceId];
        routeRenderer.sprite = SpriteManager.Get().GetSprite(routeData.Name);
        TurnOn();
    }

    public void Init(int id)
    {
        dataManager = DataManager.Get();
        Id = id;
        DiceId = Id;
        diceData = dataManager.DiceData[id];
        onClick = null;
    }

    public void OnClickDice()
    {
        onClick?.Invoke(this);
        onClickObject?.Invoke(this);
    }

    public void TurnOn()
    {
        diceButton.interactable = true;
    }

    public void TurnOff()
    {
        diceButton.interactable = false;
    }
}