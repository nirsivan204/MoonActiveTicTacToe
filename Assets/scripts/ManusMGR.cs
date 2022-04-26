using UnityEngine;
using UnityEngine.UI;

public class ManusMGR : MonoBehaviour
{
    private bool isPVP = false;
    private string bundleName;
    private int COMDifficulty = 0;
    [SerializeField] GameObject manuGUI;
    [SerializeField] GameObject gameGUI;
    [SerializeField] gameMgr GM;
    [SerializeField] public Dropdown difficultyDropdown;

    public void Awake()
    {
        difficultyDropdown.onValueChanged.AddListener(delegate { ChooseDifficulty(difficultyDropdown); });
    }
    public void StartGame()
    {
        manuGUI.SetActive(false);
        gameGUI.SetActive(true);
        GM.IsPVP = isPVP;
        GM.COMDifficulty = COMDifficulty;
        GM.StartGame();
    }

    public void OnPVPToggle()
    {
        isPVP = !isPVP;
        difficultyDropdown.gameObject.SetActive(!isPVP);
    }

    public void GetAssetBundleName(string input)
    {
        bundleName = input;
    }

    public void Reskin()
    {
        Texture2D[] loadedTextures = AssetBundleLoader.LoadAssetBundle(bundleName);
        if (loadedTextures != null)
        {
            for (int i = 0; i < 3; i++)
            {
                GM.Imgs[i] = Sprite.Create(loadedTextures[i], new Rect(0.0f, 0.0f, loadedTextures[i].width, loadedTextures[i].height), new Vector2(0.5f, 0.5f));
            }
        }
    }

    public void ChooseDifficulty(Dropdown choise)
    {
        COMDifficulty = choise.value;
        print(choise.value);
    }
}
