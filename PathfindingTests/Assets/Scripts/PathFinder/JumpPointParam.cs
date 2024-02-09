using C5;
using EpPathFinding.cs;
using PathFinder.Grid;

namespace PathFinder
{
    public delegate float HeuristicDelegate(int iDx, int iDy);

    public class JumpPointParam
    {
        private readonly BaseGrid _mSearchGrid;
        private Node _mStartNode;
        private Node _mEndNode;
        public readonly IntervalHeap<Node> openList; 

        // Combining constructors
        public JumpPointParam(BaseGrid iGrid, GridPos iStartPos, GridPos iEndPos)
        {
            _mSearchGrid = iGrid;
            SetStartAndEndNodes(iStartPos, iEndPos);
            openList = new IntervalHeap<Node>();
        }
        
        private void SetStartAndEndNodes(GridPos iStartPos, GridPos iEndPos)
        {
            _mStartNode = _mSearchGrid.GetNodeAt(iStartPos.x, iStartPos.y) ?? new Node(iStartPos.x, iStartPos.y, true);
            _mEndNode = _mSearchGrid.GetNodeAt(iEndPos.x, iEndPos.y) ?? new Node(iEndPos.x, iEndPos.y, true);
        }

        public BaseGrid SearchGrid => _mSearchGrid;

        public Node StartNode => _mStartNode;

        public Node EndNode => _mEndNode;
    }
}