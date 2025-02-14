﻿using TheraEngine.GameModes;
using System;
using Thera.Worlds.Actors;
using Extensions;

namespace Thera.GameModes
{
    public abstract class BaseTheraGameMode : GameMode<TheraCharacter, TheraCharacterController>
    {
        public const int MaxPossibleTeams = 16;
        
        private bool _spawnVehicles;
        private bool _spawnGuns;
        private bool _spawnMeleeWeapons;
        private bool _spawnUpgrades;
        private bool _spawnHealthPacks;
        private bool _spawnTools;
        private bool _spawnMagic;
        private bool _forceDefaultCameraType;
        private bool _firstPerson;
        private bool _canChangeCameras;

        public bool SpawnVehicles
        {
            get => _spawnVehicles;
            set => _spawnVehicles = value;
        }
        public bool SpawnGuns
        {
            get => _spawnGuns;
            set => _spawnGuns = value;
        }
        public bool SpawnMeleeWeapons
        {
            get => _spawnMeleeWeapons;
            set => _spawnMeleeWeapons = value;
        }
        public bool SpawnUpgrades
        {
            get => _spawnUpgrades;
            set => _spawnUpgrades = value;
        }
        public bool SpawnHealthPacks
        {
            get => _spawnHealthPacks;
            set => _spawnHealthPacks = value;
        }
        public bool SpawnTools
        {
            get => _spawnTools;
            set => _spawnTools = value;
        }
        public bool SpawnMagic
        {
            get => _spawnMagic;
            set => _spawnMagic = value;
        }
        public bool ForceDefaultCameraType
        {
            get => _forceDefaultCameraType;
            set => _forceDefaultCameraType = value;
        }
        public bool FirstPerson
        {
            get => _firstPerson;
            set => _firstPerson = value;
        }
        public bool CanChangeCameras
        {
            get => _canChangeCameras;
            set => _canChangeCameras = value;
        }

        private InheritablePlayerTraits _basePlayerTraits;
        private StaticPlayerTraits _staticPlayerTraits;
        private TheraCharacter _playableCharacter;
        private int _maxTeams = MaxPossibleTeams;
        private int _maxPerTeam = MaxPossibleTeams;
        private TeamTraits[] _teams = null;

        public TheraCharacter PlayableCharacter
        {
            get => _playableCharacter;
            set => _playableCharacter = value;
        }
        public bool UseTeams
        {
            get => _teams != null;
            set
            {
                if (UseTeams == value)
                    return;
                _teams = value ? new TeamTraits[_maxTeams] : null;
            }
        }

        public InheritablePlayerTraits BasePlayerTraits
        {
            get => _basePlayerTraits;
            set => _basePlayerTraits = value;
        }
        public int MaxTeams
        {
            get => _maxTeams;
            set
            {
                _maxTeams = value.Clamp(0, MaxPossibleTeams);
                _maxPerTeam = _maxPerTeam.Clamp(1, _maxTeams);
            }
        }
        public int MaxPerTeam
        {
            get => _maxPerTeam;
            set => _maxPerTeam = value.Clamp(1, _maxTeams);
        }

        public TeamTraits[] Teams => _teams;

        public virtual void OnCharacterDied(TheraCharacter character)
        {

        }
    }
}
