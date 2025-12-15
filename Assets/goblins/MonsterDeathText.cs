using UnityEngine;
using TMPro;

public class MonsterDeathText : MonoBehaviour
{
    // Public messages so they can be edited in Inspector or from other scripts
    public static string monsterADeathMessage =
        "Mr Fallador prefers a dark room, we aren't here to make him comfortable";

    public static string monsterBDeathMessage =
        "The orphans worked in the dark, your eyes alone should be enough to keep them busy";

    // Default fallback message
    private static string deathMessage = "You Died";

    public TMP_Text tmpText;
    public UnityEngine.UI.Text uiText;

    // Called by MonsterDeathHandler
    public static void SetDeathMessage(string msg)
    {
        deathMessage = msg;
    }

    private void OnEnable()
    {
        if (tmpText != null)
            tmpText.text = deathMessage;

        if (uiText != null)
            uiText.text = deathMessage;
    }
}