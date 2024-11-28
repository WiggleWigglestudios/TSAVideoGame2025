using UnityEngine;
using UnityEngine.Tilemaps;
public class PlayerMovement : MonoBehaviour
{

    public Vector2 pos;
    public Vector2 vel;
    public Vector2 checkPointPos;
    public float jumpHeight;
    public float speed;
    public int maxJumps;
    float jumps;
    bool canJump;
    bool grounded;

    public Tilemap map;
    public TilemapRenderer mapRenderer;
    public string[] mapTileNames;

    public SpriteRenderer spriteRenderer;
    public Sprite[] playerSprite;

    const float gravity = -9;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer.sprite = playerSprite[0];
        pos=transform.position;
    }
    private void FixedUpdate()
    {

        ////Debug.Log(getTile(transform.position));
        int subSteps = 1;
        for (int i = 0; i < subSteps; i++)
        {
            resolveTerrainCollisions();

          
            vel.y += gravity * Time.fixedDeltaTime / (float)subSteps;
            playerInputs(subSteps); 
            pos += vel * Time.fixedDeltaTime / (float)subSteps;
            transform.position = new Vector3(pos.x, pos.y, -5);
        }

        if (grounded)
        {
            //friction resistance
        //   vel.x *= Mathf.Pow(0.01f, Time.deltaTime);
        //    vel.y *= Mathf.Pow(0.01f, Time.deltaTime);
        }
        else
        {
            //air resistance
        //    vel.x *= Mathf.Pow(0.99f, Time.deltaTime);
        //   vel.y *= Mathf.Pow(0.99f, Time.deltaTime);
        }


    }

    void playerInputs(int subSteps) 
    {
        if (grounded)
        {
            jumps = maxJumps;
            canJump = true;
        }
        else 
        {
            jumps = Mathf.Max(maxJumps - 1, jumps);
        }

        if (Input.GetKey(KeyCode.D))
        {
            vel.x += speed*Time.fixedDeltaTime/ (float)subSteps;
        }
        if (Input.GetKey(KeyCode.A))
        {
            vel.x -= speed * Time.fixedDeltaTime / (float)subSteps;
        }

        if (!Input.GetKey(KeyCode.W)) 
        {
            canJump = true;
        }
        if (Input.GetKey(KeyCode.W)&&canJump&&jumps>0)
        {
            jumps--;
            canJump = false;
            //1/2mv^2=mgh    1/2v^2=gh   sqrt(2gh)=v
            vel.y = Mathf.Sqrt(Mathf.Abs(2 * gravity * jumpHeight));
            grounded = false;
        }

    }

    void resolveTerrainCollisions() 
    {
        //Debug.Log("1 " + getTile(pos));
        ////Debug.Log("2 " + getTile(new Vector2(pos.x + 1, pos.y)));
        //Debug.Log("3 " + getTile(new Vector2(pos.x , pos.y+1)));
        //Debug.Log("4 " + getTile(new Vector2(pos.x + 1, pos.y+1)));
        grounded = false;

       // //Debug.Log("1 " + getTile(pos));
        if (getTile(pos)>= 1)
        {
         //   //Debug.Log("1");
            if (Mathf.Floor(pos.x) + 1 - pos.x < Mathf.Floor(pos.y) + 1 - pos.y)
            {
           //     //Debug.Log("1 x");
                pos.x = Mathf.Floor(pos.x) + 1;
                vel.x = Mathf.Max(0,vel.x);
            }
            else
            {
              //  //Debug.Log("1 y");
                pos.y = Mathf.Floor(pos.y) + 1;
                vel.y = Mathf.Max(0,vel.y);
                grounded = true;
            }
        }
        //Debug.Log("2 " + getTile(new Vector2(pos.x + 1, pos.y)));
        if (getTile(new Vector2(pos.x+1,pos.y)) >= 1)
        {
            //Debug.Log("2");
            if (pos.x - Mathf.Floor(pos.x) < Mathf.Floor(pos.y) + 1 - pos.y)
            {
                //Debug.Log("2 x");
                pos.x = Mathf.Floor(pos.x);
                vel.x = Mathf.Min(0, vel.x);
            }
            else
            {
                //Debug.Log("2 y");
                pos.y = Mathf.Floor(pos.y) + 1;
                vel.y = Mathf.Min(0, vel.y);
                grounded = true;
            }
        }
        //Debug.Log("3 " + getTile(new Vector2(pos.x, pos.y+1)));
        if (getTile(new Vector2(pos.x, pos.y+1)) >= 1)
        {
            //Debug.Log("3");
            if (Mathf.Floor(pos.x) + 1 - pos.x < pos.y - Mathf.Floor(pos.y))
            {
                //Debug.Log("3 x");
                pos.x = Mathf.Floor(pos.x) + 1;
                vel.x = Mathf.Max(0, vel.x);
            }
            else
            {
                //Debug.Log("3 y");
                pos.y = Mathf.Floor(pos.y);
                vel.y = Mathf.Min(0, vel.y);
            }
        }
        //Debug.Log("4 " + getTile(new Vector2(pos.x + 1, pos.y+1)));
        if (getTile(new Vector2(pos.x + 1, pos.y+1)) >= 1)
        {
            //Debug.Log("4");
            if (pos.x - Mathf.Floor(pos.x) < pos.y - Mathf.Floor(pos.y))
            {
                //Debug.Log("4 x");
                pos.x = Mathf.Floor(pos.x);
                vel.x = Mathf.Min(0, vel.x);
            }
            else
            {
                //Debug.Log("4 y");
                pos.y = Mathf.Floor(pos.y);
                vel.y = Mathf.Min(0, vel.y);
            }
        }

    }


    int getTile(Vector2 inputPos)
    {
        Vector3Int gridPosition = map.WorldToCell(new Vector3((int)inputPos.x-1, (int)inputPos.y,0));
        TileBase tile= map.GetTile(gridPosition);
        ////Debug.Log(tile.ToString());
        //output=int.Parse(tile.ToString());
        
        //0 air, 1 is tile, 2 is water, 3 is lava, 4 is checkpoint
        int[] tileNumberToType = new int[]
        { 
          2,1,1,1,0,
          1,1,1,1,1,
          1,1,1,1,1,
          1,1,1,1,1,
          1,1,1,1,1,
          1,1,1,1,4,
          0,0,3,0,3,
          3,3,1,0
        };         
        

        return tileNumberToType[getFirstNumberInString(tile.ToString(),100)];
    }


    int getFirstNumberInString(string inputString, int maxInt)
    { 
        for(int i=maxInt;i>=0;i--)
        {

            if(inputString.IndexOf(""+i)>=0)
            {
                return i;
            }
        }
        return 0;
    }

}
