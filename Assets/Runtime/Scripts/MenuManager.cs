using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button _playGamebtn;
    void Start()
    {
        if(_playGamebtn != null)
        {
            _playGamebtn.onClick.AddListener(() => loadGameplayScene());
        }
    }
    
    #region handle on click
    private void loadGameplayScene()
    {
        SceneManager.LoadScene("Gameplay");
    }
    #endregion
}
