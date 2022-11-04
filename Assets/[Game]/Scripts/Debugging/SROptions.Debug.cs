#if TRIFLES_DEBUG
using System.ComponentModel;
using TriflesGames.Utils;
using UnityEngine;

/// <summary>
/// To add change a variable in runtime, you can add them here as a property.
/// They will be shown in SROptions > Options
/// There are attributes to distinguish your variables properly:
/// NumberRange: Limits a number property to the range provided.
/// Increment:   Changes how much a number property is changed by pressing the up/down buttons on the options tab.
/// DisplayName: Manually control what name the option will use on the options tab.
/// Sort:        Provide a sorting index that is used to control the order in which options appear.
/// Category:    Group options by category name. (Note: The System.ComponentModel namespace must be imported in your file to use this attribute.
/// </summary>
public partial class SROptions
{
      
    [Category("Utilities")]
    [DisplayName("Toggle FPS")]
    public bool ToggleFPS
    {
        get => Object.FindObjectOfType<FPS>().enabled;
        set => Object.FindObjectOfType<FPS>().enabled = value;
    }
}
#endif