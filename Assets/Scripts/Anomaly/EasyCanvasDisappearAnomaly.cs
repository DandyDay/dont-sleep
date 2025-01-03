using UnityEngine;

public class EasyCanvasDisappearAnomaly : Anomaly
{
    public override void Apply(GameObject map)
    {
        GameObject giraffeCanvas = storage.giraffeCanvas;
        giraffeCanvas.SetActive(false);
    }
    public override AnomalyCode GetAnomalyCode()
    {
        return AnomalyCode.EasyCanvasDisappear;
    }
}