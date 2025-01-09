using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogText : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;

    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject UtilitiesSelector;
    [SerializeField] GameObject UtilitiesDetails;
    [SerializeField] GameObject finishSelector;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> UtilitiesTexts;
    [SerializeField] List<Text> finishTexts;

    [SerializeField] Text ppText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
        }
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }
    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }
    public void EnableUtilitiesSelector(bool enabled)
    {
        UtilitiesSelector.SetActive(enabled);
        UtilitiesDetails.SetActive(enabled);
    }
    public void EnableFinishSelector(bool enabled)
    {
        finishSelector.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateFinishSelection(int selectedFinish)
    {
        for (int i = 0; i < finishTexts.Count; ++i)
        {
            if (i == selectedFinish)
                finishTexts[i].color = highlightedColor;
            else
                finishTexts[i].color = Color.black;
        }
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i=0; i<UtilitiesTexts.Count; ++i)
        {
            if (i < moves.Count)
                UtilitiesTexts[i].text = moves[i].Base.Name;
            else
                UtilitiesTexts[i].text = "-";
        }
    }

    public void UpdateUtilitiesSelection(int selectedMoves, Move move)
    {
        for (int i = 0; i < UtilitiesTexts.Count; ++i)
        {
            if (i == selectedMoves)
                UtilitiesTexts[i].color = highlightedColor;
            else
                UtilitiesTexts[i].color = Color.black;
        }

        ppText.text = $"PP{move.PP}/{move.Base.PP}";
    }
}
