using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UIManager that acts as a registry of UI panels.
/// Panels will self-register, so there is no need for any serialized references to every panel.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private UIManager _instance;

    // the registry for the panels that UIManager should keep track of:
    private readonly Dictionary<Type, UIBase> _uiPanels = new();

    private void Awake()
    {
        if (_instance != this && _instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Registers the UI Panel to this manager's registry
    /// </summary>
    /// <param name="panel">the UI panel to register</param>
    public void Register(UIBase panel) => _uiPanels[panel.GetType()] = panel;

    /// <summary>
    /// Shows the UI Panel of the given kind from the registry.
    /// </summary>
    /// <typeparam name="T">the type of UI panel (limited to the children of UIBase)</typeparam>
    public void Show<T>() where T : UIBase
    {
        _uiPanels.TryGetValue(typeof(T), out var panel);
        panel?.Show(); // this gets executed only if the panel exists in the registry.
    }

    /// <summary>
    /// Hides the UI Panel of the given kind from the registry
    /// </summary>
    /// <typeparam name="T">the type of UI panel (limited to the children of UIBase)</typeparam>
    public void Hide<T>() where T : UIBase
    {
        _uiPanels.TryGetValue(typeof(T), out var panel);
        panel?.Hide(); // this gets executed only if the panel exists in the registry.
    }

    /// <summary>
    /// Gets the registered UI panel for the given type.
    /// Used when one needs to call a panel-specific method,
    /// and to push data directly to the panel rather than via event
    /// </summary>
    /// <typeparam name="T">the type of UI panel (limited to the children of UIBase)</typeparam>
    /// <returns></returns>
    public T Get<T>() where T : UIBase
    {
        return _uiPanels.TryGetValue(typeof(T), out var panel) ? panel as T : null;
    }

}
