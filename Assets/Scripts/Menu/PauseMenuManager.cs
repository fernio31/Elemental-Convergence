using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public static bool isGamePaused = false;

    // Nombre de tu escena del menú principal
    public string mainMenuSceneName = "MainMenu";
    public GameObject mainPanelForThisMenu;


    public GameObject optionsPanel;

    void Start()
    {
        // Asegurarse de que el panel de pausa esté oculto al empezar
        // y que el juego no esté pausado.
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        ResumeGameTime(); // Asegura que el tiempo corra normal al inicio
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        ResumeGameTime();
        Debug.Log("Juego Reanudado.");

    }

    void Pause()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        PauseGameTime();
        Debug.Log("Juego Pausado.");

    }

    public void LoadMainMenu()
    {
        ResumeGameTime(); // ¡MUY IMPORTANTE! Restaurar el tiempo antes de cambiar de escena
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenOptions_Pause()
    {
        Debug.Log("Botón Opciones presionado.");
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            // Opcional: Ocultar el panel principal de este menú
            if (mainPanelForThisMenu != null) mainPanelForThisMenu.SetActive(false);

            // Opcional: Forzar la actualización de los controles del panel de opciones
            OptionsManager om = optionsPanel.GetComponent<OptionsManager>();
            if (om != null) om.RefreshUIControls(); // O om.LoadSettings(); si quieres recargar siempre
        }
    }

    public void QuitGame_Pause()
    {
        ResumeGameTime(); // Restaurar tiempo por si acaso
        Debug.Log("Salir del Juego desde Menú de Pausa.");
        // Si estás en el editor de Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        // Si es una build del juego
#else
        Application.Quit();
#endif
    }

    private void PauseGameTime()
    {
        Time.timeScale = 0f; // Congela el tiempo del juego
        isGamePaused = true;
    }

    private void ResumeGameTime()
    {
        Time.timeScale = 1f; // Restaura el tiempo del juego a normal
        isGamePaused = false;
    }

    // Es buena práctica asegurarse de que el tiempo se restaura si este objeto se destruye inesperadamente
    // o al cambiar de escena si no se llamó a LoadMainMenu() que ya lo hace.
    void OnDestroy()
    {
        if (Time.timeScale == 0f && isGamePaused) // Solo si este script fue el que pausó
        {
            ResumeGameTime();
        }
    }
    
    public void CloseOptionsPanelAndReturn()
    {
        if (optionsPanel != null)
        {
            // Primero, decirle al OptionsManager que guarde si es necesario
            OptionsManager om = optionsPanel.GetComponent<OptionsManager>();
            if (om != null)
            {
                om.ApplyAndSaveChanges(); // Guardar al volver
            }
            optionsPanel.SetActive(false); // Ocultar panel de opciones
        }

        if (mainPanelForThisMenu != null)
        {
            mainPanelForThisMenu.SetActive(true); // Mostrar el panel de menú principal/pausa de nuevo
        }
    }
}