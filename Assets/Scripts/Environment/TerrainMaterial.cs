using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple holder class for the material type.
// This determines which footstep sounds to play.
public class TerrainMaterial : MonoBehaviour {
    public enum Enum {DEFAULT, METAL}
    public Enum activeMaterial = Enum.DEFAULT;
}
