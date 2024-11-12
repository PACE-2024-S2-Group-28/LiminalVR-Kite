using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeTag : MonoBehaviour
{
    new public Tag tag = Tag.Default;

    public static Tag[] AllAsteroidTags = new Tag[]{Tag.Asteroid, Tag.GoldAsteroid};
    public static bool CheckTag(GameObject g, Tag checkingTag)
    {
        FakeTag T = g.GetComponent<FakeTag>();
        return T != null && T.tag == checkingTag;
    }

    public static bool CheckTags(GameObject g, Tag[] tags)
    {
        FakeTag T = g.GetComponent<FakeTag>();
        if(T == null){
            Debug.LogWarning("Attempted a tag check on an object with no tag attached!");
            return false;
        }  
        foreach(Tag t in tags){
            if(T.tag == t){
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public enum Tag
{
    Default,
    Bullet,
    Asteroid,
    GoldAsteroid,
    AsteroidPieces,
    Forcefield,
    Ship
};

