using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Level[] levels;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levels= GetComponentsInChildren<Level>();
    }

    /*
    0 air
    1 block
    2 water
    3 death
    4 checkpoint
    5 item thing
    6 trap
     
     */
    public int getTile(Vector2 pos)
    {
        int levelIndex = -1;
        for (int i = 0; i < levels.Length; i++)
        {
            if (inBoundBox(pos, levels[i].boundsOfLevel))
            {

                levelIndex = i;
            }
        }
        if (levelIndex == -1) { return 0; }

        //if the level collapses you die in it
        if (levels[levelIndex].collapsed) { return 3; }


        if ((int)(pos.x - levels[levelIndex].boundsOfLevel[0].x) < levels[levelIndex].levelData.GetLength(0) &&
             (int)(pos.y - levels[levelIndex].boundsOfLevel[0].y) < levels[levelIndex].levelData.GetLength(1))
        {
            if (levels[levelIndex].levelData[(int)(pos.x - levels[levelIndex].boundsOfLevel[0].x),
               (int)(pos.y - levels[levelIndex].boundsOfLevel[0].y)] > levels[levelIndex].translationTable.Length)
            {
                return 0;
            }


            return levels[levelIndex].translationTable[
               levels[levelIndex].levelData[(int)(pos.x - levels[levelIndex].boundsOfLevel[0].x),
               (int)(pos.y - levels[levelIndex].boundsOfLevel[0].y)]];

        }

        return 0;

    }
   
    public Level getLevel(Vector2 pos) 
    {
        int levelIndex =0;
        for (int i = 0; i < levels.Length; i++)
        {
            if (inBoundBox(pos, levels[i].boundsOfLevel))
            {

                levelIndex = i;
            }
        }
        return levels[levelIndex];
    }
    
    bool inBoundBox(Vector2 pos, Vector2[] boundingBox) 
    { 
        return pos.x <= boundingBox[1].x && pos.x >= boundingBox[0].x&& pos.y <= boundingBox[1].y && pos.y >= boundingBox[0].y;
    }
}
