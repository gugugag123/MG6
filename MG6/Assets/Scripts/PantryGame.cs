using UnityEngine;
using UnityEngine.SceneManagement;

public class PantryGame : MonoBehaviour
{
    public Transform cat;
    public Rat mother;
    private GUIStyle _title, _small, _counter;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (cat != null && cat.position.y < -9)
        {
            cat.position = new Vector3(-11, -3.8f, 0);
            cat.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }

    private void OnGUI()
    {
        if (_title == null)
        {
            _title = new GUIStyle(GUI.skin.label) { fontSize = 27, fontStyle = FontStyle.Bold };
            _small = new GUIStyle(GUI.skin.label) { fontSize = 15 };
            _counter = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleRight };
            _title.normal.textColor = new Color(1, 0.89f, 0.66f);
            _small.normal.textColor = new Color(0.8f, 0.84f, 0.87f);
            _counter.normal.textColor = new Color(1, 0.89f, 0.66f);
        }
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 960.0f, Screen.height / 540.0f, 1));
        GUI.color = new Color(0.025f, 0.04f, 0.07f, 0.94f);
        GUI.DrawTexture(new Rect(0, 0, 960, 76), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(0, 504, 960, 36), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(25, 8, 580, 36), "MIDNIGHT PANTRY", _title);
        GUI.Label(new Rect(27, 43, 580, 25), "One cat. Five more problems with every kick.", _small);
        if (mother != null)
        {
            int hits = mother.HitCount;
            string next = hits % 3 == 0 ? "YELLOW" : hits % 3 == 1 ? "RED" : "BLUE";
            GUI.Label(new Rect(630, 13, 305, 26), "HITS  " + hits + "     /     NEXT  " + next, _counter);
            GUI.Label(new Rect(600, 42, 335, 26), hits >= 6 ? "Pantry chaos achieved. Keep going!" : "Find the red rat. Try three kicks.", _counter);
        }
        GUI.Label(new Rect(26, 511, 930, 26), "A / D or arrows  MOVE     SPACE  JUMP     LEFT SHIFT  KICK     R  RESTART", _small);
    }
}
