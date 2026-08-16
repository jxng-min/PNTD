namespace PNTD
{
    public static class StageLoopUtility
    {
        public const int StagesPerLoop = 12;

        public static bool IsLoopClearStage(int stage)
        {
            return stage > 0 && stage % StagesPerLoop == 0;
        }

        public static int GetLoopCountFromStage(int stage)
        {
            return stage > 0 ? (stage - 1) / StagesPerLoop : 0;
        }

        public static int GetStageInLoop(int stage)
        {
            return stage > 0 ? ((stage - 1) % StagesPerLoop) + 1 : 1;
        }

        public static int GetLoopDisplayIndex(int loopCount)
        {
            return loopCount + 1;
        }
    }
}
