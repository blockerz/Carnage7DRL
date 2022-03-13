using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public class LevelGenerator
    {
        public Region Level { get; private set; }

        public Dictionary<AreaID, AreaData> areaData;

        public LevelGeneratorData Data;

        public StateMachine stateMachine;
        List<PaverState> paverStates;

        Paver paver;
        PaverState currentPaverState;

        int rowsInCurrentState;
        float probabilityMax;

        public LevelGenerator(LevelGeneratorData levelGeneratorData)
        {
            this.Data = levelGeneratorData;

            AreaData[] allAreaData;
            Resources.LoadAll<AreaData>("ScriptableObjects/Area");
            allAreaData = Resources.FindObjectsOfTypeAll<AreaData>();

            areaData = new Dictionary<AreaID, AreaData>();

            foreach (var data in allAreaData)
            {
                areaData.Add(data.AreaID, data);
            }

            Level = new Region(Constants.MAP_TILE_WIDTH, levelGeneratorData.levelHeight, Constants.MAP_ORIGIN, Constants.MAP_AREA_X, Constants.MAP_AREA_Y);

            for (int x = 0; x < Level.Width; x++)
            {
                for (int y = 0; y < Level.Height; y++)
                {
                    Level.SetAreaAtXY(x, y, new Area(Level.GetIDFromXY(x, y)));
                    
                    AreaData data;
                    if (areaData.TryGetValue(levelGeneratorData.primaryBackground, out data))
                    {
                        Level.SetAreaDataAtXY(x, y, data);
                    }
                }
            }


            paver = new Paver(areaData, Level, levelGeneratorData);

            stateMachine = new StateMachine();
            paverStates = new List<PaverState>();
            CreateStates();
        }

        public void Dispose()
        {
            areaData.Clear();
            paverStates.Clear();
            Resources.UnloadUnusedAssets();
        }

        private void CreateStates()
        {
            int splitSize = Data.splitDistance + Data.splitEnd + Data.splitStart + Data.splitSteps;

            paverStates.Add(new PaverStraightRoadState(paver, 5, 20, Data.straightRoadChance));
            paverStates.Add(new PaverRoadWidensState(paver, 5, 15, Data.widenRoadChance));
            paverStates.Add(new PaverCurvyRoadState(paver, 10, 20, Data.curvyRoadChance));
            paverStates.Add(new PaverRoadNarrowsState(paver, 5, 10, Data.narrowRoadChance));
            paverStates.Add(new PaverRoadNarrowsLeftState(paver, 5, 10, Data.narrowLeftRoadChance));
            paverStates.Add(new PaverRoadNarrowsRightState(paver, 5, 10, Data.narrowRightRoadChance));
            paverStates.Add(new PaverBridgeState(paver, 10, 30, Data.bridgeRoadChance));
            paverStates.Add(new PaverRoadSplitState(paver, splitSize, splitSize + 3, Data.splitRoadChance));

            probabilityMax = 0;
            foreach(var paverState in paverStates)
            {
                probabilityMax += paverState.Probability;
            }
        }

        public bool SwitchToRandomState()
        {
            float currentValue = 0;
            int selectedValue = Random.Range(0, Mathf.RoundToInt(probabilityMax));
            PaverState state = null;
            rowsInCurrentState = 0;

            foreach (var paverState in paverStates)
            {
                currentValue += paverState.Probability;

                if (selectedValue <= currentValue)
                {
                    state = paverState;
                    break;
                }
            }

            if (state == null && paverStates?.Count > 0)
                state = paverStates[0];
            

            if (state != null)
            {
                rowsInCurrentState = Random.Range(state.MinHeight, state.MaxHeight + 1);
                currentPaverState = state;
                return stateMachine.ChangeState(state);
            }

            return false;
        }

        public bool PaveRoad()
        {
            //int segments = 5;
            //int segmentSize = Level.Height / segments;

            currentPaverState = paverStates[0];
            stateMachine.ChangeState(currentPaverState);
            rowsInCurrentState = Data.startRoadHeight;

            for (int y = 0; y < Level.Height; y++)
            {
                if (rowsInCurrentState <= 0)
                {
                    PaverState temp = currentPaverState;

                    SwitchToRandomState();
                    
                    // if we got the same one try again once
                    if (temp == currentPaverState)
                    {
                        SwitchToRandomState();
                    }

                    if (Data.endRoadHeight + rowsInCurrentState + y > Level.Height)
                    {
                        currentPaverState = paverStates[1];
                        stateMachine.ChangeState(currentPaverState);
                        rowsInCurrentState = Level.Height;
                    }
                }

                rowsInCurrentState--;
                paver.SetCurrentRow(y);
                stateMachine.currentState.Execute();
            }

            return true;
        }
    }
}