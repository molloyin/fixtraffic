using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    /**
     * Matrix class that implements an adjacency matrix of type T.
     * T must inherit from MatrixNode, which is a MonoBehaviour with an id.
     *
     * The matrix is represented as a list of rows. Each row represents all the edges of a node.
     * Weight of the edge is the distance between the two nodes.
     * If the weight is 0, there is no edge between the two nodes.
     * 
     * The matrix is square, so the number of rows is equal to the number of columns.
     * The number of rows is equal to the number of vertices in the matrix, that is the list of vertices.
     *
     * The vertices are stored in a list. The index of the vertex in the list is its id.
     * The id of the vertex is the index of the row in the list of rows.
     *
     * For example, if we want to get the successors of the vertex with the id 2,
     * we get the row at index 2 in the list of rows, and we get the vertices of the row where the
     * weight is greater than 0.
     *
     * The matrix is consistent, so there is no jump in the ids of the vertices.
     */
    [Serializable]
    public class Matrix<T> where T : MatrixNode
    {
        // The matrix is represented as a list of rows.
        // The index of the row in the list is the id of the vertex.
        [SerializeField] private List<MatrixRow<int>> rows;

        // vertices are stored in a list. 
        // The index of the vertex in the list is its id.
        [SerializeField] private List<T> vertices;

        // Name of the vertices for renaming purposes after removal.
        [SerializeField] private string nodeName;

        public Matrix(string _nodeName)
        {
            rows = new();
            vertices = new();
            nodeName = _nodeName;
        }

        /**
         * Computes the weight of the edge between two nodes.
         * @param _from The first node.
         * @param _to The second node.
         * @return Computed weight value
         */
        private int ComputeWeight(T _from, T _to)
            => Mathf.RoundToInt(Vector3.Distance(_from.transform.position,
                _to.transform.position)
            );

        /**
         * Adds an vertex to the matrix.
         * @param _elm The vertex to add.
         * @param _from The vertex to link the new vertex to.
         * @return The added vertex.
         */
        public T Add(T _elm, T _from)
        {
            Add(_elm);
            if (_from != null)
                Link(_from, _elm);

            return _elm;
        }

        /**
         * Add an vertex to the matrix without linking it to any other vertex.
         * @param _elm The vertex to add.
         * @return The added vertex.
         */
        public T Add(T _elm)
        {
            _elm.id = GetNewIndex();
            vertices.Add(_elm);
            foreach (var row in rows)
                row.vertices.Add(0);

            rows.Add(new MatrixRow<int>()
            {
                vertices = new List<int>(new int [Count])
            });

            return _elm;
        }

        /**
         * Links two vertices in the matrix.
         * @param _from The first vertex.
         * @param _to The second vertex.
         */
        public void Link(T _from, T _to)
            => rows[_from.id].vertices[_to.id] = ComputeWeight(_from, _to);

        /**
         * Links two vertices in the matrix with their ids.
         * @param _from The id of the first vertex.
         * @param _to The id of the second vertex.
         */
        public void Link(int _from, int _to)
            => rows[_from].vertices[_to] = ComputeWeight(Get(_from), Get(_to));

        /**
         * Removes an vertex from the matrix.
         * @param _elm The vertex to remove.
         */
        public void Remove(T _elm)
        {
            vertices.Remove(_elm);
            foreach (var row in rows)
                row.vertices.RemoveAt(_elm.id);
            rows.RemoveAt(_elm.id);
            for (int i = _elm.id; i < vertices.Count; i++)
            {
                T vertexToUpdate = vertices[i];
                vertexToUpdate.id = i;
                vertexToUpdate.gameObject.name = nodeName + " " + i;
            }
        }

        // Size of the matrix.
        public int Count => vertices.Count;

        // Get all the vertices in the matrix.
        public T[] Vertices => vertices.ToArray();

        /**
         * Checks if the matrix contains an vertex.
         * @param _elm The vertex to check.
         */
        public bool Contains(T _elm)
            => vertices.Contains(_elm);

        /**
         * Checks if the matrix contains an vertex with the given id.
         * @param _id The id of the vertex to check.
         */
        public bool Contains(int _id)
            => _id >= Count || _id < 0;

        /**
         * Get successors of an vertex from all the possible successors.
         * A successor is an vertex that the weight of the edge between the
         * two vertices is greater than 0.
         * @param _row The row of the vertex.
         */
        private T[] _GetSuccessors(List<int> _row)
        {
            List<T> successors = new();
            for (int i = 0; i < _row.Count; i++)
            {
                if (_row[i] > 0)
                {
                    successors.Add(vertices[i]);
                }
            }

            return successors.ToArray();
        }

        /**
         * Get successors of an vertex, that is the vertices next to the vertex.
         * @param _elm The vertex.
         * @return The successors of the vertex.
         */
        public T[] GetSuccessors(T _elm)
            => _GetSuccessors(rows[_elm.id].vertices);

        /**
         * Get successors of an vertex by id, that is the vertices next to the vertex with the given id.
         * @param _id The id of the vertex.
         * @return The successors of the vertex.
         */
        public T[] GetSuccessors(int _id)
            => _GetSuccessors(rows[_id].vertices);

        /**
         * Get predecessors of an vertex by its index in the list of vertices.
         * We need to iterate through all the rows to find the predecessors at the given index.
         * @param _index The index of the vertex in the list of vertices.
         * @return The predecessors of the vertex.
         */
        private T[] _GetPredecessors(int _index)
        {
            List<T> predecessors = new();
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i].vertices[_index] > 0)
                {
                    predecessors.Add(vertices[i]);
                }
            }

            return predecessors.ToArray();
        }

        /**
         * Get predecessors of an vertex, that is the vertices before the vertex.
         * @param _elm The vertex.
         * @return The predecessors of the vertex.
         */
        public T[] GetPredecessors(T _elm)
            => _GetPredecessors(_elm.id);

        /**
         * Get predecessors of an vertex by id, that is the vertices before the vertex with the given id.
         * @param _id The id of the vertex.
         * @return The predecessors of the vertex.
         */
        public T[] GetPredecessors(int _id)
            => _GetPredecessors(_id);

        /**
         * Get the new index of an vertex to add to the matrix.
         * @return The new index of the vertex.
         */
        public int GetNewIndex() => Count;

        /**
         * Get an vertex by its id.
         * @param _id The id of the vertex.
         * @return The vertex.
         */
        public T Get(int _id)
            => vertices[_id];

        /**
         * Update the weights of the matrix for a given vertex.
         * @param _elm The vertex to update the weights for.
         * @return The updated vertex.
         */
        public void UpdateWeights(T _elm)
        {
            List<int> row = rows[_elm.id].vertices;
            for (int i = 0; i < row.Count; i++)
            {
                if (row[i] > 0)
                    row[i] = ComputeWeight(_elm, vertices[i]);
                if (rows[i].vertices[_elm.id] > 0)
                    rows[i].vertices[_elm.id] = ComputeWeight(vertices[i], _elm);
            }
        }

        /**
         * To string method.
         * @return The string representation of the matrix.
         */
        public override string ToString()
        {
            string str = "";

            foreach (var row in rows)
            {
                foreach (var vertex in row.vertices)
                    str += vertex + ",    ";
                str = str[..^2] + "\n";
            }

            return str;
        }

        // Get weight of an edge between two vertices.
        public int GetWeight(int _i, int _j) => rows[_i].vertices[_j];

        /**
         * Check if vertex _to is reachable from vertex _from.
         * @param _from The vertex to start from.
         * @param _to The vertex to reach.
         * @return True if the vertex is reachable, false otherwise.
         */
        public bool IsReachable(Waypoint _from, Waypoint _to)
        {
            bool[] marks = new bool [Count];
            Stack<int> stack = new Stack<int>();
            stack.Push(_from.id);
            marks[_from.id] = true;
            bool founded = false;
            
            while (stack.Count > 0 && !founded)
            {
                int visiting = stack.Pop();
                if (visiting == _to.id)
                    founded = true;
                else
                    foreach (var successor in GetSuccessors(visiting))
                    {
                        if (!marks[successor.id])
                        {
                            stack.Push(successor.id);
                            marks[successor.id] = true;
                        }
                    }

            }

            return founded;
        }
    }

    /**
     * A node in the matrix.
     */
    public class MatrixNode : MonoBehaviour
    {
        // The id of the node.
        public int id;
    }

    /**
     * A row in the matrix.
     * This ables us to serialize the matrix
     */
    [Serializable]
    public class MatrixRow<T>
    {
        // The weight of the edges between the vertices of the row.
        public List<T> vertices = new();
    }
}