using UnityEngine;

namespace Countdown
{
    public class GameHudPresenter : MonoBehaviour
    {
        [SerializeField] private GameHudModel model;
        [SerializeField] private GameHudView view;
        
        [SerializeField] private AudioClip bgm;

        public void Show()
        {
            gameObject.SetActive(true);
            GameState.Instance.GameClock.Start(model.GameDuration);
            
            if(bgm != null)
                SoundManager.Instance.PlayMusic(bgm);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            if(bgm != null)
                SoundManager.Instance.StopMusic();
        }

        private void Update()
        {
            view.SetTime(GameState.Instance.GameClock.RemainingSeconds);
        }
    }
}
