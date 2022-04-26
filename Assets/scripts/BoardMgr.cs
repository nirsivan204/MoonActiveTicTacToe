using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMgr : MonoBehaviour
{
    [SerializeField] gameMgr gm;
    [SerializeField] GameObject boardGraphics;
    [SerializeField] GameObject boardClickables;
    [SerializeField] boardSprite[] sprites; //must be ordered like: 0 1 2
                                            //                      3 4 5
                                            //                      6 7 8
    public void OnSpriteClicked(int id)
    {
        gm.ChooseBoardPlace(id);
    }

    public void DrawSpriteInPlace(int id, Sprite img)
    {
        if (sprites!=null)
        {
            sprites[id].SR.sprite = img;
        }
    }

    public void Init()
    {
        int i = 0;
        foreach (boardSprite BS in sprites)
        {
            BS.Init(i++);
        }
        boardGraphics.SetActive(true);
        boardClickables.SetActive(true);
    }
}
