using DG.Tweening;
using UnityEngine;

public class Ball : MonoBehaviour, ISubscriber
{
    [SerializeField] private GameObject _particlePrefab;

    [SerializeField] private Vector2[] _directions;

    private int _currentDirection;

    private float _speed = 1.8f;


    private void FixedUpdate()
    {
        if (GameState.Instance.CurrentState != GameState.State.InGame)
            return;

        transform.position += new Vector3(_directions[_currentDirection].x * _speed * Time.fixedDeltaTime, _directions[_currentDirection].y * _speed * Time.fixedDeltaTime, 0);
    }
    public void SubscribeAll()
    {
        PlayerInput.Instance.PlayerMouseDown += OnPlayerMouseDown;
        GameState.Instance.ScoreAdded += CheckMySpeed;
    }
    public void UnsubscribeAll()
    {
        PlayerInput.Instance.PlayerMouseDown -= OnPlayerMouseDown;
        GameState.Instance.ScoreAdded -= CheckMySpeed;
    }
    private void SpawnParticle()
    {
        var particle = Instantiate(_particlePrefab).GetComponent<ParticleSystem>();

        particle.transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
        particle.Play();

        Destroy(particle.gameObject, 2f);
    }
    private void CheckMySpeed()
    {
        if (_speed > 8)
            return;

        _speed += 0.25f;
    }
    private void RotateBall()
    {
        _currentDirection++;

        if (_currentDirection >= _directions.Length)
            _currentDirection = 0;
    }
    private void OnPlayerMouseDown()
    {
        RotateBall();
    }
    private void DestroyBall()
    {

        GameState.Instance.FinishGame();
        AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.Win, 1f);

        transform.DOScale(0, 0.1f).SetLink(gameObject);

        SpawnParticle();
        Destroy(gameObject, 0.2f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.ScoreAdd, 1f);
            PlayerScore.Instance.AddScore();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            DestroyBall();
        }
    }
}
