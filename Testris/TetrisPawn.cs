using System;
using System.Collections.Generic;
using System.Drawing;
using TheraEngine;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.HUD;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;

namespace Testris
{
    public class TetrisPawn : HudManager, I3DRenderable
    {
        private int _columns = 10;
        private int _rows = 20;
        private float _secondsPerBlock = 2.0f;
        private bool[,] _blockBoard;
        private TetrisPiece _currentBlock;
        private TetrisPiece _nextBlock;
        private float _delta;
        private bool _playing = false;
        private int _score;
        private int[] _baseScores = { 40, 100, 300, 1200 };
        private int _level = 0;
        private Random _rng = new Random();
        private IOctreeNode _renderNode;
        private bool _isRendering;

        private void AddScore(int lines)
        {
            _score += _baseScores[lines] * (_level + 1);
        }

        public float BlocksPerSec
        {
            get => 1.0f / _secondsPerBlock;
            set => _secondsPerBlock = 1.0f / value;
        }

        public bool HasTransparency => false;
        public Shape CullingVolume => null;

        public IOctreeNode RenderNode
        {
            get => _renderNode;
            set => _renderNode = value;
        }
        public bool IsRendering
        {
            get => _isRendering;
            set => _isRendering = value;
        }

        public override void RegisterInput(InputInterface input)
        {
            input.RegisterButtonEvent(GamePadButton.SpecialRight, ButtonInputType.Pressed, Pause, InputPauseType.TickAlways);
            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, Pause, InputPauseType.TickAlways);

            input.RegisterButtonEvent(EKey.Enter, ButtonInputType.Pressed, BeginGame, InputPauseType.TickOnlyWhenPaused);
            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, BeginGame, InputPauseType.TickOnlyWhenPaused);

            input.RegisterButtonEvent(EKey.Up, ButtonInputType.Pressed, Rotate, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Left, ButtonInputType.Pressed, MoveLeft, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Right, ButtonInputType.Pressed, MoveRight, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Down, ButtonInputType.Pressed, StartSpeedUp, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Down, ButtonInputType.Released, EndSpeedUp, InputPauseType.TickOnlyWhenUnpaused);
        }

        private void BeginGame()
        {
            _score = 0;
            _playing = true;
            _nextBlock = TetrisPiece.New(_rng.Next(0, 6));
            SpawnBlock();
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, SceneUpdate);
        }
        private void GameOver()
        {
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, SceneUpdate);
        }
        private void Pause()
        {
            Engine.TogglePause(LocalPlayerController.LocalPlayerIndex);
        }
        
        private void EndSpeedUp()
        {
            BlocksPerSec = 2.0f;
        }

        private void StartSpeedUp()
        {
            BlocksPerSec = 4.0f;
        }

        private void MoveRight()
        {
            if (_currentBlock == null)
                return;
            MoveUpdateBoard(false);
            ++_currentBlock.ColumnShift;
            if (HasCollision())
                --_currentBlock.ColumnShift;
            MoveUpdateBoard(true);
        }
        private void MoveLeft()
        {
            if (_currentBlock == null)
                return;
            MoveUpdateBoard(false);
            --_currentBlock.ColumnShift;
            if (HasCollision())
                ++_currentBlock.ColumnShift;
            MoveUpdateBoard(true);
        }
        private bool MoveDown()
        {
            if (_currentBlock == null)
                return false;
            MoveUpdateBoard(false);
            ++_currentBlock.RowShift;
            if (HasCollision())
            {
                --_currentBlock.RowShift;
                return false;
            }
            MoveUpdateBoard(true);
            return true;
        }
        private void Rotate()
        {
            MoveUpdateBoard(false);
            _currentBlock.Rotate(true);
            if (!RotationSuccessful(out int kickColumnShift))
                _currentBlock.Rotate(false);
            else
                _currentBlock.ColumnShift += kickColumnShift;
            MoveUpdateBoard(true);
        }
        private bool RotationSuccessful(out int kickColumnShift)
        {
            kickColumnShift = 0;
            if (_currentBlock == null)
                return false;
            return !HasCollision();
        }
        private bool HasCollision()
        {
            bool[,] blockLayout = _currentBlock.CurrentRotation;
            int rowShift = _currentBlock.RowShift;
            int colShift = _currentBlock.ColumnShift;
            for (int rowIndex = 0; rowIndex < blockLayout.GetLength(0); ++rowIndex)
            {
                int boardRowIndex = rowIndex + rowShift;
                //No need to check if the row index is less than zero
                //The pieces do not move up, and they spawn offscreen anyway
                if (boardRowIndex >= _rows)
                    return true;
                for (int colIndex = 0; colIndex < blockLayout.GetLength(1); ++colIndex)
                {
                    int boardColIndex = colIndex + colShift;
                    if (boardColIndex < 0 || boardColIndex >= _columns)
                        return true;
                    if (blockLayout[rowIndex, colIndex] && _blockBoard[boardRowIndex, boardColIndex])
                        return true;
                }
            }
            return true;
        }
        private void MoveUpdateBoard(bool postMove)
        {
            bool[,] blockLayout = _currentBlock.CurrentRotation;
            int rowShift = _currentBlock.RowShift;
            int colShift = _currentBlock.ColumnShift;

            for (int rowIndex = 0; rowIndex < blockLayout.GetLength(0); ++rowIndex)
            {
                int boardRowIndex = rowIndex + rowShift;

                //Nothing to update offscreen
                if (boardRowIndex >= _rows || boardRowIndex < 0)
                    continue;

                for (int colIndex = 0; colIndex < blockLayout.GetLength(1); ++colIndex)
                {
                    int boardColIndex = colIndex + colShift;

                    //Nothing to update offscreen
                    if (boardColIndex >= _columns || boardColIndex < 0)
                        continue;

                    //If the layout has a true signifying a piece,
                    //update that piece in the board
                    if (blockLayout[rowIndex, colIndex])
                        _blockBoard[boardRowIndex, boardColIndex] = postMove;
                }
            }
        }
        protected override DockableHudComponent OnConstruct()
        {
            Hud = this;
            DockableHudComponent root = new DockableHudComponent()
            {
                DockStyle = HudDockStyle.Fill
            };
            TextureHudComponent boardTexture = new TextureHudComponent()
            {

            };
            boardTexture.Parent = root;
            return root;
        }

        public override void OnSpawned(World world)
        {
            _blockBoard = new bool[_rows, _columns];
            base.OnSpawned(world);
        }

        public override void OnDespawned()
        {
            base.OnDespawned();
        }
        
        private void SceneUpdate(float delta)
        {
            _delta += delta;
            if (_delta >= _secondsPerBlock)
            {
                _delta = 0.0f;
                if (!MoveDown())
                    BlockPlaced();
            }
        }

        private void BlockPlaced()
        {
            int rows = 0;
            for (int rowIndex = _currentBlock.CurrentRotation.GetLength(0); rowIndex >= 0; --rowIndex)
            {
                if (rowIndex < 0)
                {
                    GameOver();
                    return;
                }
                int boardRowIndex = rowIndex + _currentBlock.RowShift;
                bool allFilled = true;
                for (int boardColIndex = 0; boardColIndex < _columns; ++boardColIndex)
                {
                    if (!_blockBoard[boardRowIndex, boardColIndex])
                    {
                        allFilled = false;
                        break;
                    }
                }
                if (allFilled)
                {
                    ++rows;
                    RemoveRow(boardRowIndex);
                }
            }
            AddScore(rows);
            SpawnBlock();
        }
        private void RemoveRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _rows)
                return;

            for (int row = rowIndex - 1; row >= 0; --row)
                for (int col = 0; col < _columns; ++col)
                    _blockBoard[row + 1, col] = _blockBoard[row, col];

            for (int col = 0; col < _columns; ++col)
                _blockBoard[0, col] = false;
        }
        private void SpawnBlock()
        {
            _currentBlock = _nextBlock;
            if (HasCollision())
                GameOver();
            else
                _nextBlock = TetrisPiece.New(_rng.Next(0, 6));
        }
    }
    public class TetrisPiece
    {
        private ColorF3 _color;
        private int _columnShift, _rowShift;

        private int _currentRotationIndex;
        private bool[,] _currentRotation;
        private bool[,] _pos1;
        private bool[,] _pos2;
        private bool[,] _pos3;
        private bool[,] _pos4;

        private TetrisPiece(
            ColorF3 color, int initialRowShift, int initialColumnShift, 
            bool[,] pos1, bool[,] pos2, bool[,] pos3, bool[,] pos4)
        {
            _color = color;
            _rowShift = initialRowShift;
            _columnShift = initialColumnShift;
            _pos1 = pos1;
            _pos2 = pos2;
            _pos3 = pos3;
            _pos4 = pos4;
            _currentRotationIndex = 0;
            _currentRotation = _pos1;
        }

        public ColorF3 RGB => _color;
        public int ColumnShift
        {
            get => _columnShift;
            set => _columnShift = value;
        }
        public int RowShift
        {
            get => _rowShift;
            set => _rowShift = value;
        }
        public bool[,] CurrentRotation => _currentRotation;

        internal void Rotate(bool clockwise)
        {
            if (clockwise)
            {
                if (_currentRotationIndex == 3)
                    _currentRotationIndex = 0;
                else
                    ++_currentRotationIndex;
            }
            else
            {
                if (_currentRotationIndex == 0)
                    _currentRotationIndex = 3;
                else
                    --_currentRotationIndex;
            }
            switch (_currentRotationIndex)
            {
                case 0: _currentRotation = _pos1; break;
                case 1: _currentRotation = _pos2; break;
                case 2: _currentRotation = _pos3; break;
                case 3: _currentRotation = _pos4; break;
            }
        }

        public static TetrisPiece New(int id)
        {
            ColorF3 color = new ColorF3();
            int rowShift = 0, colShift = 0;
            bool[,] pos1 = null, pos2 = null, pos3 = null, pos4 = null;
            switch (id)
            {
                case 0:
                    colShift = 3;
                    rowShift = -4;
                    color = Color.LightSkyBlue;
                    pos3 = new bool[,]
                    {
                        { false, false, false, false },
                        { true,  true,  true,  true  },
                        { false, false, false, false },
                        { false, false, false, false },
                    };
                    pos4 = new bool[,]
                    {
                        { false, false, true, false },
                        { false, false, true, false },
                        { false, false, true, false },
                        { false, false, true, false },
                    };
                    pos1 = new bool[,]
                    {
                        { false, false, false, false },
                        { false, false, false, false },
                        { true,  true,  true,  true  },
                        { false, false, false, false },
                    };
                    pos2 = new bool[,]
                    {
                        { false, true, false, false },
                        { false, true, false, false },
                        { false, true, false, false },
                        { false, true, false, false },
                    };
                    break;
                case 1:
                    colShift = 4;
                    rowShift = -3;
                    color = Color.Blue;
                    pos3 = new bool[,]
                    {
                        { true,  false, false },
                        { true,  true,  true  },
                        { false, false, false },
                    };
                    pos4 = new bool[,]
                    {
                        { false, true, true  },
                        { false, true, false },
                        { false, true, false },
                    };
                    pos1 = new bool[,]
                    {
                        { false, false, false },
                        { true,  true,  true  },
                        { false, false, true  },
                    };
                    pos2 = new bool[,]
                    {
                        { false, true, false },
                        { false, true, false },
                        { true,  true, false },
                    };
                    break;
                case 2:
                    colShift = 4;
                    rowShift = -3;
                    color = Color.Orange;
                    pos3 = new bool[,]
                    {
                        { false, false, true  },
                        { true,  true,  true  },
                        { false, false, false },
                    };
                    pos4 = new bool[,]
                    {
                        { false, true, false },
                        { false, true, false },
                        { false, true, true  },
                    };
                    pos1 = new bool[,]
                    {
                        { false, false, false },
                        { true,  true,  true  },
                        { true,  false, false },
                    };
                    pos2 = new bool[,]
                    {
                        { true,  true, false },
                        { false, true, false },
                        { false, true, false },
                    };
                    break;
                case 3:
                    color = Color.Yellow;
                    pos1 = pos2 = pos3 = pos4 = new bool[,]
                    {
                        { true, true, },
                        { true, true, },
                    };
                    break;
                case 4:
                    color = Color.Green;
                    pos3 = new bool[,]
                    {
                        { false, true,  true  },
                        { true,  true,  false },
                        { false, false, false },
                    };
                    pos4 = new bool[,]
                    {
                        { false, true,  false },
                        { false, true,  true  },
                        { false, false, true  },
                    };
                    pos1 = new bool[,]
                    {
                        { false, false, false },
                        { false, true,  true  },
                        { true,  true,  false },
                    };
                    pos2 = new bool[,]
                    {
                        { true,  false, false },
                        { true,  true,  false },
                        { false, true,  false },
                    };
                    break;
                case 5:
                    color = Color.Magenta;
                    pos3 = new bool[,]
                    {
                        { false, true,  false },
                        { true,  true,  true  },
                        { false, false, false },
                    };
                    pos4 = new bool[,]
                    {
                        { false, true, false },
                        { false, true, true  },
                        { false, true, false },
                    };
                    pos1 = new bool[,]
                    {
                        { false, false, false },
                        { true,  true,  true  },
                        { false, true,  false },
                    };
                    pos2 = new bool[,]
                    {
                        { false, true, false },
                        { true,  true, false },
                        { false, true, false },
                    };
                    break;
                case 6:
                    color = Color.Red;
                    pos3 = new bool[,]
                    {
                        { true,  true,  false },
                        { false, true,  true  },
                        { false, false, false },
                    };
                    pos4 = new bool[,]
                    {
                        { false, false, true  },
                        { false, true,  true  },
                        { false, true,  false },
                    };
                    pos1 = new bool[,]
                    {
                        { false, false, false },
                        { true,  true,  false },
                        { false, true,  true  },
                    };
                    pos2 = new bool[,]
                    {
                        { false, true,  false },
                        { true,  true,  false },
                        { true,  false, false },
                    };
                    break;
                default:
                    return null;
            }
            return new TetrisPiece(color, rowShift, colShift, pos1, pos2, pos3, pos4);
        }
    }
}