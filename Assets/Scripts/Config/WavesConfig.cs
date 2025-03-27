using UnityEngine;

[CreateAssetMenu(fileName="Waves Config", menuName = "Configs/WavesConfig")]
class WavesConfig : ScriptableObject
{
    [Header("Slime")]
    [SerializeField, Min(0)] int slimeStartAmount;
    [SerializeField, Min(1)] int slimeIncreasePeriod;
    [SerializeField, Min(1)] int slimeIncreaseAmount;

    [Header("Troll")]
    [SerializeField, Min(0)] int trollStartAmount;
    [SerializeField, Min(1)] int trollIncreasePeriod;
    [SerializeField, Min(1)] int trollIncreaseAmount;

    [Header("Orc")]
    [SerializeField, Min(0)] int orcStartAmount;
    [SerializeField, Min(1)] int orcIncreasePeriod;
    [SerializeField, Min(1)] int orcIncreaseAmount;


    private int NumSlimes(int waveNumber) => 
        slimeStartAmount + (slimeIncreaseAmount * (waveNumber / slimeIncreasePeriod));
    private int NumTrolls(int waveNumber) => 
        trollStartAmount + (trollIncreaseAmount * (waveNumber / trollIncreasePeriod));
    private int NumOrcs(int waveNumber) => 
        orcStartAmount + (orcIncreaseAmount * (waveNumber / orcIncreasePeriod));

    public (int, int, int) NumEnemies(int waveNumber) =>
        (
            NumSlimes(waveNumber),
            NumTrolls(waveNumber),
            NumOrcs(waveNumber)
        );
}