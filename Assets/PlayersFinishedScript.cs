using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayersFinishedScript : MonoBehaviour
{
    public Player player1;
    public Player player2;
    public Transform minPos;
    public Transform maxPos;

    public string nameOfTitleScene;
    // Update is called once per frame
    void Update()
    {
        Vector2 p1Pos = player1.fixedToFloating(player1.pos);
        Vector2 p2Pos = player2.fixedToFloating(player2.pos);
        if (p1Pos.x > minPos.position.x && p1Pos.y > minPos.position.y &&
            p1Pos.x < maxPos.position.x && p1Pos.y < maxPos.position.y &&
            p2Pos.x > minPos.position.x && p2Pos.y > minPos.position.y &&
            p2Pos.x < maxPos.position.x && p2Pos.y < maxPos.position.y)
        {
            SceneManager.LoadScene(nameOfTitleScene);
        }
    }
}
