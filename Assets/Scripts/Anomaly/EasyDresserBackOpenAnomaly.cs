using UnityEngine;

public class EasyDresserBackOpenAnomaly : Anomaly
{
    public override void Apply(GameObject map)
    {
        GameObject dresser = storage.dresser;
        GameObject backOpenedDresser = storage.backOpenedDresser;
        dresser.SetActive(false);
        backOpenedDresser.SetActive(true);
    }
    public override AnomalyCode GetAnomalyCode()
    {
        return AnomalyCode.EasyDresserBackOpen;
    }
}