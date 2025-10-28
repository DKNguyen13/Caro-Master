using UnityEngine;

public class AIController : MonoBehaviour
{
    public static AIController Instance;
    private const int AI = 2;
    private const int PLAYER = 1;
    private const int MAX_DEPTH = 3;

    private void Awake() => Instance = this;

    public void MakeMove()
    {
        int[,] board = (int[,])GameController.Instance.GetBoard.Clone();
        int size = GameController.Instance.CellQuantity;

        int bestScore = int.MinValue;
        Vector2Int bestMove = new Vector2Int(-1, -1);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (board[x, y] == 0)
                {
                    board[x, y] = AI;
                    int score = Minimax(board, MAX_DEPTH, false, int.MinValue, int.MaxValue);
                    board[x, y] = 0;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = new Vector2Int(x, y);
                    }
                }
            }
        }

        if (bestMove.x != -1)
        {
            GameController.Instance.SetCell(bestMove.x, bestMove.y, AI);

            GameObject cellObj = GameObject.Find($"Cell({bestMove.x},{bestMove.y})");
            if (cellObj)
            {
                Cell cell = cellObj.GetComponent<Cell>();
                cell.setMark(AI);
            }

            if (GameController.Instance.CheckWin(bestMove.x, bestMove.y, AI))
            {
                Debug.Log("AI WIN!");
            }
        }
    }

    private int Minimax(int[,] board, int depth, bool isMaximizing, int alpha, int beta)
    {
        int size = GameController.Instance.CellQuantity;

        // Kiểm tra thắng thua
        if (IsWinning(board, AI)) return 10000 + depth;
        if (IsWinning(board, PLAYER)) return -10000 - depth;
        if (depth == 0) return Evaluate(board);

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (board[x, y] == 0)
                    {
                        board[x, y] = AI;
                        int eval = Minimax(board, depth - 1, false, alpha, beta);
                        board[x, y] = 0;
                        maxEval = Mathf.Max(maxEval, eval);
                        alpha = Mathf.Max(alpha, eval);
                        if (beta <= alpha) return maxEval; // Cắt nhánh
                    }
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (board[x, y] == 0)
                    {
                        board[x, y] = PLAYER;
                        int eval = Minimax(board, depth - 1, true, alpha, beta);
                        board[x, y] = 0;
                        minEval = Mathf.Min(minEval, eval);
                        beta = Mathf.Min(beta, eval);
                        if (beta <= alpha) return minEval;
                    }
                }
            }
            return minEval;
        }
    }

    private int Evaluate(int[,] board)
    {
        int score = 0;
        int size = GameController.Instance.CellQuantity;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (board[x, y] == AI)
                    score += CountPotential(board, x, y, AI);
                else if (board[x, y] == PLAYER)
                    score -= CountPotential(board, x, y, PLAYER);
            }
        }
        return score;
    }

    private int CountPotential(int[,] board, int x, int y, int player)
    {
        int[][] dirs = new int[][]
        {
            new int[]{1,0}, new int[]{0,1}, new int[]{1,1}, new int[]{1,-1}
        };

        int count = 0;
        foreach (var dir in dirs)
        {
            int consecutive = 1;
            int nx = x + dir[0];
            int ny = y + dir[1];
            while (InBoard(nx, ny, board) && board[nx, ny] == player)
            {
                consecutive++;
                nx += dir[0];
                ny += dir[1];
            }
            if (consecutive >= 5) return 10000;
            count += consecutive * consecutive;
        }
        return count;
    }

    private bool IsWinning(int[,] board, int player)
    {
        int size = GameController.Instance.CellQuantity;
        GameController g = GameController.Instance;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (board[x, y] == player)
                {
                    if (g.CheckWin(x, y, player)) return true;
                }
            }
        }
        return false;
    }

    private bool InBoard(int x, int y, int[,] board)
    {
        int size = board.GetLength(0);
        return x >= 0 && y >= 0 && x < size && y < size;
    }
}
