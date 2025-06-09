using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class MainMenuManager : MonoBehaviour
{
    // Nombre de la escena de tu juego principal
    
    public string gameSceneName = "ElementalConvergence"; 

    public GameObject optionsPanel; 
    public GameObject mainPanelForThisMenu;

    public void StartGame()
    {
        Debug.Log("Botón Iniciar Juego presionado. Cargando escena: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenOptions()
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

    

    public void QuitGame()
    {
        Debug.Log("Botón Salir presionado.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();
#endif
    }
}