using UnityEngine;


public class Item
{
    public int type; //0 affects stats 1 throwable
    public float timeLeft; //can be used for how long an affect lasts or for if the throwable has been used yet
    public float jumps;


    Item(int type)
    {
        this.type = type;
    }

}
