using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    [SerializeField] BehaviourTree _tree;
    public BehaviourTree Tree { get => _tree; private set => _tree = value; }
    // Start is called before the first frame update
    void Start()
    {
        Tree = Tree.Clone();
        Tree.Bind(/*GetComponent<AiAgent>*/);
    }

    // Update is called once per frame
    void Update()
    {
        Tree.Update();
    }
}
