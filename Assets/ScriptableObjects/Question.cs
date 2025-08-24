using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQA", menuName = "Quiz/QuestionAnswer", order = 1)]
public class Question : ScriptableObject
{

public class QuestionAnswer : ScriptableObject {
    [TextArea(2, 5)]
    public string question;

    [TextArea(1, 3)]
    public string answer;
}


}
