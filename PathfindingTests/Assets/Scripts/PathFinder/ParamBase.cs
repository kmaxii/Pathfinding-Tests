using EpPathFinding.cs;
using PathFinder.Grid;

namespace PathFinder
{
    public delegate float HeuristicDelegate(int iDx, int iDy);

    public abstract class ParamBase
    {
        public ParamBase(BaseGrid iGrid, GridPos iStartPos, GridPos iEndPos) : this(iGrid)
        {
            m_startNode = m_searchGrid.GetNodeAt(iStartPos.x, iStartPos.y);
            m_endNode = m_searchGrid.GetNodeAt(iEndPos.x, iEndPos.y);
            if (m_startNode == null)
                m_startNode = new Node(iStartPos.x, iStartPos.y, true);
            if (m_endNode == null)
                m_endNode = new Node(iEndPos.x, iEndPos.y, true);
        }

        public ParamBase(BaseGrid iGrid)
        {
            m_searchGrid = iGrid;
            m_startNode = null;
            m_endNode = null;
        }
        


        public BaseGrid SearchGrid
        {
            get
            {
                return m_searchGrid;
            }
        }

        public Node StartNode
        {
            get
            {
                return m_startNode;
            }
        }
        public Node EndNode
        {
            get
            {
                return m_endNode;
            }
        }
        

        protected BaseGrid m_searchGrid;
        protected Node m_startNode;
        protected Node m_endNode;
    }
}
