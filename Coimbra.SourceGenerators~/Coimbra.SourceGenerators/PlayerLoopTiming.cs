namespace Coimbra.SourceGenerators
{
    internal enum PlayerLoopTiming
    {
        PreTimeUpdate = 14,
        PostTimeUpdate = 15,
        PreInitialization = 0,
        PostInitialization = 1,
        FirstEarlyUpdate = 2,
        LastEarlyUpdate = 3,
        FirstFixedUpdate = 4,
        LastFixedUpdate = 5,
        FirstPreUpdate = 6,
        LastPreUpdate = 7,
        FirstUpdate = 8,
        LastUpdate = 9,
        PreLateUpdate = 10,
        LastLateUpdate = 11,
        PostLateUpdate = 12,
        LastPostLateUpdate = 13,
    }
}
