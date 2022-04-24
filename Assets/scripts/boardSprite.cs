using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardSprite : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] BoardMgr boardMgr;
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SR { get => spriteRenderer; set => spriteRenderer = value; }

    //    [SerializeField]  gm;
    // Start is called before the first frame update

    private void OnMouseDown()
    {
        boardMgr.OnSpriteClicked(id);
    }

    public void init(int id)
    {
        this.id = id;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
