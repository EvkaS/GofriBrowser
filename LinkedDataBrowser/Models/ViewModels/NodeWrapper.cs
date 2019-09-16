using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Query;

namespace LinkedDataBrowser.Models.ViewModels
{
    public class NodeWrapper
    {
        public NodeWrapper(INode node, SparqlRemoteEndpoint endpoint)
        {
            originalNode = node;
            Endpoint = endpoint;
        }

        public SparqlRemoteEndpoint Endpoint { get; private set; }
        
        public NodeType NodeType => originalNode.NodeType;

        /// <summary>
        /// If the Node is Literal, get its lexical value; otherwise null
        /// </summary>
        public string Value => (NodeType == NodeType.Literal) ? ((ILiteralNode)originalNode).Value : null;

        /// <summary>
        /// If the Node is Literal and has a DataType, get its DataType URI; otherwise null
        /// </summary>
        public Uri DataType => (NodeType == NodeType.Literal) ? ((ILiteralNode)originalNode).DataType : null;

        /// <summary>
        /// If the Node is Literal and has a Language, get its Language; otherwise null
        /// </summary>
        public string Language => (NodeType == NodeType.Literal) ? ((ILiteralNode)originalNode).Language : null;
        /// <summary>
        /// Get the String representation of the Node
        /// </summary>
        public override string ToString() => originalNode.ToString();

        private INode originalNode;
    }

    public class PredicateResultWrapper : NodeWrapper, IComparable
    {
        public PredicateResultWrapper(INode node, SparqlRemoteEndpoint endpoint) : base(node, endpoint)
        {
            if (node.NodeType != NodeType.Uri)
            {
                /*TODO: remove this exception*/
                throw new Exception("This predicate is not IRI!");
            }
        }

        public string SubjectIri { get; set; }

        /// <summary>
        /// Number of objects this predicate has, it is equal to number of triples with given subject and predicate
        /// </summary>
        public int ObjectCount {
            get
            {
                return (objectList == null) ? 0 : objectList.Count;
            }
        }
        public int ExpectedObjectCount { get; set; }

        /// <summary>
        /// Popularity of the predicate according to LOV data or -1 if not set
        /// </summary>
        public int Popularity { get; set; } = -1;
        /// <summary>
        /// Returns object of this predicate from the position given by index
        /// </summary>
        /// <param name="index">position of object in the list of objects</param>
        public ObjectWrapper this[int index]
        {
            get
            {
                return (objectList.Count != 0) ? objectList[index] : null;
            }

        }
        public void AddObject(ObjectWrapper objectWrapper)
        {
            objectList.Add(objectWrapper);
        }

        
        private List<ObjectWrapper> objectList = new List<ObjectWrapper>();

        public int CompareTo(object obj)
        {
            //bigger popularity goes sooner => reversed order
            PredicateResultWrapper otherPredicate = obj as PredicateResultWrapper;
            if (otherPredicate == null)
                throw new ArgumentException($"Object is not a {nameof(PredicateResultWrapper)}");
            else
                return otherPredicate.Popularity - this.Popularity;
        }
    }

    public class ObjectWrapper : NodeWrapper
    {
        public ObjectWrapper(INode node, SparqlRemoteEndpoint endpoint) : base(node, endpoint) { }
    }

    
}
