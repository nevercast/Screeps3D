﻿using UnityEngine;

namespace Screeps3D.RoomObjects
{
    public class ObjectFactory
    {
        public RoomObject Get(string type)
        {
            switch (type)
            {
                case Constants.TypeCreep:
                    return new Creep();
                case Constants.TypeExtension:
                    return new Extension();
                case Constants.TypeSpawn:
                    return new Spawn();
                case Constants.TypeStorage:
                    return new Storage();
                case Constants.TypeTower:
                    return new Tower();
                case Constants.TypeController:
                    return new Controller();
                case Constants.TypeTerminal:
                    return new Terminal();
                case Constants.TypeContainer:
                    return new Container();
                case Constants.TypeLink:
                    return new Link();
                case Constants.TypeRampart:
                    return new Rampart();
                case Constants.TypeConstruction:
                    return new ConstructionSite();
                case Constants.TypeLab:
                    return new Lab();
                case Constants.TypeConstructedWall:
                    return new ConstructedWall();
                case Constants.TypeNuker:
                    return new Nuker();
                case Constants.TypeMineral:
                    return new Mineral();
                case Constants.TypePowerSpawn:
                    return new PowerSpawn();
                case Constants.TypeSource:
                    return new Source();
                case Constants.TypeTombstone:
                    return new Tombstone();
                case Constants.TypeResource:
                    return new Resource();
                case Constants.TypeSourceKeeperLair:
                    return new SourceKeeperLair();
                case Constants.TypePowerBank:
                    return new PowerBank();
                case Constants.TypePowerCreep:
                    return new PowerCreep();
                case Constants.TypePortal:
                    return new Portal();
                case Constants.TypeRoad:
                    return new Road();
                case Constants.TypeObserver:
                    return new Observer();
                case Constants.TypeExtractor:
                    return new Extractor();
                case Constants.TypeFactory:
                    return new Factory();
                case Constants.TypeDeposit:
                    return new Deposit();
                case Constants.TypeRuin:
                    return new Ruin();
                case Constants.TypeInvaderCore:
                    return new InvaderCore();
                case Constants.TypeNuke:
                    return new Nuke();

                // Season 1
                case Constants.Season1_TypeScoreContainer:
                    return new ScoreContainer();
                case Constants.Season1_TypeScoreCollector:
                    return new ScoreCollector();

                // Season 2
                case Constants.Season2_TypeSymbolContainer:
                    return new SymbolContainer();
                case Constants.Season2_TypeSymbolDecoder:
                    return new SymbolDecoder();

                default:
                    return new PlaceHolderRoomObject();
            }
        }
    }
}