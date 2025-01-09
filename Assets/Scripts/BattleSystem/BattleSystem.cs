using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy, Finish }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUI playerHUD;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleUI enemyHUD;
    [SerializeField] DialogText dialogBox;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentFinish;

    public void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHUD.SetData(playerUnit.Unit);
        enemyHUD.SetData(enemyUnit.Unit);

        dialogBox.SetMoveNames(playerUnit.Unit.Moves);

        yield return dialogBox.TypeDialog("A guard is approaching...");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }
    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose your action"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableUtilitiesSelector(true);
    }

    void FinishGame()
    {
        state = BattleState.Finish;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(true);
        dialogBox.EnableFinishSelector(true);
    }

    IEnumerator PlayerDefend()
    {
        var move = enemyUnit.Unit.RandomMove();
        var playerAnim = playerUnit.GetComponent<Animator>();
        playerAnim.SetBool("Defend", true);
        yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name} is attacking!");
        yield return new WaitForSeconds(1f);
        bool isDefending = true;
        bool isFainted = playerUnit.Unit.TakeDamage(move, playerUnit.Unit, isDefending);
        playerAnim.SetBool("Hit", true);
        yield return playerHUD.UpdateHP();
        playerAnim.SetBool("Hit", false);
        if (isDefending)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name} is successfully defend");
            yield return new WaitForSeconds(0.5f);
            playerAnim.SetBool("Defend", false);
            PlayerAction();
        }
        else if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name} Fainted!");
            playerAnim.SetTrigger("Death");
        }
    }

    IEnumerator Utility()
    {
        var move = playerUnit.Unit.Moves[currentMove];
        float powerMultiplier = 1f;
        yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name} used {move.Base.Name}!");
        if (currentMove == 0)
        {
            //Power Up
            powerMultiplier = 1.5f;
            yield return new WaitForSeconds(1f);
            PlayerAction();
        }
        else if (currentMove == 1)
        {
            Debug.Log("Dust");
            enemyUnit.Unit.IsBlinded = true;
            yield return new WaitForSeconds(1f);
            yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name} is blinded and may miss their attacks!");
            yield return new WaitForSeconds(1f);
            PlayerAction();
            --currentMove;
        }
        playerUnit.Unit.PowerMultiplier = powerMultiplier;
    }

    IEnumerator RemoveBlindnessAfterTurns(int turns)
    {
        for (int i = 0; i < turns; i++)
        {
            yield return new WaitForSeconds(1f);
        }

        enemyUnit.Unit.IsBlinded = false;
        yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name} is no longer blind!");
        yield return new WaitForSeconds(1f);
        PlayerAction();
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var playerAnim = playerUnit.GetComponent<Animator>();
        var enemyAnim = enemyUnit.GetComponent<Animator>();
        var move = playerUnit.Unit.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name} is attacking!");
        yield return new WaitForSeconds(1f);

        int originalPower = move.Base.Power;
        int adjustedPower = Mathf.FloorToInt(originalPower * playerUnit.Unit.PowerMultiplier);
        Debug.Log($"Original Power: {originalPower}, Adjusted Power: {adjustedPower}");

        move.Base.Power = adjustedPower;
        playerAnim.SetTrigger("Attack");
        yield return dialogBox.TypeDialog($"Damage outcome: {move.Base.Power}");
        playerAnim.SetTrigger("Idle");

        bool isFainted = enemyUnit.Unit.TakeDamage(move, playerUnit.Unit);
        move.Base.Power = originalPower;
        playerUnit.Unit.PowerMultiplier = 1f;
        enemyAnim.SetBool("Hit", true);
        yield return enemyHUD.UpdateHP();
        enemyAnim.SetBool("Hit", false);
        Debug.Log($"Power reset to: {move.Base.Power}");
        yield return new WaitForSeconds(1f);

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name} Fainted!");
            enemyAnim.SetTrigger("Death");
            yield return new WaitForSeconds(1f);
            yield return dialogBox.TypeDialog("You Won!");
            FinishGame();
        }
        else
        {
            StartCoroutine(EnemyMove());
        }

    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Unit.RandomMove();
        var playerAnim = playerUnit.GetComponent<Animator>();
        var enemyAnim = enemyUnit.GetComponent<Animator>();
        if (enemyUnit.Unit.IsBlinded)
        {
            float missChance = Random.Range(0f, 1f);

            if (missChance < 0.3f)
            {
                yield return new WaitForSeconds(1f);
                yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name} is blinded and missed the attack!");
                yield return new WaitForSeconds(1f);
                StartCoroutine(RemoveBlindnessAfterTurns(0));
                yield break;
            }
        }
        yield return new WaitForSeconds(1f);
        yield return dialogBox.TypeDialog($"{enemyUnit.Unit.Base.Name} is attacking!");
        yield return new WaitForSeconds(1f);

        bool isFainted = playerUnit.Unit.TakeDamage(move, playerUnit.Unit);
        enemyAnim.SetTrigger("Attack");
        yield return dialogBox.TypeDialog($"Damage outcome: {move.Base.Power}");
        enemyAnim.SetTrigger("Idle");
        playerAnim.SetBool("Hit", true);
        yield return playerHUD.UpdateHP();
        playerAnim.SetBool("Hit", false);

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Unit.Base.Name} Fainted!");
            playerAnim.SetTrigger("Death");
            yield return new WaitForSeconds(1f);
            yield return dialogBox.TypeDialog("You're defeated.");
            FinishGame();
        }
        else
        {
            PlayerAction();
        }
    }

    private void Update()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleUtilitiesSelection();
        }
        else if(state == BattleState.Finish)
        {
            HandleFinishSelection();
        }
    }

    IEnumerator ApplicationQuit()
    {
        yield return dialogBox.TypeDialog("You run away");
        yield return new WaitForSeconds(1f);
        yield return dialogBox.TypeDialog("You were considered as defeated");
        yield return new WaitForSeconds(1f);
        Application.Quit();
        Debug.Log("You're quitting game");
    }

    IEnumerator Quitgame()
    {
        yield return dialogBox.TypeDialog("Thanks for playing the game!");
        yield return new WaitForSeconds(1f);
        Application.Quit();
        Debug.Log("The game is over!");
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(1f);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Debug.Log("You're restarting the game");
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currentAction < 3)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if(currentAction == 0)
            {
                dialogBox.EnableActionSelector(false);
                dialogBox.EnableDialogText(true);
                StartCoroutine(PerformPlayerMove());
            }
            else if(currentAction == 1)
            {
                //Defend
                dialogBox.EnableActionSelector(false);
                dialogBox.EnableDialogText(true);
                StartCoroutine(PlayerDefend());
            }
            else if(currentAction == 2)
            {
                //Utility
                PlayerMove();
            }
            else
            {
                //Run
                dialogBox.EnableDialogText(true);
                StartCoroutine(ApplicationQuit());
            }
        }
    }

    void HandleUtilitiesSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Unit.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Unit.Moves.Count - 2)
                currentMove += 2;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateUtilitiesSelection(currentMove, playerUnit.Unit.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            dialogBox.EnableUtilitiesSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(Utility());
        }
    }

    void HandleFinishSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentFinish < 1)
                ++currentFinish;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentFinish > 0)
                --currentFinish;
        }

        dialogBox.UpdateFinishSelection(currentFinish);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (currentFinish == 0)
            {
                dialogBox.EnableActionSelector(false);
                dialogBox.EnableDialogText(true);
                StartCoroutine(Restart());
            }
            else if (currentFinish == 1)
            {

                dialogBox.EnableActionSelector(false);
                dialogBox.EnableDialogText(true);
                StartCoroutine(Quitgame());
            }
        }
    }
}