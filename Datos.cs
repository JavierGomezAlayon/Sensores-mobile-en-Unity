using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SensorDisplay : MonoBehaviour
{
    public TextMeshProUGUI textUI;

    IEnumerator Start()
    {
        // Habilitar todos los sensores disponibles (si están deshabilitados)
        foreach (var device in InputSystem.devices)
        {
            Debug.Log($"Device: {device.displayName}, Enabled: {device.enabled}");
            if (!device.enabled)
                InputSystem.EnableDevice(device);
        }

        // ---------- GPS / Location Service ----------
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS desactivado por el usuario");
            yield break;
        }

        Input.location.Start(2f, 0f); // (precision en metros, distancia mínima)

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            Debug.Log("GPS: timeout");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("No se pudo obtener la localización");
            yield break;
        }

        Debug.Log("GPS iniciado correctamente");
    }

    void OnDisable()
    {
        // Deshabilitar sensores cuando ya no se usen
        foreach (var device in InputSystem.devices)
        {
            if (device.enabled)
                InputSystem.DisableDevice(device);
        }

        // Apagar GPS cuando no se use
        if (Input.location.isEnabledByUser)
            Input.location.Stop();
    }

    void Update()
    {
        if (textUI == null) return;

        string texto = "";

        // Acelerómetro
        if (UnityEngine.InputSystem.Accelerometer.current != null)
            texto += $"Accelerometer: {UnityEngine.InputSystem.Accelerometer.current.acceleration.ReadValue()}\n";

        // Giroscopio
        if (UnityEngine.InputSystem.Gyroscope.current != null)
            texto += $"Gyroscope: {UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue()}\n";

        // Gravedad
        if (UnityEngine.InputSystem.GravitySensor.current != null)
            texto += $"Gravity: {UnityEngine.InputSystem.GravitySensor.current.gravity.ReadValue()}\n";

        // Orientación
        if (UnityEngine.InputSystem.AttitudeSensor.current != null)
            texto += $"Attitude (Quat): {UnityEngine.InputSystem.AttitudeSensor.current.attitude.ReadValue()}\n";

        // Aceleración lineal
        if (UnityEngine.InputSystem.LinearAccelerationSensor.current != null)
            texto += $"Linear Acceleration: {UnityEngine.InputSystem.LinearAccelerationSensor.current.acceleration.ReadValue()}\n";

        // Campo magnético
        if (UnityEngine.InputSystem.MagneticFieldSensor.current != null)
            texto += $"Magnetic Field: {UnityEngine.InputSystem.MagneticFieldSensor.current.magneticField.ReadValue()}\n";

        // Luz
        if (UnityEngine.InputSystem.LightSensor.current != null)
            texto += $"Light: {UnityEngine.InputSystem.LightSensor.current.lightLevel.ReadValue()}\n";

        // Proximidad
        if (UnityEngine.InputSystem.ProximitySensor.current != null)
            texto += $"Proximity: {UnityEngine.InputSystem.ProximitySensor.current.distance.ReadValue()} cm\n";

        // Presión
        if (UnityEngine.InputSystem.PressureSensor.current != null)
            texto += $"Pressure: {UnityEngine.InputSystem.PressureSensor.current.atmosphericPressure.ReadValue()} hPa\n";

        // Temperatura
        if (UnityEngine.InputSystem.AmbientTemperatureSensor.current != null)
            texto += $"Temp: {UnityEngine.InputSystem.AmbientTemperatureSensor.current.ambientTemperature.ReadValue()} °C\n";

        // Humedad
        if (UnityEngine.InputSystem.HumiditySensor.current != null)
            texto += $"Humidity: {UnityEngine.InputSystem.HumiditySensor.current.relativeHumidity.ReadValue()} %\n";

        // Pasos
        if (UnityEngine.InputSystem.StepCounter.current != null)
            texto += $"Steps: {UnityEngine.InputSystem.StepCounter.current.stepCounter.ReadValue()}\n";

        // ---------- GPS ----------
        if (Input.location.status == LocationServiceStatus.Running)
        {
            var data = Input.location.lastData;

            texto += "\n--- GPS ---\n";
            texto += $"Latitud: {data.latitude}\n";
            texto += $"Longitud: {data.longitude}\n";
            texto += $"Altitud: {data.altitude} m\n";
            texto += $"Precisión: {data.horizontalAccuracy} m\n";
            texto += $"Timestamp: {data.timestamp}\n";
        } 
        else
        {
            texto += "\nGPS: No disponible\n";
        }

        textUI.text = texto;
    }
}
