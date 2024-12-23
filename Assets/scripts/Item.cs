using UnityEngine;


public class Item
{
    public int type; //0 affects stats 1 throwable
    public float effectTimer; //can be used for how long an affect lasts or for if the throwable has been used yet
    public int maxJumps;
    public float jumpHeight;
    public float speed;

    public Item(int type,float effectTimer,int maxJumps,float jumpHeight,float speed)
    {
        this.type = type;
        this.effectTimer = effectTimer;
        this.maxJumps = maxJumps;
        this.jumpHeight = jumpHeight;  
        this.speed = speed;
    }
    
}
