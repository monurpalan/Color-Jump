using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Oyun blok renklerini tanımlar
public enum BLOCKCOLOR
{ PINK, YELLOW, ORANGE, VIOLET, BLUE, RED, NAVY, GREEN }

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject startPanel, endPanel, player, left, right, blockPrefab;

    [SerializeField]
    private TMP_Text scoreText, highScoreText, highScoreEndText;

    private int score, highScore;
    private BLOCKCOLOR currentColor;
    private const float maxSize = 80f;
    private int currentLevel;

    // Seviye ve blok/puan eşleşmeleri
    private readonly Dictionary<int, int> levelToBlock = new Dictionary<int, int>()
    {
        {1,1},{2,2},{3,3},{4,4},{5,5},{6,6},{7,8}
    };
    private readonly Dictionary<int, int> levelToScore = new Dictionary<int, int>()
    {
        {1,1},{2,2},{3,5},{4,6},{5,75},{6,100},{7,150}
    };

    [SerializeField]
    public List<Color32> colors = new List<Color32>();

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        startPanel.SetActive(true);
        endPanel.SetActive(false);
        Time.timeScale = 1f;
        score = 0;
        highScore = PlayerPrefs.HasKey("HighScore") ? PlayerPrefs.GetInt("HighScore") : 0;
        scoreText.text = score.ToString();
        highScoreText.text = "BEST " + highScore.ToString();
        currentLevel = 1;
        SpawnBlocks();
        SetRandomPlayerColor();
        SetColors();
    }

    // Blokları oluşturur
    void SpawnBlocks()
    {
        int numOfBlocks = levelToBlock[currentLevel];
        for (int i = 0; i < numOfBlocks; i++)
        {
            CreateBlock(left.transform, numOfBlocks, i);
            CreateBlock(right.transform, numOfBlocks, i);
        }
    }

    // Tek bir blok oluşturur
    void CreateBlock(Transform parent, int numOfBlocks, int index)
    {
        GameObject tempBlock = Instantiate(blockPrefab);
        tempBlock.transform.parent = parent;
        Vector3 tempScale = tempBlock.transform.localScale;
        tempScale.y = maxSize / numOfBlocks;
        tempBlock.transform.localScale = tempScale;
        Vector3 tempPos = Vector3.zero;
        tempPos.y = tempScale.y * (numOfBlocks / 2 - index) - (numOfBlocks % 2 == 0 ? tempScale.y / 2 : 0);
        tempBlock.transform.localPosition = tempPos;
    }

    // Bloklara rastgele renk atar
    void SetColors()
    {
        List<GameObject> leftBlocks = GetChildren(left.transform);
        List<GameObject> rightBlocks = GetChildren(right.transform);
        List<BLOCKCOLOR> availableColors = new List<BLOCKCOLOR>() {
            BLOCKCOLOR.PINK, BLOCKCOLOR.YELLOW, BLOCKCOLOR.ORANGE, BLOCKCOLOR.VIOLET,
            BLOCKCOLOR.BLUE, BLOCKCOLOR.RED, BLOCKCOLOR.NAVY, BLOCKCOLOR.GREEN
        };
        List<BLOCKCOLOR> currentColorList = new List<BLOCKCOLOR>() { currentColor };
        availableColors.Remove(currentColor);

        // Sol bloklara renk ata
        for (int i = 1; i < leftBlocks.Count; i++)
        {
            BLOCKCOLOR tempColor = availableColors[Random.Range(0, availableColors.Count)];
            currentColorList.Add(tempColor);
            availableColors.Remove(tempColor);
        }
        AssignColorsToBlocks(leftBlocks, currentColorList);
        AssignColorsToBlocks(rightBlocks, currentColorList);
    }

    // Bloklara renkleri atar
    void AssignColorsToBlocks(List<GameObject> blocks, List<BLOCKCOLOR> colorList)
    {
        List<GameObject> tempList = new List<GameObject>(blocks);
        foreach (var color in colorList)
        {
            int idx = Random.Range(0, tempList.Count);
            var block = tempList[idx];
            block.GetComponent<Block>().blockColor = color;
            block.GetComponent<SpriteRenderer>().color = colors[(int)color];
            tempList.RemoveAt(idx);
        }
    }

    // Bir parent altındaki tüm çocukları döndürür
    List<GameObject> GetChildren(Transform parent)
    {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++)
            children.Add(parent.GetChild(i).gameObject);
        return children;
    }

    // Oyuncuya rastgele bir renk atar
    void SetRandomPlayerColor()
    {
        currentColor = (BLOCKCOLOR)Random.Range(0, 8);
        player.GetComponent<SpriteRenderer>().color = colors[(int)currentColor];
        player.GetComponent<Player>().playerColor = currentColor;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && startPanel.activeSelf)
        {
            GameStart();
        }
    }

    // Yeni seviye için blokları yeniden oluşturur
    void Reshuffle()
    {
        currentLevel++;
        ClearBlocks(left.transform);
        ClearBlocks(right.transform);
        SpawnBlocks();
    }

    // Bir parent altındaki tüm blokları siler
    void ClearBlocks(Transform parent)
    {
        foreach (Transform child in parent)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    // Skoru günceller ve blokları yeniler
    public void UpdateScore()
    {
        score++;
        scoreText.text = score.ToString();
        Invoke("SetBlocks", 0.05f);
    }

    // Blokları ve oyuncu rengini günceller
    void SetBlocks()
    {
        SetRandomPlayerColor();
        if (levelToScore.TryGetValue(currentLevel + 1, out int tempScore))
        {
            if (score > tempScore)
            {
                Reshuffle();
            }
        }
        SetColors();
    }

    public void GameStart()
    {
        startPanel.SetActive(false);
        player.GetComponent<Player>().GameStart();
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        endPanel.SetActive(true);
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        highScoreEndText.text = "BEST " + highScore.ToString();
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void GameRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
