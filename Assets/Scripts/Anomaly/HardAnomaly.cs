using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HardAnomaly : Anomaly
{
    public Laptop laptop;
    public abstract void SetHardAnomalyCodeForLaptop();
}
