using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    [SerializeField] BehaviourTree _tree;
    public BehaviourTree Tree { get => _tree; private set => _tree = value; }

    private Context _context;
    public Context Context { get => _context; private set => _context = value; }
    // Start is called before the first frame update
    void Start()
    {
        Context = Context.CreateFromGameObject(gameObject);
        Tree = Tree.Clone();
        Tree.Bind(Context);
    }

    // Update is called once per frame
    void Update()
    {
        if(Tree != null)
            Tree.Update();
    }

    private void OnDrawGizmosSelected()
    {
        if (Tree == null)
            return;

        BehaviourTree.Traverse(Tree.RootNode, (node) =>
        {
            if (node.DrawGizmos)
                node.OnDrawGizmos();
        });
    }
}
