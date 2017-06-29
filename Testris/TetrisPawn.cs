using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Audio;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.HUD;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;

namespace Testris
{
    public class TetrisPawn : HudManager, I3DRenderable
    {
        public const float DEFAULT_SPEED = 3.0f;
        public const float FAST_SPEED = 14.0f;
        public const int COLUMNS = 10;
        public const int ROWS = 20;

        private int _columns = COLUMNS;
        private int _rows = ROWS;
        private float _secondsPerBlock;
        private int[,] _blockBoard;
        private MaterialHudComponent[,] _hudBoard;
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
        private SoundFile _theme = new SoundFile();
        private AudioSourceParameters _ambientParams = new AudioSourceParameters()
        {
            SourceRelative = new UsableValue<bool>(true, false, true),
            Gain = new UsableValue<float>(0.6f, 1.0f, true),
            Loop = new UsableValue<bool>(true, false, true),
        };

        private void AddScore(int lines)
        {
            --lines;
            if (lines < 0)
                return;
            _score += _baseScores[lines] * (_level + 1);
        }

        public float BlocksPerSec
        {
            get => 1.0f / _secondsPerBlock;
            set => _secondsPerBlock = 1.0f / value;
        }

        public bool HasTransparency => false;
        public Shape CullingVolume => null;

        public IOctreeNode OctreeNode
        {
            get => _renderNode;
            set => _renderNode = value;
        }
        public bool IsRendering
        {
            get => _isRendering;
            set => _isRendering = value;
        }

        public int[,] BlockBoard => _blockBoard;

        public override void RegisterInput(InputInterface input)
        {
            input.RegisterButtonEvent(GamePadButton.SpecialRight, ButtonInputType.Pressed, Pause, InputPauseType.TickAlways);
            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, Pause, InputPauseType.TickAlways);

            input.RegisterButtonEvent(EKey.Enter, ButtonInputType.Pressed, BeginGame, InputPauseType.TickAlways);
            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, BeginGame, InputPauseType.TickOnlyWhenPaused);

            input.RegisterButtonEvent(EKey.Up, ButtonInputType.Pressed, Rotate, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Left, ButtonInputType.Pressed, MoveLeft, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Right, ButtonInputType.Pressed, MoveRight, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Down, ButtonInputType.Pressed, StartSpeedUp, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Down, ButtonInputType.Released, EndSpeedUp, InputPauseType.TickOnlyWhenUnpaused);
        }

        private void BeginGame()
        {
            _blockBoard = new int[_rows, _columns];
            for (int row = 0; row < _rows; ++row)
                for (int col = 0; col < _columns; ++col)
                {
                    _blockBoard[row, col] = -1;
                    _hudBoard[row, col].Parameter<GLVec4>(0).Value = (ColorF4)Color.Transparent;
                }

            _score = 0;
            _playing = true;
            _nextBlock = TetrisPiece.New(_rng.Next(0, 6), _columns);

            _theme.SoundPath = Engine.StartupPath + "..\\..\\..\\ProjectFiles\\" + string.Format("bgm{0}.wav", _rng.Next(1, 5));
            _theme.Play(_ambientParams, int.MaxValue);

            BlocksPerSec = DEFAULT_SPEED;
            SpawnBlock();
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, SceneUpdate);
        }
        private void GameOver()
        {
            _playing = false;
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, SceneUpdate);
            Engine.SetPaused(true, LocalPlayerController.LocalPlayerIndex);
            _theme.StopAllInstances();
        }
        private void Pause()
        {
            //Engine.TogglePause(LocalPlayerController.LocalPlayerIndex);
            //Application.Exit();
            Engine.CloseApplication();
        }
        
        private void EndSpeedUp()
        {
            BlocksPerSec = DEFAULT_SPEED;
        }

        private void StartSpeedUp()
        {
            BlocksPerSec = FAST_SPEED;
        }

        private void MoveRight()
        {
            MoveUpdateBoard(false);
            if (_currentBlock != null && !HasCollision(_currentBlock.RowShift, _currentBlock.ColumnShift + 1, _currentBlock.CurrentRotation))
                ++_currentBlock.ColumnShift;
            MoveUpdateBoard(true);
        }
        private void MoveLeft()
        {
            MoveUpdateBoard(false);
            if (_currentBlock != null && !HasCollision(_currentBlock.RowShift, _currentBlock.ColumnShift - 1, _currentBlock.CurrentRotation))
                --_currentBlock.ColumnShift;
            MoveUpdateBoard(true);
        }
        private bool MoveDown()
        {
            if (_currentBlock == null)
                return false;
            MoveUpdateBoard(false);
            if (HasCollision(_currentBlock.RowShift + 1, _currentBlock.ColumnShift, _currentBlock.CurrentRotation))
            {
                MoveUpdateBoard(true);
                return false;
            }
            else
            {
                ++_currentBlock.RowShift;
                MoveUpdateBoard(true);
                return true;
            }
        }
        private void Rotate()
        {
            if (_currentBlock == null)
                return;
            bool[,] layout = _currentBlock.GetNextRotation(true);
            MoveUpdateBoard(false);
            if (RotationSuccessful(layout, out int kickColumnShift))
            {
                _currentBlock.SetLayout(layout, _currentBlock.CurrentRotationIndex == 3 ? 0 : _currentBlock.CurrentRotationIndex + 1);
                _currentBlock.ColumnShift += kickColumnShift;
            }
            MoveUpdateBoard(true);
        }
        private bool RotationSuccessful(bool[,] layout, out int kickColumnShift)
        {
            kickColumnShift = 0;
            return _currentBlock != null && !HasCollision(_currentBlock.RowShift, _currentBlock.ColumnShift, layout);
        }
        private bool HasCollision(int rowShift, int columnShift, bool[,] blockLayout)
        {
            for (int rowIndex = 0; rowIndex < blockLayout.GetLength(0); ++rowIndex)
            {
                int boardRowIndex = rowIndex + rowShift;
                if (boardRowIndex >= _rows)
                {
                    if (CheckRowValid(rowIndex, blockLayout))
                        return true;
                    continue;
                }
                if (boardRowIndex < 0)
                    continue;
                for (int colIndex = 0; colIndex < blockLayout.GetLength(1); ++colIndex)
                {
                    int boardColIndex = colIndex + columnShift;
                    if (boardColIndex < 0 || boardColIndex >= _columns)
                    {
                        if (CheckColumnValid(colIndex, blockLayout))
                            return true;
                        continue;
                    }
                    if (blockLayout[rowIndex, colIndex] && 
                        _blockBoard[boardRowIndex, boardColIndex] >= 0)
                        return true;
                }
            }
            return false;
        }
        private bool CheckRowValid(int rowIndex, bool[,] blockLayout)
        {
            for (int colIndex = 0; colIndex < blockLayout.GetLength(1); ++colIndex)
                if (blockLayout[rowIndex, colIndex])
                    return true;
            return false;
        }
        private bool CheckColumnValid(int colIndex, bool[,] blockLayout)
        {
            for (int rowIndex = 0; rowIndex < blockLayout.GetLength(0); ++rowIndex)
                if (blockLayout[rowIndex, colIndex])
                    return true;
            return false;
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
                    {
                        if (postMove)
                        {
                            _blockBoard[boardRowIndex, boardColIndex] = _currentBlock.PieceTypeID;
                            _hudBoard[boardRowIndex, boardColIndex].Parameter<GLVec4>(0).Value = TetrisPiece.Colors[_currentBlock.PieceTypeID];
                        }
                        else
                        {
                            _blockBoard[boardRowIndex, boardColIndex] = -1;
                            _hudBoard[boardRowIndex, boardColIndex].Parameter<GLVec4>(0).Value = (ColorF4)Color.Transparent;
                        }
                    }
                }
            }
        }
        protected override DockableHudComponent OnConstruct()
        {
            _hudBoard = new MaterialHudComponent[_rows, _columns];

            Hud = this;

            TextureReference r = new TextureReference(Engine.StartupPath + "..\\..\\..\\ProjectFiles\\test.jpg");
            Material m = Material.GetUnlitTextureMaterial(r, false);
            MaterialHudComponent root = new MaterialHudComponent(m)
            {
                DockStyle = HudDockStyle.Fill
            };
            DockableHudComponent board = new DockableHudComponent()
            {
                HeightValue = _rows * 54,
                WidthValue = _columns * 54,
                //WidthHeightConstraint = WidthHeightConstraint.WidthAsRatioToHeight,
                WidthMode = SizingMode.Pixels,
                HeightMode = SizingMode.Pixels,
                OriginXPercentage = 0.5f,
                OriginYPercentage = 0.5f,
                PosXValue = 0.5f,
                PosYValue = 0.5f,
                PositionXMode = SizingMode.Percentage,
                PositionYMode = SizingMode.Percentage,
            };
            for (int row = 0; row < _rows; ++row)
                for (int col = 0; col < _columns; ++col)
                {
                    Material mat = Material.GetUnlitColorMaterial(
                        new ColorF4(row / 20.0f, 0.0f, col / 10.0f, 1.0f), false);
                    MaterialHudComponent square = new MaterialHudComponent(mat)
                    {
                        WidthValue = 54,
                        HeightValue = 54,
                        PosXValue = col * 54,
                        PosYValue = (_rows - row - 1) * 54,
                    };
                    _hudBoard[row, col] = square;
                    board.Add(square);
                }
            root.Add(board);
            return root;
        }
        public override void OnSpawned(World world)
        {
            LocalPlayerController.SetPause(true);
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
            int removedRows = 0;
            for (int rowIndex = _currentBlock.CurrentRotation.GetLength(0) - 1; rowIndex >= 0; --rowIndex)
            {
                int boardRowIndex = rowIndex + _currentBlock.RowShift + removedRows;

                //Not all of the piece could fit on screen after spawn
                if (boardRowIndex < 0)
                {
                    GameOver();
                    return;
                }

                //Should not happen
                if (boardRowIndex >= _rows)
                    continue;

                bool allFilled = true;
                for (int boardColIndex = 0; boardColIndex < _columns; ++boardColIndex)
                {
                    if (_blockBoard[boardRowIndex, boardColIndex] < 0)
                    {
                        allFilled = false;
                        break;
                    }
                }
                if (allFilled)
                {
                    ++removedRows;
                    RemoveRow(boardRowIndex);
                }
            }
            AddScore(removedRows);
            SpawnBlock();
        }
        private void RemoveRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _rows)
                return;

            bool anyValid = false;
            for (int row = rowIndex - 1; row >= 0; --row)
            {
                anyValid = false;
                for (int col = 0; col < _columns; ++col)
                {
                    anyValid = anyValid || (_blockBoard[row + 1, col] >= 0 || _blockBoard[row, col] >= 0);

                    _hudBoard[row + 1, col].Parameter<GLVec4>(0).Value = _hudBoard[row, col].Parameter<GLVec4>(0).Value;
                    _blockBoard[row + 1, col] = _blockBoard[row, col];
                }
                if (!anyValid)
                    break;
            }
            if (anyValid)
                for (int col = 0; col < _columns; ++col)
                {
                    _blockBoard[0, col] = -1;
                    _hudBoard[0, col].Parameter<GLVec4>(0).Value = new Vec4(0.0f, 0.0f, 0.0f, 1.0f);
                }
        }
        private void SpawnBlock()
        {
            _currentBlock = _nextBlock;
            _nextBlock = TetrisPiece.New(_rng.Next(0, 6), _columns);
        }
    }
    public class TetrisPiece
    {
        private int _pieceTypeID;
        private int _columnShift, _rowShift;

        private int _currentRotationIndex;
        private bool[,] _currentRotation;
        private bool[,] _pos1;
        private bool[,] _pos2;
        private bool[,] _pos3;
        private bool[,] _pos4;
        
        private TetrisPiece(
            int id, int initialRowShift, int initialColumnShift, 
            bool[,] pos1, bool[,] pos2, bool[,] pos3, bool[,] pos4)
        {
            _pieceTypeID = id;
            _rowShift = initialRowShift;
            _columnShift = initialColumnShift;
            _pos1 = pos1;
            _pos2 = pos2;
            _pos3 = pos3;
            _pos4 = pos4;
            _currentRotationIndex = 0;
            _currentRotation = _pos1;
        }

        public int PieceTypeID => _pieceTypeID;
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

        internal int CurrentRotationIndex { get => _currentRotationIndex; set => _currentRotationIndex = value; }

        internal bool[,] GetNextRotation(bool clockwise)
        {
            int r = _currentRotationIndex;
            if (clockwise)
            {
                if (r == 3)
                    r = 0;
                else
                    ++r;
            }
            else
            {
                if (r == 0)
                    r = 3;
                else
                    --r;
            }
            switch (r)
            {
                case 0: return _pos1;
                case 1: return _pos2;
                case 2: return _pos3;
                case 3: return _pos4;
            }
            return null;
        }
        internal void SetLayout(bool[,] layout, int rotationIndex)
        {
            _currentRotation = layout;
            _currentRotationIndex = rotationIndex;
        }
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
        public static readonly ColorF4[] Colors =
        {
            Color.LightSkyBlue,
            Color.Blue,
            Color.Orange,
            Color.Yellow,
            Color.Green,
            Color.Magenta,
            Color.Red,
        };
        public static TetrisPiece New(int id, int boardColumns)
        {
            int rowShift = 0, colShift = 0;
            bool[,] pos1 = null, pos2 = null, pos3 = null, pos4 = null;
            switch (id)
            {
                case 0:
                    colShift = (int)Math.Ceiling((boardColumns - 4) / 2.0);
                    rowShift = -3;
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
                    colShift = (int)Math.Ceiling((boardColumns - 3) / 2.0);
                    rowShift = -3;
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
                    colShift = (int)Math.Ceiling((boardColumns - 3) / 2.0);
                    rowShift = -3;
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
                    colShift = (int)Math.Ceiling((boardColumns - 2) / 2.0);
                    rowShift = -2;
                    pos1 = pos2 = pos3 = pos4 = new bool[,]
                    {
                        { true, true, },
                        { true, true, },
                    };
                    break;
                case 4:
                    colShift = (int)Math.Ceiling((boardColumns - 3) / 2.0);
                    rowShift = -3;
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
                    colShift = (int)Math.Ceiling((boardColumns - 3) / 2.0);
                    rowShift = -3;
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
                    colShift = (int)Math.Ceiling((boardColumns - 3) / 2.0);
                    rowShift = -3;
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
            return new TetrisPiece(id, rowShift, colShift, pos1, pos2, pos3, pos4);
        }
    }
}