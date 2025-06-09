using UnityEngine;
using UnityEngine.UI; // Para Slider, Toggle
using UnityEngine.Audio; // Para AudioMixer (opcional pero recomendado)
using System.Collections.Generic; // Para List (usado en resoluciones)
using System.Linq; // Para operaciones LINQ (usado en resoluciones)
using TMPro; // Si usas TextMeshPro para el Dropdown

public class OptionsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider masterVolumeSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown; // Usa TMP_Dropdown si es TextMeshPro

    [Header("Audio Mixer (Opcional)")]
    // Crea un Audio Mixer (Window > Audio > Audio Mixer)
    // En el grupo Master, haz clic derecho en "Volume" -> "Expose 'Volume (of Master)' to script"
    // Luego, en la ventana del Audio Mixer (arriba a la derecha), renombra el parámetro expuesto a "MasterVolumeParam"
    public AudioMixer masterAudioMixer;
    public string masterVolumeParameterName = "MasterVolumeParam"; // Nombre del parámetro expuesto

    private Resolution[] allResolutions;
    private List<Resolution> filteredResolutions; // Para guardar resoluciones únicas con su tasa de refresco

    // Claves para PlayerPrefs (para guardar y cargar)
    private const string KEY_MASTER_VOLUME = "MasterVolume";
    private const string KEY_FULLSCREEN = "FullscreenEnabled";
    private const string KEY_RESOLUTION_INDEX = "ResolutionIndex";
    // private const string KEY_REFRESH_RATE = "RefreshRate"; // Podrías guardar esto también


    void Awake() // Awake se ejecuta antes que Start, incluso si el GameObject está inactivo y se activa luego
    {
        SetupResolutionDropdown();
        LoadSettings(); // Cargar configuraciones al iniciar/activar
    }

    void OnEnable() // Se llama cuando el panel se activa
    {
        // Asegurarse de que los controles UI reflejen los valores actuales al mostrar el panel
        // LoadSettings ya lo hace, pero una actualización explícita podría ser útil si los valores cambian fuera.
        RefreshUIControls();
    }


    void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        allResolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        // Filtrar para obtener resoluciones únicas con tasas de refresco comunes o la actual
        // Esto evita listas enormes si hay muchas tasas de refresco por resolución
        for (int i = 0; i < allResolutions.Length; i++)
        {
            // Puedes añadir un filtro más estricto aquí si quieres (ej. solo 16:9, o ciertas tasas de refresco)
            string optionString = $"{allResolutions[i].width} x {allResolutions[i].height} @ {allResolutions[i].refreshRateRatio.value:F0} Hz";
            if (!options.Contains(optionString)) // Evitar duplicados exactos
            {
                options.Add(optionString);
                filteredResolutions.Add(allResolutions[i]);
            }
        }

        resolutionDropdown.AddOptions(options);
    }


    public void SetMasterVolume(float volume)
    {
        if (masterAudioMixer != null && !string.IsNullOrEmpty(masterVolumeParameterName))
        {
            // El AudioMixer usa una escala logarítmica (dB). -80 es silencio, 0 es volumen completo.
            // Un slider de 0 a 1 necesita convertirse.
            float dB = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 20f;
            masterAudioMixer.SetFloat(masterVolumeParameterName, dB);
        }
        else
        {
            AudioListener.volume = volume; // Controla el volumen maestro global si no hay mixer
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolutionFromDropdown(int resolutionIndex)
    {
        if (filteredResolutions == null || resolutionIndex < 0 || resolutionIndex >= filteredResolutions.Count)
            return;

        Resolution selectedResolution = filteredResolutions[resolutionIndex];
        // Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen, selectedResolution.refreshRateRatio);
        // La línea de arriba es la más moderna. Si da problemas o usas Unity más antiguo:
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen, (int)selectedResolution.refreshRateRatio.value);

    }

    public void ApplyAndSaveChanges()
    {
        // Guardar volumen
        if (masterVolumeSlider != null)
            PlayerPrefs.SetFloat(KEY_MASTER_VOLUME, masterVolumeSlider.value);

        // Guardar pantalla completa
        if (fullscreenToggle != null)
            PlayerPrefs.SetInt(KEY_FULLSCREEN, fullscreenToggle.isOn ? 1 : 0);

        // Guardar resolución (guardamos el índice de nuestra lista filtrada)
        if (resolutionDropdown != null && filteredResolutions != null && resolutionDropdown.value < filteredResolutions.Count)
        {
            PlayerPrefs.SetInt(KEY_RESOLUTION_INDEX, resolutionDropdown.value);
        }

        PlayerPrefs.Save(); // Escribir los cambios a disco
        Debug.Log("Configuración guardada y aplicada.");

        // Opcional: Aplicar de nuevo por si acaso, aunque los listeners ya deberían haberlo hecho
        // SetMasterVolume(masterVolumeSlider.value);
        // SetFullscreen(fullscreenToggle.isOn);
        // SetResolutionFromDropdown(resolutionDropdown.value);
    }

    public void LoadSettings()
    {
        // Cargar y aplicar volumen
        if (masterVolumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat(KEY_MASTER_VOLUME, 0.75f); // Default a 75%
            masterVolumeSlider.value = savedVolume; // Actualizar el slider
            SetMasterVolume(savedVolume);       // Aplicar el volumen
        }

        // Cargar y aplicar pantalla completa
        if (fullscreenToggle != null)
        {
            // Default a pantalla completa si es la primera vez o no hay guardado
            bool isFullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;
            fullscreenToggle.isOn = isFullscreen; // Actualizar el toggle
            SetFullscreen(isFullscreen);        // Aplicar
        }

        // Cargar y aplicar resolución
        if (resolutionDropdown != null && filteredResolutions != null && filteredResolutions.Count > 0)
        {
            int savedResolutionIndex = PlayerPrefs.GetInt(KEY_RESOLUTION_INDEX, -1);
            int currentActualResIndex = -1;

            // Intentar encontrar un índice que coincida con la resolución actual de la pantalla
            for (int i = 0; i < filteredResolutions.Count; i++)
            {
                if (filteredResolutions[i].width == Screen.currentResolution.width &&
                   filteredResolutions[i].height == Screen.currentResolution.height &&
                   Mathf.Approximately((float)filteredResolutions[i].refreshRateRatio.value, (float)Screen.currentResolution.refreshRateRatio.value))
                {
                    currentActualResIndex = i;
                    break;
                }
            }

            if (savedResolutionIndex != -1 && savedResolutionIndex < filteredResolutions.Count) // Si hay un índice guardado válido
            {
                resolutionDropdown.value = savedResolutionIndex;
            }
            else if (currentActualResIndex != -1) // Sino, usar el índice de la resolución actual
            {
                resolutionDropdown.value = currentActualResIndex;
            }
            else // Sino, usar la última de la lista (normalmente la más alta)
            {
                resolutionDropdown.value = filteredResolutions.Count - 1;
            }

            resolutionDropdown.RefreshShownValue(); // Actualizar el texto visible del dropdown
            SetResolutionFromDropdown(resolutionDropdown.value); // Aplicar la resolución
        }
        Debug.Log("Configuración cargada.");
    }

    public void RefreshUIControls() // Llama a esto si quieres forzar la UI a actualizarse con los valores actuales
    {
        if (masterVolumeSlider != null) masterVolumeSlider.value = PlayerPrefs.GetFloat(KEY_MASTER_VOLUME, 0.75f);
        if (fullscreenToggle != null) fullscreenToggle.isOn = PlayerPrefs.GetInt(KEY_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;
        if (resolutionDropdown != null && filteredResolutions != null && filteredResolutions.Count > 0)
        {
            int idx = PlayerPrefs.GetInt(KEY_RESOLUTION_INDEX, -1);
            // Lógica similar a LoadSettings para encontrar el índice correcto
            if (idx != -1 && idx < filteredResolutions.Count) resolutionDropdown.value = idx;
            else
            { /* default a actual o la más alta */
                int currentActualResIndex = -1;
                for (int i = 0; i < filteredResolutions.Count; i++) { /* ... encontrar actual ... */ if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height) currentActualResIndex = i; }
                if (currentActualResIndex != -1) resolutionDropdown.value = currentActualResIndex;
                else if (filteredResolutions.Count > 0) resolutionDropdown.value = filteredResolutions.Count - 1;
            }
            resolutionDropdown.RefreshShownValue();
        }
    }


    public void ClosePanelAndSaveChanges()
    {
        ApplyAndSaveChanges(); // Guardar cambios al salir
        gameObject.SetActive(false); // Ocultar este panel de opciones

        // El manager que abrió este panel (MainMenuManager o PauseMenuManager)
        // necesitará reactivar su propio panel principal.
        // Ver Paso 4 para cómo manejar esto.
    }
    
    
}