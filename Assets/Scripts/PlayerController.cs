using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Скорость движения персонажа
    public float MovementSpeed = 2.0f;
    public float SprintSpeed = 4.0f;

    private GameManager _GameManager;

    // Сила прыжка персонажа
    public float JumpForce = 5.0f;

    public float RotationSmoothing = 20f;

    public GameObject HandMeshes;

    public GameObject[] WeaponInventory;

    public GameObject[] WeaponMeshes;

    private int SelectedWeaponId = 0;

    private Weapon _Weapon;

    private float pitch, yaw;


    private Rigidbody Rigidbody;


    private bool IsGrounded;


    public float DistanceToGround = 0.1f;


    void Start()
    {
 
        Rigidbody = GetComponent<Rigidbody>();
        _GameManager = FindObjectOfType<GameManager>();

    
        for (int i = 0; i < WeaponInventory.Length; i++)
        {
            Weapon weapon = WeaponInventory[i].GetComponent<Weapon>();

            if (i == 2)
            {
                weapon.isUnlocked = false; 
            }
            else
            {
                weapon.isUnlocked = true; 
            }
        }

      
        _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();

        WeaponMeshes[SelectedWeaponId].SetActive(true);
    }

    void FixedUpdate()
    {
        GroundCheck();

        if (Input.GetKey(KeyCode.Mouse0)) _Weapon.Fire();
        if (Input.GetKey(KeyCode.R)) _Weapon.Reload();
        if (Input.GetAxis("Mouse ScrollWheel") > 0) SelectedNextWeapon();
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) SelectPrevWeapon();


        Rigidbody.MovePosition(CalculateMovement());

        if (Input.GetKey(KeyCode.LeftShift) && !_GameManager.IsStaminaRestoring)
        {
            _GameManager.SpendStamina();
            Rigidbody.MovePosition(CalculateSprint());
        }
        else
        {
            Rigidbody.MovePosition(CalculateMovement());
        }

        SetRotation();
    }

    void Update()
    {
        // Проверяем, нажал ли игрок пробел и находится ли персонаж на земле
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            Jump();
        }
    }

    private Vector3 CalculateMovement()
    {
        float HorizontalDirection = Input.GetAxis("Horizontal");
        float VerticalDirection = Input.GetAxis("Vertical");

        // Направление камеры
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Очищаем вертикальный компонент направления
        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 Move = cameraRight * HorizontalDirection + cameraForward * VerticalDirection;

        return Rigidbody.transform.position + Move * Time.fixedDeltaTime * MovementSpeed;
    }

    private Vector3 CalculateSprint()
    {
        float HorizontalDirection = Input.GetAxis("Horizontal");
        float VerticalDirection = Input.GetAxis("Vertical");

        // Направление камеры
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Очищаем вертикальный компонент направления
        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 Move = cameraRight * HorizontalDirection + cameraForward * VerticalDirection;

        return Rigidbody.transform.position + Move * Time.fixedDeltaTime * SprintSpeed;
    }

    private void Jump()
    {
        // Создание силы, направленной вверх для прыжка
        Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    private void GroundCheck()
    {
        // Отправка луча, который при соприкосновении с поверхностью вернет значение true
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, DistanceToGround);
    }

    // Отрисовка вспомогательных элементов
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // Рисуем линию, которая накладывается с лучом, чтобы убедиться, что проверка нахождения на земле проходит верно
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * DistanceToGround));
    }

    public void SetRotation()
    {
        yaw += Input.GetAxis("Mouse X");
        pitch -= Input.GetAxis("Mouse Y");

        pitch = Mathf.Clamp(pitch, -60, 90);

        Quaternion SmoothRotation = Quaternion.Euler(pitch, yaw, 0);

        HandMeshes.transform.rotation = Quaternion.Slerp(HandMeshes.transform.rotation,
            SmoothRotation, RotationSmoothing * Time.fixedDeltaTime);

        SmoothRotation = Quaternion.Euler(0, yaw, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, SmoothRotation, RotationSmoothing * Time.fixedDeltaTime);
    }

    private void SelectPrevWeapon()
    {
        if (SelectedWeaponId != 0)
        {
            WeaponMeshes[SelectedWeaponId].SetActive(false);
            SelectedWeaponId -= 1;

            Weapon _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();

            if (_Weapon != null && _Weapon.isUnlocked)
            {
                WeaponMeshes[SelectedWeaponId].SetActive(true);
                Debug.Log("Оружие " + _Weapon.WeaponType);
            }
            else
            {
                Debug.Log("Это оружие заблокировано.");
                SelectPrevWeapon(); 
            }
        }
    }

    private void SelectedNextWeapon()
    {
        if (WeaponInventory.Length > SelectedWeaponId + 1)
        {
            WeaponMeshes[SelectedWeaponId].SetActive(false);
            SelectedWeaponId += 1;

            _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();

            if (_Weapon.isUnlocked)
            {
                WeaponMeshes[SelectedWeaponId].SetActive(true);
                Debug.Log("Оружие " + _Weapon.WeaponType);
            }
            else
            {
                Debug.Log("Это оружие заблокировано.");
                SelectedNextWeapon(); 
            }
        }
    }

   
    public void UnlockWeapon(int weaponId)
    {
        if (weaponId >= 0 && weaponId < WeaponInventory.Length)
        {
            WeaponInventory[weaponId].GetComponent<Weapon>().isUnlocked = true;
            Debug.Log("Оружие с ID " + weaponId + " разблокировано");
        }
        else
        {
            Debug.LogWarning("Неверный ID оружия");
        }
    }
}
