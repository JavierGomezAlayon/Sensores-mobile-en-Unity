using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class Ejercicio2 : MonoBehaviour
{
    [Header("Movimiento")]
    public float speedMultiplier = 3f;
    public float rotationSpeed = 5f; // velocidad de Slerp

    [Header("Rango geográfico permitido")]
    public float minLatitude = 28.0f;
    public float maxLatitude = 28.8f;
    public float minLongitude = -16.6f;
    public float maxLongitude = -16.0f;

    private Accelerometer accelerometer;
    public float aceleración;

    void Start()
    {
        // Activar GPS
        Input.location.Start();

        // Obtener acelerómetro y habilitarlo
        accelerometer = Accelerometer.current;
        if (accelerometer != null)
            InputSystem.EnableDevice(accelerometer);
    }

    void Update()
    {
        // 1. Comprobar si estamos dentro del rango lat/long
        // if (!IsInsideAllowedArea())
        // {
        //     return; // no mover ni rotar
        // }

        // 2. Rotación hacia el norte con Slerp
        OrientToNorth();

        // 3. Movimiento según acelerómetro
        MoveByAcceleration();
    }

    bool IsInsideAllowedArea()
    {
        if (Input.location.status != LocationServiceStatus.Running)
            return false;

        var lat = Input.location.lastData.latitude;
        var lon = Input.location.lastData.longitude;

        return lat >= minLatitude && lat <= maxLatitude &&
               lon >= minLongitude && lon <= maxLongitude;
    }

    void OrientToNorth()
    {
        // Dirección Norte en el plano
        Vector3 north = new Vector3(0, 0, 1);

        // Rotación deseada
        Quaternion targetRotation = Quaternion.LookRotation(north, Vector3.up);

        // Aplicar Slerp suave
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void MoveByAcceleration()
    {
        // Seguridad: `accelerometer` puede ser null y `ReadValue()` devuelve un Vector3 (no-null),
        // por eso no podemos usar `??` directamente. Usamos un fallback explícito.
        Vector3 accel = Vector3.zero;
        if (accelerometer != null)
        {
            accel = accelerometer.acceleration.ReadValue();
        }
        else if (Accelerometer.current != null)
        {
            accel = Accelerometer.current.acceleration.ReadValue();
        }

        if (accel == Vector3.zero)
            accel.z = aceleración;

        Debug.Log("Aceleración Z: " + accel.z);
        if (accel.z > 0.3f || accel.z < -0.3f)
        {
        // Queremos usar el eje Z → movimiento hacia adelante/atrás
        float forward = -accel.z; // invertido por la orientación del dispositivo

        // Queremos que coincida con la postura Horizontal Izquierda
        // Rotamos el vector para ajustarlo a cómo se sostiene el móvil
        Vector3 corrected = new Vector3(0, 0, forward);

        // Movimiento final
        transform.Translate(corrected * speedMultiplier * Time.deltaTime, Space.Self);
        }
    }

    private void OnDisable()
    {
        if (accelerometer != null)
            InputSystem.DisableDevice(accelerometer);
    }
}

