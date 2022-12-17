using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private RectTransform squareInfoPanel;

    public void Hide() => gameObject.SetActive(false);

    public void SetSquareInfo(BattleSquare square) {
        if(!square) return;
        TextMeshProUGUI[] tmpTxts = squareInfoPanel.GetComponentsInChildren<TextMeshProUGUI>();
        tmpTxts[0].text = $"{square.activeElement} Square";
        tmpTxts[1].text = $"Inhabitant: {(square.ContainsUnits() ? square.GetInhabitedUnits()[0].name : "<empty>")}";
        gameObject.SetActive(true);
    }

}
