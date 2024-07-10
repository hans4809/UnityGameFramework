using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodePort : Port
{
    private class DefaultEdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphViewChange m_GraphViewChange;
        public GraphViewChange GraphViewChange { get => m_GraphViewChange; set => m_GraphViewChange = value; }

        private List<Edge> m_EdgesToCreate;
        public  List<Edge> EdgesToCreate { get => m_EdgesToCreate; set => m_EdgesToCreate = value; }

        private List<GraphElement> m_EdgesToDelete;
        public List<GraphElement> EdgesToDelete { get => m_EdgesToDelete; set => m_EdgesToDelete = value; }

        public DefaultEdgeConnectorListener()
        {
            EdgesToCreate = new List<Edge>();
            EdgesToDelete = new List<GraphElement>();

            m_GraphViewChange.edgesToCreate = EdgesToCreate;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position) { }
        public void OnDrop(GraphView graphView, Edge edge)
        {
            EdgesToCreate.Clear();
            EdgesToCreate.Add(edge);

            // We can't just add these edges to delete to the m_GraphViewChange
            // because we want the proper deletion code in GraphView to also
            // be called. Of course, that code (in DeleteElements) also
            // sends a GraphViewChange.
            EdgesToDelete.Clear();
            if (edge.input.capacity == Capacity.Single)
                foreach (Edge edgeToDelete in edge.input.connections)
                    if (edgeToDelete != edge)
                        EdgesToDelete.Add(edgeToDelete);
            if (edge.output.capacity == Capacity.Single)
                foreach (Edge edgeToDelete in edge.output.connections)
                    if (edgeToDelete != edge)
                        EdgesToDelete.Add(edgeToDelete);
            if (EdgesToDelete.Count > 0)
                graphView.DeleteElements(EdgesToDelete);

            var edgesToCreate = EdgesToCreate;
            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(GraphViewChange).edgesToCreate;
            }

            foreach (Edge e in edgesToCreate)
            {
                graphView.AddElement(e);
                edge.input.Connect(e);
                edge.output.Connect(e);
            }
        }
    }

    public NodePort(Direction direction, Capacity capacity) : base(Orientation.Vertical, direction, capacity, typeof(bool))
    {
        var connectorListener = new DefaultEdgeConnectorListener();
        m_EdgeConnector = new EdgeConnector<Edge>(connectorListener);
        this.AddManipulator(m_EdgeConnector);
        style.width = 100;
    }

    public override bool ContainsPoint(Vector2 localPoint)
    {
        Rect rect = new Rect(0, 0, layout.width, layout.height);
        return rect.Contains(localPoint);
    }
}
