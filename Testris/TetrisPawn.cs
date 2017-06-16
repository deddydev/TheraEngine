using System;
using System.Drawing;
using TheraEngine;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.HUD;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;

namespace Testris
{
    public class TetrisPiece
    {
        private ColorF3 _color;
        private IVec2[] _blocks;
        private int _columnShift, _rowShift;

        public TetrisPiece(bool[,] blocks, ColorF3 color, int initialColumnShift)
        {
            _color = color;
            _rowShift = 0;
            _columnShift = initialColumnShift;

            int rows = blocks.GetLength(0);
            int cols = blocks.GetLength(1);
            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                {
                    bool value = blocks[i, j];
                }
        }

        public ColorF3 Color { get => _color; set => _color = value; }
        public IVec2[] BlockIndices { get => _blocks; set => _blocks = value; }
        public int ColumnShift { get => _columnShift; set => _columnShift = value; }
        public int RowShift { get => _rowShift; set => _rowShift = value; }
    }
    public class TetrisPawn : HudManager
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

        private ColorF3[] _colors = new ColorF3[]
        {
            Color.LightSkyBlue,
            Color.Orange,
            Color.Blue,
            Color.Red,
            Color.Green,
            Color.Yellow,
            Color.Magenta
        };

        bool[,] _block1 = new bool[,]
        {
            { true, true, true, true },
        };
        bool[,] _block2 = new bool[,]
        {
             { true, true, true },
             { true, false, false },
        };
        bool[,] _block3 = new bool[,]
        {
            { true, true, true },
            { false, false, true },
        };
        bool[,] _block4 = new bool[,]
        {
            { true, true, false },
            { false, true, true },
        };
        bool[,] _block5 = new bool[,]
        {
            { false, true, true },
            { true, true, false },
        };
        bool[,] _block6 = new bool[,]
        {
            { true, true, },
            { true, true, },
        };
        bool[,] _block7 = new bool[,]
        {
            { true, true, true },
            { false, true, false },
        };

        public float BlocksPerSec
        {
            get => 1.0f / _secondsPerBlock;
            set => _secondsPerBlock = 1.0f / value;
        }

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

        }

        private void MoveLeft()
        {

        }

        private void Rotate()
        {

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
                if (!TryMoveBlock())
                    _currentBlock = null;
            }
        }
        private void SpawnBlock()
        {
            _currentBlock = _nextBlock;
        }
        private bool TryMoveBlock()
        {
            throw new NotImplementedException();
        }
    }
}