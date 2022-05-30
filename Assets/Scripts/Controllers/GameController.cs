using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SpeedController speedController;
    [SerializeField] private GameObject diedCanvas;
    [SerializeField] private GameObject butText;
    [SerializeField] private GameObject beginningText;
    [SerializeField] private GameObject hellCanvas;
    [SerializeField] private float butTextDelay;
    [SerializeField] private float beginningTextDelay;
    [SerializeField] private float transmitToHellDelay;
    [SerializeField] private GameObject worldCleaner;
    [SerializeField] private GameObject firstSpawner;
    [SerializeField] private GameObject hellSpawner;
    [SerializeField] private PlayerMoveController playerMoveController;
    [SerializeField] private float newMaxHeight;
    [SerializeField] private float newMinHeight;
    [SerializeField] private float newMaxWidth;
    [SerializeField] private float newMinWidth;
    [SerializeField] private GameObject shootText;
    [SerializeField] private GameObject shootTextShadow;
    [SerializeField] private float shootTextDelay;
    [SerializeField] private int shootTextBlinkCount;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private float enterToCastleSpeed;
    [SerializeField] private PlayerFireController playerFireController;
    [SerializeField] private GameObject WinCanvas;
    [SerializeField] private GameObject GameCanvas;
    [SerializeField] private ScoreKeeper scoreKeeper;
    [SerializeField] private TMP_Text finalScoreText;

    private float tmpSpeed;
    private int tmpLevel;
    private bool isFirstShotgunPickUp = true;

    private void OnEnable()
    {
        player.Died += OnPlayerDied;
        player.FreezeWorld += OnFreezeWorld;
        player.PickUpShotgunInHell += OnPickUpShotgunInHell;
        player.GameOver += OnGameOver;
        player.PlayerEnterCastleTrigger += OnPlayerEnterCastleTrigger;
        player.PlayerWin += OnPlayerWin;
    }

    private void OnDisable()
    {
        player.Died -= OnPlayerDied;
        player.FreezeWorld -= OnFreezeWorld;
        player.PickUpShotgunInHell -= OnPickUpShotgunInHell;
        player.GameOver -= OnGameOver;
        player.PlayerEnterCastleTrigger -= OnPlayerEnterCastleTrigger;
        player.PlayerWin -= OnPlayerWin;
    }

    private void OnPlayerDied()
    {
        firstSpawner.GetComponent<Spawner>().SetSpawnCondition(false);
        diedCanvas.SetActive(true);
        StartCoroutine(TransmitToHell());
    }

    private IEnumerator TransmitToHell()
    {
        speedController.SetCurrentSpeed(0);
        tmpLevel = firstSpawner.GetComponent<Spawner>().CurrentLevel;
        worldCleaner.SetActive(true);
        firstSpawner.SetActive(false);
        yield return new WaitForSeconds(butTextDelay);
        butText.SetActive(true);
        worldCleaner.SetActive(false);
        yield return new WaitForSeconds(beginningTextDelay);
        beginningText.SetActive(true);
        player.SetInitialCondition();
        player.SetHealthValue(1);
        playerMoveController.SetBorders(newMaxHeight, newMinHeight, newMaxWidth, newMinWidth);
        playerMoveController.EnableMove();
        yield return new WaitForSeconds(transmitToHellDelay);
        diedCanvas.SetActive(false);
        hellCanvas.SetActive(true);
        hellSpawner.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        hellSpawner.GetComponent<Spawner>().SetCurrentLevel(tmpLevel - 1);
        UnfreezeWorld();
    }

    private void OnFreezeWorld()
    {
        tmpSpeed = speedController.CurrentSpeed;
        speedController.SetCurrentSpeed(0);
    }

    private void UnfreezeWorld()
    {
        speedController.SetCurrentSpeed(tmpSpeed);
    }

    private void OnPickUpShotgunInHell()
    {
        playerMoveController.ResetBorders();
        if(isFirstShotgunPickUp)
        {
            StartCoroutine(BlinkShootText());
            isFirstShotgunPickUp = false;
        }
    }

    private IEnumerator BlinkShootText()
    {
        for (int i = 0; i < shootTextBlinkCount; i++)
        {
            shootText.SetActive(true);
            shootTextShadow.SetActive(true);
            yield return new WaitForSeconds(shootTextDelay);
            shootText.SetActive(false);
            shootTextShadow.SetActive(false);
            yield return new WaitForSeconds(shootTextDelay);
        }
    }

    private void OnGameOver()
    {
        hellSpawner.GetComponent<Spawner>().SetSpawnCondition(false);
        gameOverScreen.SetActive(true);
    }

    private void OnPlayerEnterCastleTrigger()
    {
        player.TurnOffPowerUp();
        speedController.SetCurrentSpeed(enterToCastleSpeed);
    }

    private void OnPlayerWin()
    {
        Debug.Log("YOU WIN!!!");
        playerMoveController.DisableMove();
        playerFireController.DisableFire();
        WinCanvas.SetActive(true);
        GameCanvas.SetActive(false);
        StartCoroutine(displayFinalScore());
    }

    private IEnumerator displayFinalScore()
    {
        yield return new WaitForSeconds(0.001f);
        finalScoreText.text = scoreKeeper.CoinCount.ToString();
    }
}
