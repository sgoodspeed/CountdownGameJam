using UnityEngine;

namespace Countdown
{
    public class GameHudPresenter : MonoBehaviour
    {
        [SerializeField] private GameHudModel model;
        [SerializeField] private GameHudView view;

        public void Show()
        {
            gameObject.SetActive(true);
            GameState.Instance.GameClock.Start(model.GameDuration);
        }

        public void Hide() => gameObject.SetActive(false);

        private void Update()
        {
            view.SetTime(GameState.Instance.GameClock.RemainingSeconds);
        }
    }
}
