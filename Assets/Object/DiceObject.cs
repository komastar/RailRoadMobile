using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceObject : MonoBehaviour, IGameActor
{
    public Image routeRenderer;

    private DataManager dataManager;

    public DiceModel diceData;
    public Action<DiceObject> onClick;

    public int Id { get; set; }
    public int DiceId { get; set; }

    public void Roll()
    {
        DiceId = diceData.Routes[Random.Range(0, diceData.Routes.Length)];
        var routeData = dataManager.RouteData[DiceId];
        routeRenderer.sprite = SpriteManager.Get().GetSprite(routeData.Name);
    }

    public void Init(int id)
    {
        dataManager = DataManager.Get();
        Id = id;
        diceData = dataManager.DiceData[id];
        onClick = null;
    }

    public void OnClickDice()
    {
        onClick?.Invoke(this);
    }
}