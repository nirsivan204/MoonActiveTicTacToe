using UnityEngine;
public class ManusMGR : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isPVP = false;
    private string bundleName;
    [SerializeField] GameObject manuGUI;
    [SerializeField] GameObject gameGUI;
    [SerializeField] gameMgr GM;


    public void startGame()
    {
        manuGUI.SetActive(false);
        gameGUI.SetActive(true);
        GM.IsPVP = isPVP;
        GM.StartGame();
    }

    public void OnPVPToggle()
    {
        isPVP = !isPVP;
    }

    public void GetAssetBundleName(string input)
    {
        bundleName = input;
    }

    public void reskin()
    {
        Texture2D[] loadedTextures = AssetBundleUtility.loadAssetBundle(bundleName);
        for (int i = 0; i<3; i++)
        {
            GM.Imgs[i] = Sprite.Create(loadedTextures[i], new Rect(0.0f, 0.0f, loadedTextures[i].width, loadedTextures[i].height), new Vector2(0.5f, 0.5f));
        }
    }
}
