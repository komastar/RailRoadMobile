using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceObject : MonoBehaviour, IGameActor
{
    public Toggle diceButton;
    public Image buttonRenderer;
    public Image routeRenderer;

    private DataManager dataManager;

    public DiceModel diceData;
    public Action<DiceObject> onClick;

    [SerializeField]
    private int id;
    public int Id { get => id; set => id = value; }

    [SerializeField]
    private int diceId;
    public int DiceId { get => diceId; set => diceId = value; }

    public void Roll()
    {
        DiceId = diceData.Routes[Random.Range(0, diceData.Routes.Length)];
        var routeData = dataManager.RouteData[DiceId];
        routeRenderer.sprite = SpriteManager.Get().GetSprite(routeData.Name);
        TurnOn();
    }

    public void Init(int id)
    {
        dataManager = DataManager.Get();
        Id = id;
        diceData = dataManager.DiceData[id];
        onClick = null;
    }

    public void OnClickDice(bool value)
    {
        if (value)
        {
            onClick?.Invoke(this);
        }
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