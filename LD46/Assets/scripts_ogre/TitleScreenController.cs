using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{

    public GameObject audio_manager_ref;

        public void refreshAudioManager()
    {
        if (audio_manager_ref==null)
        {
            audio_manager_ref = GameObject.Find(Constants.AUDIO_MANAGER_GO_NAME);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // INIT GAME IF NEEDED
        refreshAudioManager();
        if (!!audio_manager_ref)
        {
            AudioManager am = audio_manager_ref.GetComponent<AudioManager>();
            string sound_name_to_play = Constants.TITLE_THEME_BGM;
            am.Play(sound_name_to_play);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool spacePressed = Input.anyKey;
        if (spacePressed)
            SceneManager.LoadScene(Constants.MAIN_GAME_SCENE, LoadSceneMode.Single);

    }


}
