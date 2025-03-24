using UnityEngine;

[CreateAssetMenu(fileName="Waves Config", menuName = "Configs/WavesConfig")]
class WavesConfig : ScriptableObject
{
    [Header("Slime")]
    [SerializeField, Min(0)] int slimeStartAmount;
    [SerializeField, Min(1)] int slimeIncreasePeriod;
    [SerializeField, Min(1)] int slimeIncreaseAmount;

    [Header("Skeleton")]
    [SerializeField, Min(0)] int skeletonStartAmount;
    [SerializeField, Min(1)] int skeletonIncreasePeriod;
    [SerializeField, Min(1)] int skeletonIncreaseAmount;

    [Header("Orc")]
    [SerializeField, Min(0)] int orcStartAmount;
    [SerializeField, Min(1)] int orcIncreasePeriod;
    [SerializeField, Min(1)] int orcIncreaseAmount;


    private int NumSlimes(int waveNumber) => 
        slimeStartAmount + (slimeIncreaseAmount * (waveNumber / slimeIncreasePeriod));
    private int NumSkeletons(int waveNumber) => 
        skeletonStartAmount + (skeletonIncreaseAmount * (waveNumber / skeletonIncreasePeriod));
    private int NumOrcs(int waveNumber) => 
        orcStartAmount + (orcIncreaseAmount * (waveNumber / orcIncreasePeriod));

    public (int, int, int) NumEnemies(int waveNumber) =>
        (
            NumSlimes(waveNumber),
            NumSkeletons(waveNumber),
            NumOrcs(waveNumber)
        );
}