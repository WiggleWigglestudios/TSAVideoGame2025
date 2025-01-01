using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class Player : MonoBehaviour
{
    public Vector2 pos;
    public Vector2 vel;
    public Vector2 checkPointPos;
    public float jumpHeight;
    float normalJumpHeight;
    public float speed;
    float normalSpeed;
    public int maxJumps;
    public float jumps;
    bool canJump;
    bool grounded;
    bool inWater;

    public LevelManager levelManager;

    public SpriteRenderer spriteRenderer;
    public Sprite[] playerSprites;

    const float gravity = -30;

    public bool UsesArrowKeys;

    int AnimationFrameCount = 0;
    public float AnimationFrameRate = 8;
    float AnimationFrameCountdown = 0.25f;
    public bool facing;

    public List<Item> items=new List<Item>();

    public List<Vector2> checkPointHits = new List<Vector2>();
    [Serialize]
    public List<Vector2> itemtHits = new List<Vector2>();

    public Player otherPlayer;
    public ItemUIManager itemUIManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        normalSpeed = speed;
        normalJumpHeight = jumpHeight;
        spriteRenderer.sprite = playerSprites[4];
        pos = transform.position;
        pos -= new Vector2(0.5f, 0.5f);
    }

    private void Update()
    {
        updateAnimations();
        itemUpdate();
    }

    private void FixedUpdate()
    {
       
        // Debug.Log(levelManager.getTile(transform.position));

        vel.y += gravity * Time.fixedDeltaTime;
        int subSteps = 4;
        playerInputs(1);
        grounded = false;
        for (int i = 0; i < subSteps; i++)
        {



            pos += vel * Time.fixedDeltaTime / (float)subSteps;
            resolveTerrainCollisions();
        }
        if (grounded)
        {
            jumps = maxJumps;
            canJump = true;
        }
        else
        {
            jumps = Mathf.Min(maxJumps - 1, jumps);
        }


        transform.position = new Vector3(pos.x+0.5f, pos.y+0.5f, -5);
        if (grounded)
        {
            //friction resistance
            vel.x *= Mathf.Pow(0.01f, Time.deltaTime);
            // vel.y *= Mathf.Pow(0.01f, Time.deltaTime);
        }
        else
        {
            //air resistance
            vel.x *= Mathf.Pow(0.05f, Time.deltaTime);
            // vel.y *= Mathf.Pow(0.05f, Time.deltaTime);
        }

        tileCheck();

    }

    void playerInputs(int subSteps)
    {
       
        if ((Input.GetKey(KeyCode.D) && !UsesArrowKeys) || (Input.GetKey(KeyCode.RightArrow) && UsesArrowKeys))
        {
            vel.x += speed * Time.fixedDeltaTime / (float)subSteps;
            facing = true;
        }
        if ((Input.GetKey(KeyCode.A) && !UsesArrowKeys) || (Input.GetKey(KeyCode.LeftArrow) && UsesArrowKeys))
        {
            vel.x -= speed * Time.fixedDeltaTime / (float)subSteps;
            facing = false;
        }

        if (!((Input.GetKey(KeyCode.W) && !UsesArrowKeys) || (Input.GetKey(KeyCode.UpArrow) && UsesArrowKeys)))
        {
            canJump = true;
        }
        if (((Input.GetKey(KeyCode.W) && !UsesArrowKeys) || (Input.GetKey(KeyCode.UpArrow) && UsesArrowKeys)) && canJump && jumps > 0&&!inWater)
        {
            jumps--;
            canJump = false;
            //1/2mv^2=mgh    1/2v^2=gh   sqrt(2gh)=v
            vel.y = Mathf.Sqrt(Mathf.Abs(2 * gravity * jumpHeight));
            grounded = false;
        }


        //swimming
        if (inWater)
        {
            if (((Input.GetKey(KeyCode.W) && !UsesArrowKeys) || (Input.GetKey(KeyCode.UpArrow) && UsesArrowKeys)))
            {
                vel.y += -gravity * jumpHeight/4.0f*Time.deltaTime* WaterDepth()*3.0f;
                float depth = WaterDepth();
                if (depth < 0.1f&&vel.y>0) 
                {
                    pos.y += 0.1f;
                }
            }
            if (((Input.GetKey(KeyCode.S) && !UsesArrowKeys) || (Input.GetKey(KeyCode.DownArrow) && UsesArrowKeys)))
            {
                vel.y -= -gravity * jumpHeight / 4.0f * Time.deltaTime* WaterDepth()*3.0f;
            }
        }


    }

    void resolveTerrainCollisions()
    {
        //Debug.Log("1 " + getTile(pos));
        ////Debug.Log("2 " + getTile(new Vector2(pos.x + 1, pos.y)));
        //Debug.Log("3 " + getTile(new Vector2(pos.x , pos.y+1)));
        //Debug.Log("4 " + getTile(new Vector2(pos.x + 1, pos.y+1)));
        //  grounded = false;



        // //Debug.Log("1 " + getTile(pos));
        if (levelManager.getTile(new Vector2(pos.x, pos.y)) == 1)
        {
            //   //Debug.Log("1");
            if (Mathf.Floor(pos.x) + 1 - pos.x < Mathf.Floor(pos.y) + 1 - pos.y)
            {
                //Debug.Log("1 x");
                pos.x = Mathf.Floor(pos.x) + 1;
                vel.x = Mathf.Max(0, vel.x);
            }
            else
            {
                //Debug.Log("1 y");
                pos.y = Mathf.Floor(pos.y) + 1;
                vel.y = Mathf.Max(0, vel.y);
                grounded = true;
            }
        }
        //Debug.Log("2 " + getTile(new Vector2(pos.x + 1, pos.y)));
        if (levelManager.getTile(new Vector2(pos.x + 1, pos.y)) == 1)
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
                vel.y = Mathf.Max(0, vel.y);
                grounded = true;
            }
        }
        //Debug.Log("3 " + getTile(new Vector2(pos.x, pos.y+1)));
        if (levelManager.getTile(new Vector2(pos.x, pos.y + 1)) == 1)
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
        if (levelManager.getTile(new Vector2(pos.x + 1, pos.y + 1)) == 1)
        {
            //Debug.Log("4");
            if (pos.x - Mathf.Floor(pos.x) < pos.y - Mathf.Floor(pos.y))
            {
                ////Debug.Log("4 x");
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

    void tileCheck()
    {
        //checkPoint check
        if (levelManager.getTile(new Vector2Int((int)pos.x, (int)pos.y)) == 4) {            checkPointCheck(new Vector2Int((int)pos.x, (int)pos.y)); }
        if (levelManager.getTile(new Vector2Int((int)pos.x + 1, (int)pos.y)) == 4) {        checkPointCheck(new Vector2Int((int)pos.x+1, (int)pos.y)); }
        if (levelManager.getTile(new Vector2Int((int)pos.x, (int)pos.y + 1)) == 4) {        checkPointCheck(new Vector2Int((int)pos.x, (int)pos.y+1)); }
        if (levelManager.getTile(new Vector2Int((int)pos.x + 1, (int)pos.y + 1)) == 4) {    checkPointCheck(new Vector2Int((int)pos.x+1, (int)pos.y+1)); }

        //Death check
        if (levelManager.getTile(new Vector2(pos.x + 0.5f, pos.y + 0.5f)) == 3) { Died(); }


        //water check
        if (levelManager.getTile(new Vector2(pos.x + 0.5f, pos.y)) == 2)
        {
            inWater = true;
            //damping
            vel.x *= Mathf.Pow(0.1f, Time.deltaTime);
            vel.y *= Mathf.Pow(0.1f, Time.deltaTime);

            vel.x = Mathf.Clamp(vel.x, -1.4f, 1.4f);
            vel.y = Mathf.Clamp(vel.y, -20.0f, 20.0f);

            //depth estimate
            float depth = WaterDepth();


            vel.y -= gravity * depth * 2.0f * Time.fixedDeltaTime;
        }else { inWater = false; }

        //item check
        if (levelManager.getTile(new Vector2(pos.x + 0.5f, pos.y+0.5f)) == 5)
        {
            getItem(new Vector2Int((int)pos.x , (int)pos.y));
        }
    }

    void getItem(Vector2Int pos)
    {
        bool hasBeen = false;

        for (int i = 0; i < itemtHits.Count; i++)
        {
            if (Vector2.Distance(itemtHits[i], pos) < 5)
            {
                hasBeen = true;
            }
        }

        if (!hasBeen)
        {
            itemtHits.Add(pos);

            float rand = Random.Range(0.0f, 1.0f);
            if (rand < 0.2f)
            {
                items.Add(new Item(0, 0, 15, 2, normalJumpHeight, normalSpeed));
                itemUIManager.addItem( items.Count - 1);
                Debug.Log("double jump");
            }
            else if (rand < 0.4f)
            {

                items.Add(new Item(0, 1, 15, 1, normalJumpHeight, normalSpeed * 1.5f));
                itemUIManager.addItem( items.Count - 1);
                Debug.Log("faster");
            }
            else if (rand < 0.6f)
            {
                items.Add(new Item(0, 2, 15, 1, normalJumpHeight * 1.5f, normalSpeed));
                itemUIManager.addItem( items.Count - 1);
                Debug.Log("higher jump");
            }
            else if (rand < 0.70f)
            {
                otherPlayer.items.Add(new Item(0, 3, 5, 0, normalJumpHeight, normalSpeed));
                otherPlayer.itemUIManager.addItem(otherPlayer.items.Count - 1);
                Debug.Log("other player cant jump");
            }
            else if (rand < 0.85f)
            {
                otherPlayer.items.Add(new Item(0, 4, 7, 1, normalJumpHeight, normalSpeed * 0.5f));
                otherPlayer.itemUIManager.addItem(otherPlayer.items.Count - 1);
                Debug.Log("other player becomes slow");
            }
            else
            {
                otherPlayer.items.Add(new Item(0, 5, 7, 1, normalJumpHeight * 0.75f, normalSpeed));
                otherPlayer.itemUIManager.addItem(otherPlayer.items.Count - 1);
                Debug.Log("other player cant jump as high");
            }
           

        }
    }

    void itemUpdate() 
    {
        speed = normalSpeed;
        maxJumps = 1;
        jumpHeight = normalJumpHeight;
        
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].effectTimer > 0)
            {
                if (items[i].type == 0)
                {
                    speed = items[i].speed;
                    maxJumps = items[i].maxJumps;
                    jumpHeight= items[i].jumpHeight;
                    items[i].effectTimer -= Time.deltaTime;
                }
            }
            else {
                itemUIManager.removeItem(i);
                items.RemoveAt(i);
                i--;
            }
        }
    }

    void checkPointCheck(Vector2Int newCheckPointPos) 
    {
        bool hasBeen = false;

        for(int i=0;i<checkPointHits.Count;i++)
        {
            if (Vector2.Distance(checkPointHits[i],newCheckPointPos)<5) 
            {
                hasBeen = true;
            }
        }

        if (!hasBeen)
        {
            checkPointPos = newCheckPointPos + new Vector2(0, 0.5f);
            checkPointHits.Add(checkPointPos);
            levelManager.getLevel(newCheckPointPos).checkPointHits++;
        }
    }

    float WaterDepth()
    {
        if (levelManager.getTile(new Vector2(pos.x + 0.5f, pos.y)) == 2)
        {
            //depth estimate
            float depth = 0;
            for (int i = 0; i < 10; i++)
            {
                if (levelManager.getTile(new Vector2(pos.x + 0.5f, pos.y + Mathf.Pow(1.5f, i) * 0.1f)) == 2)
                {
                    depth = Mathf.Pow(1.5f, i) * 0.1f;
                }
                else
                {
                    depth = (Mathf.Pow(1.5f, i - 1) * 0.1f + Mathf.Pow(1.5f, i) * 0.1f) / 2.0f;
                    i = 11;
                }
            }
            return depth;
        }
        else { return 0; }
    }

    void Died()
    {
       pos = checkPointPos;
       vel = new Vector2(0, 0);
       maxJumps = 1;
       jumpHeight = 4.5f;
    }


    void updateAnimations()
    {
        AnimationFrameCountdown -= Time.deltaTime;
        if(AnimationFrameCountdown < 0 ) 
        {
            AnimationFrameCountdown = 1.0f/AnimationFrameRate;
            AnimationFrameCount++;
            AnimationFrameCount = AnimationFrameCount % 4;
        }
        if (Mathf.Abs(vel.x) > 0.25f)
        {
            if (grounded || inWater)
            {
                spriteRenderer.sprite = playerSprites[AnimationFrameCount];
            }
            else {
                spriteRenderer.sprite = playerSprites[1];
            }
        }
        else 
        {
            AnimationFrameCountdown = 1.0f / AnimationFrameRate;
            AnimationFrameCount = 0;
            spriteRenderer.sprite = playerSprites[4];
        }
        spriteRenderer.flipX = !facing;
    }
    


}
