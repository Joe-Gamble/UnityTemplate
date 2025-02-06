using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    private Vector3 moveDir = Vector3.zero;

    [SerializeField] private int speed;

    private void Awake()
    {
        inputReader.Jump += OnJump;
        inputReader.Look += OnLook;

        inputReader.EnablePlayerActions();
    }

    private void OnLook(Vector2 mouseDir)
    {
        Debug.Log(mouseDir);
    }

    private void Update()
    {
        moveDir = new Vector3(inputReader.MoveDirection.x, 0, inputReader.MoveDirection.y);

        transform.position += moveDir * Time.deltaTime * speed;
    }

    private void OnJump(bool jumped)
    {
        Debug.Log(jumped);
    }
}
