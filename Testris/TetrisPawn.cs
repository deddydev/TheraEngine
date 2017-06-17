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
    public class TetrisPawn : HudManager, IRenderable
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
        private int _level = 0;
        private Random _rng = new Random();
        private IOctreeNode _renderNode;
        private bool _isRendering;

        public float BlocksPerSec
        {
            get => 1.0f / _secondsPerBlock;
            set => _secondsPerBlock = 1.0f / value;
        }

        public bool HasTransparency => false;
        public Shape CullingVolume => null;

        public IOctreeNode RenderNode { get => _renderNode; set => _renderNode = value; }
        public bool IsRendering { get => _isRendering; set => _isRendering = value; }

        public override void RegisterInput(InputInterface input)
        {
            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, Pause, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Enter, ButtonInputType.Pressed, Start, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Up, ButtonInputType.Pressed, Rotate, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Left, ButtonInputType.Pressed, MoveLeft, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Right, ButtonInputType.Pressed, MoveRight, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Down, ButtonInputType.Pressed, StartSpeedUp, InputPauseType.TickOnlyWhenUnpaused);
            input.RegisterButtonEvent(EKey.Down, ButtonInputType.Released, EndSpeedUp, InputPauseType.TickOnlyWhenUnpaused);
        }

        private void Start()
        {
            _score = 0;
            _playing = true;
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, SceneUpdate);
        }

        private void Pause()
        {
            Engine.TogglePause(PlayerIndex.One);
        }

        private void GameEnded()
        {
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, SceneUpdate);
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
            if (_currentBlock != null)
            {
                ++_currentBlock.ColumnShift;
            }
        }

        private void MoveLeft()
        {
            if (_currentBlock != null)
            {
                --_currentBlock.ColumnShift;
            }
        }

        private void Rotate()
        {
            _currentBlock?.Rotate();
        }

        private bool MoveDown()
        {
            if (_currentBlock != null)
            {
                ++_currentBlock.RowShift;
                return true;
            }
            return false;
        }

        protected override DockableHudComponent OnConstruct()
        {
            DockableHudComponent root = new DockableHudComponent()
            {
                DockStyle = HudDockStyle.Fill
            };
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
            if (_currentBlock == null)
                SpawnBlock();
            _delta += delta;
            if (_delta >= _secondsPerBlock)
            {
                _delta = 0.0f;
                if (!MoveDown())
                {
                    _currentBlock = null;
                    BlockPlaced();
                }
            }
        }

        private void BlockPlaced()
        {

        }

        private void SpawnBlock()
        {
            _currentBlock = _nextBlock;
        }
    }
    public class TetrisPiece
    {
        private ColorF3 _color;

        private int _currentIndex;
        private bool[,] _current;
        private bool[,] _pos1;
        private bool[,] _pos2;
        private bool[,] _pos3;
        private bool[,] _pos4;

        private int _columnShift, _rowShift;

        private TetrisPiece(ColorF3 color, int initialRowShift, int initialColumnShift, bool[,] pos1, bool[,] pos2, bool[,] pos3, bool[,] pos4)
        {
            _color = color;
            _rowShift = initialRowShift;
            _columnShift = initialColumnShift;
            _pos1 = pos1;
            _pos2 = pos2;
            _pos3 = pos3;
            _pos4 = pos4;
            _currentIndex = 0;
        }

        public ColorF3 RGB => _color;
        public int ColumnShift { get => _columnShift; set => _columnShift = value; }
        public int RowShift { get => _rowShift; set => _rowShift = value; }

        internal void MoveRight()
        {

        }

        internal void MoveLeft()
        {

        }

        internal void Rotate()
        {
            if (_currentIndex == 3)
                _currentIndex = 0;
            else
                _currentIndex++;
        }

        public static TetrisPiece New(int id, int boardColumns)
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