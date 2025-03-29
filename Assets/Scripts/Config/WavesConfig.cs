using UnityEngine;

[CreateAssetMenu(fileName="Waves Config", menuName = "Configs/WavesConfig")]
class WavesConfig : ScriptableObject
{
    [SerializeField] int finalWave;
    public int FinalWave => finalWave;


    [Header("Slime")]
    [SerializeField, Min(1)] int slimeFirstWave = 1;
    [SerializeField, Min(0)] int slimeStartAmount;
    [SerializeField, Min(1)] int slimeIncreasePeriod;
    [SerializeField, Min(0)] int slimeIncreaseAmount;

    [Header("Troll")]
    [SerializeField, Min(1)] int trollFirstWave = 1;
    [SerializeField, Min(0)] int trollStartAmount;
    [SerializeField, Min(1)] int trollIncreasePeriod;
    [SerializeField, Min(0)] int trollIncreaseAmount;

    [Header("Orc")]
    [SerializeField, Min(1)] int orcFirstWave = 1;
    [SerializeField, Min(0)] int orcStartAmount;
    [SerializeField, Min(1)] int orcIncreasePeriod;
    [SerializeField, Min(0)] int orcIncreaseAmount;


    private int NumSlimes(int waveNumber) => 
        (waveNumber < slimeFirstWave) ? 0 :
        slimeStartAmount + (slimeIncreaseAmount * ((waveNumber - slimeFirstWave) / slimeIncreasePeriod));
    private int NumTrolls(int waveNumber) =>
        (waveNumber < trollFirstWave) ? 0 :
        trollStartAmount + (trollIncreaseAmount * ((waveNumber - trollFirstWave) / trollIncreasePeriod));
    private int NumOrcs(int waveNumber) =>
        (waveNumber < orcFirstWave) ? 0 :
        orcStartAmount + (orcIncreaseAmount * ((waveNumber - orcFirstWave) / orcIncreasePeriod));

    public (int, int, int) NumEnemies(int waveNumber) =>
        (
            NumSlimes(waveNumber),
            NumTrolls(waveNumber),
            NumOrcs(waveNumber)
        );
}