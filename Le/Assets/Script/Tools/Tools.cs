using UnityEditor;

public class Tools
{
    public static void ClearConsole()
    {
        typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntries")
            .GetMethod("Clear")
            ?.Invoke(null, null);
    }
}
