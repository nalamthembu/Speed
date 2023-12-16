using System.Collections.Generic;

namespace CustomDataTypes
{
    [System.Serializable]
    public class Graph<T>
    {
        readonly Dictionary<T, List<T>> vertices = new();

        public void AddVertex(T data) => vertices.Add(data, new());

        public int GetVerticeCount
        {
            get
            {
                return vertices.Count;
            }
        }

        public bool AddEdge(T from, T to)
        {
            //check if both vertices exist
            if (!vertices.ContainsKey(from) || !vertices.ContainsKey(to))
                return false;

            //check if from contains to
            if (vertices[from].Contains(to))
                return false;


            vertices[from].Add(to);
            return true;
        }

        public List<T> GetConnectedVertices(T data)
        {
            if (vertices.ContainsKey(data))
                return vertices[data];

            return new List<T>();
        }

        public bool RemoveVertex(T data)
        {
            if (!vertices.ContainsKey(data))
                return false;

            vertices.Remove(data);

            foreach (KeyValuePair<T, List<T>> pair in vertices)
            {
                if (pair.Value.Contains(data))
                {
                    pair.Value.Remove(data);
                }
            }

            return true;
        }

        public bool ContainsVertex(T data)
        {
            return vertices.ContainsKey(data);
        }
    }
}
